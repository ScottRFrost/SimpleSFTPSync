SimpleSFTPSync
==============

OBJECT: Download files ONCE from a remote SFTP, even if you move or delete the files from the download location!

This SUPER simple (I did it in half of a day) app uses WinSCP to download from a remote SFTP server, saving each download to a MS SQL Server Compact 4.0 database.  As each file is successfully downloaded, it saves it to the database and then it won't download it again unless the file size changes, even if you delete or move the file out of your download directory.

To use it, just open up the excellent included Sql Server CE toolkit (https://sqlcetoolbox.codeplex.com/) and edit the connections table of the SimpleSFTPSync.sdf in the root.  Once the connections are in there, just run the app and it will do the rest.  It closes 60 seconds after the run finishes rather than using some built in timer, so you may want to set it to run via Scheduled Task at 1AM every night or some other time when your internet connection is idle.

Entity Framework 5 is included, but you may need to install the SQL Compact 4.0 runtime (https://www.microsoft.com/en-us/download/details.aspx?id=30709) if you don't have Visual Studio on the target machine.

I thought I'd go ahead and throw it up on github in case anyone else was having similar needs.  Input always appreciated!
