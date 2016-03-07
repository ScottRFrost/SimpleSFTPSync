using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinSCP;

namespace SimpleSFTPSync
{
    /// <summary>
    /// The main form
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Entity Framework
        /// </summary>
        private readonly SimpleSFTPSyncEntities db = new SimpleSFTPSyncEntities();

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Close form after timeout
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimerTick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Start threaded main process
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainShown(object sender, EventArgs e)
        {
            Ui("Form", "SimpleSFTPSync - Starting Up...");
            Application.DoEvents();
            Ui("Form", "SimpleSFTPSync - Beginning Main Loop...");
            ////var x = await MainLoop();  //Doing this instead of Task.Run can be helpful for debugging.  Remember to set Main_Shown to async as well.
            Task.Run(() => MainLoop());
        }

        /// <summary>
        /// Async process to control main functionality.  Most work occurs here.
        /// </summary>
        /// <returns>Number of files downloaded</returns>
        private async Task<int> MainLoop()
        {
            Ui("Form", "SimpleSFTPSync - Connecting to DB...");
            var filesDownloaded = 0;
            var rars = new List<string>();
            var mkvs = new List<string>();

            // Connect
            try
            {
                Ui("Status", "Found " + db.Connections.Count() + " connections");
            }
            catch (Exception ex)
            {
                Ui("Error", "InitialDBConnetion - " + ex);
                return 0;
            }

            // Update
            foreach (var checkconnection in db.Connections.OrderBy(c => c.ConnectionID))
            {
                try
                {
                    // Setup session options
                    Ui("Status", "Connecting to " + checkconnection.Name);
                    Ui("Form", " SimpleSFTPSync - Checking " + checkconnection.Name + " for new files");
                    var sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = checkconnection.Hostname,
                        PortNumber = checkconnection.Port,
                        UserName = checkconnection.Username,
                        Password = checkconnection.Password,
                        SshHostKeyFingerprint = checkconnection.SshHostKeyFingerprint
                    };
                    int foundFiles;
                    using (var checksession = new Session())
                    {
                        // Connect
                        checksession.Open(sessionOptions);
                        Ui("Log", "Checking " + checkconnection.Name + " for new files");
                        foundFiles = await ListFilesRecursive(checksession, checkconnection.ConnectionID, checkconnection.RemotePath, string.Empty);
                    }
                    Ui("Status", checkconnection.Name + " - " + foundFiles + " files found");
                }
                catch (Exception ex)
                {
                    Ui("Error", "Updating " + checkconnection.Name + " - " + ex);
                }
                finally
                {
                    Ui("Form", "SimpleSFTPSync - Completed " + checkconnection.Name);
                }
            }

