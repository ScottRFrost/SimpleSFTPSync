namespace SimpleSFTPSync
{
    partial class Setup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.hostname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.sshkey = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.localPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.remotePath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.saveSetup = new System.Windows.Forms.Button();
            this.sqlCommand = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.execSqlCommand = new System.Windows.Forms.Button();
            this.parseTV = new System.Windows.Forms.Button();
            this.renameText = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.parseMovie = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(12, 25);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(107, 20);
            this.name.TabIndex = 5;
            // 
            // hostname
            // 
            this.hostname.Location = new System.Drawing.Point(125, 25);
            this.hostname.Name = "hostname";
            this.hostname.Size = new System.Drawing.Size(172, 20);
            this.hostname.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(125, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Hostname";
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(303, 25);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(48, 20);
            this.port.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(303, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Port";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(357, 25);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(113, 20);
            this.username.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(357, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Username";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(476, 25);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(255, 20);
            this.password.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(476, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Password";
            // 
            // sshkey
            // 
            this.sshkey.Location = new System.Drawing.Point(12, 80);
            this.sshkey.Name = "sshkey";
            this.sshkey.Size = new System.Drawing.Size(719, 20);
            this.sshkey.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "SSH Key Fingerprint";
            // 
            // localPath
            // 
            this.localPath.Location = new System.Drawing.Point(12, 126);
            this.localPath.Name = "localPath";
            this.localPath.Size = new System.Drawing.Size(719, 20);
            this.localPath.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Local Path";
            // 
            // remotePath
            // 
            this.remotePath.Location = new System.Drawing.Point(12, 177);
            this.remotePath.Name = "remotePath";
            this.remotePath.Size = new System.Drawing.Size(638, 20);
            this.remotePath.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Remote Path";
            // 
            // saveSetup
            // 
            this.saveSetup.Location = new System.Drawing.Point(656, 174);
            this.saveSetup.Name = "saveSetup";
            this.saveSetup.Size = new System.Drawing.Size(75, 23);
            this.saveSetup.TabIndex = 20;
            this.saveSetup.Text = "Save Setup";
            this.saveSetup.UseVisualStyleBackColor = true;
            this.saveSetup.Click += new System.EventHandler(this.saveSetup_Click);
            // 
            // sqlCommand
            // 
            this.sqlCommand.Location = new System.Drawing.Point(12, 276);
            this.sqlCommand.Multiline = true;
            this.sqlCommand.Name = "sqlCommand";
            this.sqlCommand.Size = new System.Drawing.Size(719, 223);
            this.sqlCommand.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 260);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(145, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "SQL Command (PRO MODE)";
            // 
            // execSqlCommand
            // 
            this.execSqlCommand.Location = new System.Drawing.Point(588, 517);
            this.execSqlCommand.Name = "execSqlCommand";
            this.execSqlCommand.Size = new System.Drawing.Size(143, 23);
            this.execSqlCommand.TabIndex = 23;
            this.execSqlCommand.Text = "Execute SQL Command";
            this.execSqlCommand.UseVisualStyleBackColor = true;
            this.execSqlCommand.Click += new System.EventHandler(this.execSqlCommand_Click);
            // 
            // parseTV
            // 
            this.parseTV.Location = new System.Drawing.Point(533, 632);
            this.parseTV.Name = "parseTV";
            this.parseTV.Size = new System.Drawing.Size(96, 23);
            this.parseTV.TabIndex = 26;
            this.parseTV.Text = "Parse As TV";
            this.parseTV.UseVisualStyleBackColor = true;
            this.parseTV.Click += new System.EventHandler(this.parseTV_Click);
            // 
            // renameText
            // 
            this.renameText.Location = new System.Drawing.Point(12, 603);
            this.renameText.Name = "renameText";
            this.renameText.Size = new System.Drawing.Size(719, 20);
            this.renameText.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 587);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Rename Logic Tester";
            // 
            // parseMovie
            // 
            this.parseMovie.Location = new System.Drawing.Point(635, 632);
            this.parseMovie.Name = "parseMovie";
            this.parseMovie.Size = new System.Drawing.Size(96, 23);
            this.parseMovie.TabIndex = 27;
            this.parseMovie.Text = "Parse as Movie";
            this.parseMovie.UseVisualStyleBackColor = true;
            this.parseMovie.Click += new System.EventHandler(this.parseMovie_Click);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 667);
            this.Controls.Add(this.parseMovie);
            this.Controls.Add(this.parseTV);
            this.Controls.Add(this.renameText);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.execSqlCommand);
            this.Controls.Add(this.sqlCommand);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.saveSetup);
            this.Controls.Add(this.remotePath);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.localPath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.sshkey);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.password);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.username);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.port);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.hostname);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Setup";
            this.ShowIcon = false;
            this.Text = "SimpleSFTPSync - Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.TextBox hostname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox sshkey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox localPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox remotePath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button saveSetup;
        private System.Windows.Forms.TextBox sqlCommand;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button execSqlCommand;
        private System.Windows.Forms.Button parseTV;
        private System.Windows.Forms.TextBox renameText;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button parseMovie;
    }
}