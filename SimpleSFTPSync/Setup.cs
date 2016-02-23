using System;
using System.Linq;
using System.Windows.Forms;

namespace SimpleSFTPSync
{
    /// <summary>
    /// The setup form
    /// </summary>
    public partial class Setup : Form
    {
        /// <summary>
        /// Entity Framework
        /// </summary>
        private readonly SimpleSFTPSyncEntities _db = new SimpleSFTPSyncEntities();

        /// <summary>
        /// Initializes a new instance of the <see cref="Setup"/> class. 
        /// Constructor
        /// </summary>
        public Setup()
        {
            InitializeComponent();

            // Load data
            if (!_db.Connections.Any())
            {
                return;
            }
            var connection = _db.Connections.First();
            name.Text = connection.Name;
            hostname.Text = connection.Hostname;
            port.Text = connection.Port.ToString();
            username.Text = connection.Username;
            password.Text = connection.Password;
            sshkey.Text = connection.SshHostKeyFingerprint;
            remotePath.Text = connection.RemotePath;
            localPath.Text = connection.LocalPath;
        }

        /// <summary>
        /// Button that saves setup to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveSetup_Click(object sender, EventArgs e)
        {
            Connection connection;
            var isNew = false;
            if (_db.Connections.Any())
            {
                connection = _db.Connections.First();
            }
            else
            {
                connection = new Connection();
                isNew = true;
            }
            connection.Name = name.Text;
            connection.Hostname = hostname.Text;
            connection.Port = Convert.ToInt16(port.Text);
            connection.Username = username.Text;
            connection.Password = password.Text;
            connection.SshHostKeyFingerprint = sshkey.Text;
            connection.RemotePath = remotePath.Text;
            connection.LocalPath = localPath.Text;
            if (isNew)
            {
                _db.Connections.Add(connection);
            }
            _db.SaveChanges();
            Text = @"Changes Saved Successfully!";
        }

        /// <summary>
        /// Button that executes an arbitrary SQL command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void execSqlCommand_Click(object sender, EventArgs e)
        {
            _db.Database.ExecuteSqlCommand(sqlCommand.Text);
            Text = @"Command executed!";
        }

        private void parseTV_Click(object sender, EventArgs e)
        {
            Text = Rename.TV(renameText.Text);
        }

        private void parseMovie_Click(object sender, EventArgs e)
        {
            Text = Rename.Movie(renameText.Text);
        }
    }
}
