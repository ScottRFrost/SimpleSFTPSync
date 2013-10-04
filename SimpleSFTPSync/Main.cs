﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WinSCP;

namespace SimpleSFTPSync
{
    public partial class Main : Form
    {
        /// <summary>
        /// Entity Framework
        /// </summary>
        private readonly SimpleSFTPSyncEntities _db = new SimpleSFTPSyncEntities();

        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Close form after timeout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Start threaded main process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Task.Run(() => MainLoop());
        }

        /// <summary>
        /// Async process to control main functionality.  Most work occurs here.
        /// </summary>
        /// <returns>Number of files downloaded</returns>
        private async Task<int> MainLoop()
        {
            var filesDownloaded = 0;
            //Update
            foreach (var checkconnection in _db.Connections.OrderBy(c => c.ConnectionID))
            {
                try
                {
                    // Setup session options
                    Ui("Status","Connecting to " + checkconnection.Name);
                    Ui("Form","SimpleSFTPSync - Checking " + checkconnection.Name);
                    SessionOptions sessionOptions = new SessionOptions
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
                        Ui("Log", "Checking " + checkconnection.Name);
                        foundFiles = await ListFilesRecursive(checksession, 1, "/home/torrents", "");
                    }
                    Ui("Status",checkconnection.Name + " - " + foundFiles + " files found");
                }
                catch (Exception ex)
                {
                    Ui("Log",checkconnection.Name + " ERROR - " + ex);
                }
                finally
                {
                    Ui("Form","SimpleSFTPSync - Completed " + checkconnection.Name);
                }
            }

            //Download
            foreach (var connection in _db.Connections.OrderBy(c => c.ConnectionID))
            {
                Ui("Status","Connecting to " + connection.Name);
                Ui("Form","SimpleSFTPSync - Downloading From " + connection.Name);
                SessionOptions sessionOptions = new SessionOptions
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
                    foreach (var file in _db.Files.Where(f => f.DateDownloaded == null && f.ConnectionID == connectionId).OrderBy(f => f.DateDiscovered))
                    {
                        try
                        {
                            var localPath = (connection.LocalPath + file.RemotePath).Replace("/", "\\").Replace("\\\\", "\\").Replace("\\\\", "\\");
                            var localDirectory = localPath.Substring(0, localPath.LastIndexOf("\\"));
                            if (System.IO.File.Exists(localPath))
                            {
                                var localFile = new FileInfo(localPath);
                                if (localFile.Length == file.Length)
                                {
                                    Ui("Log",connection.Name + " " + file.RemotePath + " and " + localPath + " are the same size.  Skipping.");
                                    file.DateDownloaded = DateTime.Now;
                                    _db.SaveChanges();
                                    continue;
                                }
                                else
                                {
                                    Ui("Log",connection.Name + " " + file.RemotePath + " and " + localPath + " are different sizes.  Replacing.");
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
                                    Ui("Log",connection.Name + " downloaded " + file.RemotePath + " to " + localPath);
                                    file.DateDownloaded = DateTime.Now;
                                    _db.SaveChanges();
                                    filesDownloaded++;
                                }
                                else
                                {
                                    Ui("Log",connection.Name + " downloaded " + file.RemotePath + " error - " + transferOperationResult.Failures[0].Message);
                                }
                            }
                            else
                            {
                                Ui("Log",connection.Name + " downloaded " + file.RemotePath + " no longer exists");
                                file.DateDownloaded = DateTime.Now;
                                _db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            Ui("Log",connection.Name + " ERROR - " + ex);
                        }
                        finally
                        {
                            Ui("Form","SimpleSFTPSync - Completed " + connection.Name);
                        }
                    }
                }
            }
            Ui("Status",String.Empty);
            Ui("Form","SimpleSFTPSync - " + filesDownloaded + " files downloaded.  All jobs complete.  Closing in 60 seconds...");
            Ui("Timer", String.Empty);
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
            Ui("Status","Checking /" + subDirectory);
            var foundFiles = 0;
            var remoteDirectoryInfo = session.ListDirectory(basePath + "/" + subDirectory);
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
                    var file = _db.Files.FirstOrDefault(f => f.ConnectionID == connectionId && f.RemotePath == filePath);
                    if (file == null)
                    {
                        Ui("Log", "Found New file: " + filePath);
                        _db.Files.Add(new File
                        {
                            ConnectionID = connectionId,
                            DateDiscovered = DateTime.Now,
                            DateDownloaded = null,
                            Length = fileInfo.Length,
                            RemoteDateModified = fileInfo.LastWriteTime,
                            RemotePath = filePath
                        });
                        _db.SaveChanges();
                        foundFiles++;
                    }
                    else if (file.Length != fileInfo.Length || file.RemoteDateModified != fileInfo.LastWriteTime)
                    {
                        Ui("Log", "Found Modified file: " + filePath);
                        file.DateDownloaded = null;
                        file.Length = fileInfo.Length;
                        file.RemoteDateModified = fileInfo.LastWriteTime;
                        _db.SaveChanges();
                        foundFiles++;
                    }
                }
            }
            return foundFiles;
        }

        /// <summary>
        /// Callback to update progress as file transfers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionFileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            Ui("Status","Downloading " + e.FileName + " at " + Convert.ToInt32(e.CPS / 1024) + " K/sec " + (e.OverallProgress * 100) + "%");
            Ui("Progress",Convert.ToInt32(e.OverallProgress*100).ToString());
        }

        /// <summary>
        /// Class to cleanly allow background threads access to the main UI thread
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        private void Ui(string item, string value)
        {
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
                case "Timer":
                    Invoke(new Action(() => Timer.Enabled = true));
                    break;
                case "Progress":
                    Invoke(new Action(() => ProgressBar.Value = Convert.ToInt32(value)));
                    break;
            }
        }
    }
}