            // Download
            foreach (var connection in db.Connections.OrderBy(c => c.ConnectionID))
            {
                try
                {
                    Ui("Log", "Downloading from " + connection.Name);
                    Ui("Status", "Connecting to " + connection.Name);
                    Ui("Form", "SimpleSFTPSync - Downloading From " + connection.Name);
                    Application.DoEvents();
                    var sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = connection.Hostname,
                        PortNumber = connection.Port,
                        UserName = connection.Username,
                        Password = connection.Password,
                        SshHostKeyFingerprint = connection.SshHostKeyFingerprint
                    };
                    var downloadsession = new Session();
                    downloadsession.FileTransferProgress += SessionFileTransferProgress;
                    downloadsession.Open(sessionOptions);
                    using (downloadsession)
                    {
                        var connectionId = connection.ConnectionID;
                        foreach (var file in db.Files.Where(f => f.DateDownloaded == null && f.ConnectionID == connectionId).OrderBy(f => f.DateDiscovered))
                        {
                            try
                            {
                                var localPath = (connection.LocalPath + file.RemotePath).Replace("/", "\\").Replace("\\\\", "\\").Replace("\\\\", "\\");
                                var localDirectory = localPath.Substring(0, localPath.LastIndexOf("\\", StringComparison.Ordinal));
                                if (System.IO.File.Exists(localPath))
                                {
                                    var localFile = new FileInfo(localPath);
                                    if (localFile.Length == file.Length)
                                    {
                                        Ui("Log", connection.Name + " " + file.RemotePath + " and " + localPath + " are the same size.  Skipping.");
                                        file.DateDownloaded = DateTime.Now;
                                        db.SaveChanges();
                                        continue;
                                    }
                                    else
                                    {
                                        Ui("Log", connection.Name + " " + file.RemotePath + " and " + localPath + " are different sizes.  Replacing.");
                                        System.IO.File.Delete(localPath);
                                    }
                                }
                                if (downloadsession.FileExists(connection.RemotePath + file.RemotePath))
                                {
                                    if (!Directory.Exists(localDirectory))
                                    {
                                        Directory.CreateDirectory(localDirectory);
                                    }
                                    var transferOperationResult = downloadsession.GetFiles(connection.RemotePath + file.RemotePath, localPath);
                                    if (transferOperationResult.IsSuccess)
                                    {
                                        Ui("Log", connection.Name + " downloaded " + file.RemotePath + " to " + localPath);
                                        file.DateDownloaded = DateTime.Now;
                                        db.SaveChanges();
                                        filesDownloaded++;
                                        if (localPath.EndsWith(".part1.rar", StringComparison.Ordinal) || !localPath.Contains(".part") && localPath.EndsWith(".rar", StringComparison.Ordinal))
                                        {
                                            rars.Add(localPath);
                                            Ui("Log", connection.Name + " Added " + localPath + " to auto-unrar queue");
                                        }
                                        else if (localPath.EndsWith(".mkv", StringComparison.Ordinal))
                                        {
                                            mkvs.Add(localPath);
                                        }
                                    }
                                    else
                                    {
                                        Ui("Log", connection.Name + " downloaded " + file.RemotePath + " error - " + transferOperationResult.Failures[0].Message);
                                    }
                                }
                                else
                                {
                                    Ui("Log", connection.Name + " downloaded " + file.RemotePath + " no longer exists");
                                    file.DateDownloaded = DateTime.Now;
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                Ui("Error", "Downloading " + file + " - " + ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Ui("Error", "Download - " + connection.Name + " - " + ex);
                }
                finally
                {
                    Ui("Form", "SimpleSFTPSync - Completed " + connection.Name);
                }
            }
            Ui("Status", string.Empty);
            Ui("Form", "SimpleSFTPSync - " + filesDownloaded + " files downloaded.");
            Application.DoEvents();

            // Unrar
            foreach (var rar in rars)
            {
                try
                {
                    var unrarFolder = rar.Substring(0, rar.LastIndexOf("\\", StringComparison.Ordinal) + 1) + "_unrar";
                    if (!Directory.Exists(unrarFolder))
                    {
                        Directory.CreateDirectory(unrarFolder);
                    }
                    Ui("Form", "SimpleSFTPSync - Unraring " + rar);
                    var process = Process.Start(new ProcessStartInfo("UnRAR.exe") { Arguments = "x -o- \"" + rar + "\" \"" + unrarFolder + "\"" }); // x = extract, -o- = Don't overwrite or prompt to overwrite
                    if (process == null)
                    {
                        continue;
                    }
                    process.WaitForExit();
                    Ui("Log", "Unrared " + rar);
                    Thread.Sleep(2000); // Wait for unrar.exe to completely clean up
                    mkvs.AddRange(Directory.GetFiles(unrarFolder, "*.mkv"));
                }
                catch (Exception ex)
                {
                    Ui("Error", "UnRar of " + rar + " - " + ex);
                }
            }
            Application.DoEvents();

            // MKV move & rename
            foreach (var mkv in mkvs.Where(mkv => !mkv.Contains("Sample")))
            {
                try
                {
                    Ui("Form", "SimpleSFTPSync - Moving " + mkv);

                    // Determine if TV or Movie
                    if (mkv.ToLower().Contains("hdtv") || mkv.ToLower().Contains("webrip") || mkv.ToLower().Contains("web-dl"))
                    {
                        var filename = Rename.TV(mkv);
                        System.IO.File.Move(mkv, ConfigurationManager.AppSettings["TVFolder"] + '\\' + filename);
                        Ui("Log", "Moved TV " + mkv + " to " + ConfigurationManager.AppSettings["TVFolder"] + '\\' + filename);
                    }
                    else
                    {
                        var filename = Rename.Movie(mkv);
                        System.IO.File.Move(mkv, ConfigurationManager.AppSettings["MovieFolder"] + '\\' + filename);
                        Ui("Log", "Moved Movie " + mkv + " to " + ConfigurationManager.AppSettings["MovieFolder"] + '\\' + filename);
                    }
                }
                catch (Exception ex)
                {
                    Ui("Error", "Move of " + mkv + " - " + ex);
                }
            }
            Ui("Form", "SimpleSFTPSync - " + filesDownloaded + " files downloaded.  All jobs complete.  Closing in 60 seconds...");
            Ui("Timer", string.Empty);
            return filesDownloaded;
        }

        /// <summary>
        /// Scan entire directory structure of remote SFTP server
        /// </summary>
        /// <param name="session">WinSCP Session</param>
        /// <param name="connectionId">ConnectionId for saving new files to EF</param>
        /// <param name="basePath">Root of the remote file system</param>
        /// <param name="subDirectory">Subdirectory to look for files and further subdirectories in</param>
        /// <returns>Number of files found</returns>
        private async Task<int> ListFilesRecursive(Session session, int connectionId, string basePath, string subDirectory)
        {
            Ui("Status", "Checking /" + subDirectory);
            var foundFiles = 0;
            var remoteDirectoryInfo = session.ListDirectory(basePath + "/" + subDirectory);
            try
            {
                foreach (var fileInfo in remoteDirectoryInfo.Files.OrderBy(f => f.Name))
                {
                    var filePath = subDirectory + "/" + fileInfo.Name;
                    if (fileInfo.IsDirectory)
                    {
                        if (!fileInfo.Name.StartsWith("."))
                        {
                            foundFiles += await ListFilesRecursive(session, connectionId, basePath, filePath);
                        }
                    }
                    else
                    {
                        var file =
                            db.Files.FirstOrDefault(f => f.ConnectionID == connectionId && f.RemotePath == filePath);
                        if (file == null)
                        {
                            Ui("Log", "Found New file: " + filePath);
                            db.Files.Add(new File
                            {
                                ConnectionID = connectionId,
                                DateDiscovered = DateTime.Now,
                                DateDownloaded = null,
                                Length = fileInfo.Length,
                                RemoteDateModified = fileInfo.LastWriteTime,
                                RemotePath = filePath
                            });
                            db.SaveChanges();
                            foundFiles++;
                        }
                        else if (file.Length != fileInfo.Length || file.RemoteDateModified != fileInfo.LastWriteTime)
                        {
                            Ui("Log", "Found Modified file: " + filePath);
                            file.DateDownloaded = null;
                            file.Length = fileInfo.Length;
                            file.RemoteDateModified = fileInfo.LastWriteTime;
                            db.SaveChanges();
                            foundFiles++;
                        }
                    }
                }
                return foundFiles;
            }
            catch (Exception ex)
            {
                Ui("Error", "ListFilesRecursive - " + ex);
                return 0;
            }
        }

        /// <summary>
        /// Callback to update progress as file transfers
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SessionFileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            Ui("Status", "Downloading " + e.FileName + " at " + Convert.ToInt32(e.CPS / 1024) + " K/sec " + (e.OverallProgress * 100) + "%");
            Ui("Progress", Convert.ToInt32(e.OverallProgress * 100).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Class to cleanly allow background threads access to the main UI thread
        /// </summary>
        /// <param name="item">
        /// The item to change.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        private void Ui(string item, string value)
        {
            if (item != "Progress" && !(item == "Status" && value.StartsWith("Downloading ", StringComparison.Ordinal)))
            {
                var streamWriter = new StreamWriter(DateTime.Now.ToString("MM-dd-yyyy") + ".log", true);
                streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff") + " " + item + " " + value);
                streamWriter.Close(); 
            }
            switch (item)
            {
                case "Form":
                    Invoke(new Action(() => Text = value));
                    break;
                case "Status":
                    Invoke(new Action(() => Status.Text = value));
                    break;
                case "Log":
                    Invoke(new Action(() => Log.Items.Add(value)));
                    break;
                case "Error":
                    Invoke(new Action(() => Log.Items.Add("ERROR " + value)));
                    break;
                case "Timer":
                    Invoke(new Action(() => Timer.Enabled = true));
                    break;
                case "TimerOff":
                    Invoke(new Action(() => Timer.Enabled = false));
                    break;
                case "Progress":
                    Invoke(new Action(() => ProgressBar.Value = Convert.ToInt32(value)));
                    break;
            }
        }

        /// <summary>
        /// Load setup page
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SetupButtonButtonClick(object sender, EventArgs e)
        {
            var s = new Setup();
            Ui("Form", "SimpleSFTPSync - Setup Opened.  Close app manually when finished.");
            Ui("TimerOff", string.Empty);
            s.ShowDialog();
        }
    }
}
