using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks
{
    public class DatabaseAdminFunctions
    {
        private readonly IConfigurationService _configService;
        private readonly IGoogleDriveService _googleDriveService;

        public DatabaseAdminFunctions(IConfigurationService configService , IGoogleDriveService googleDriveService)
        {
            _configService = configService;
            googleDriveService = googleDriveService;
        }

        public void ExecuteBackup()
        {
            var connectionString = _configService.GetConfig("DatabaseConnectionString");
            var backupPath = _configService.GetConfig("DBBackupPath");

            Server botServer = new Server();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            botServer.ConnectionContext.ConnectionString = connectionString;

            Backup backupMgr = new Backup();

            var fileName = $"Full_Backup_{ DateTime.Now.ToFileTimeUtc()}";

            backupMgr.Database = builder.InitialCatalog;
            backupMgr.BackupSetDescription = "Full Automatic backup of Furventure " + DateTime.Now.ToLongDateString().ToString();
            backupMgr.BackupSetName = "Furventure " + DateTime.Now.ToFileTimeUtc().ToString();
            backupMgr.Action = BackupActionType.Database;

            BackupDeviceItem bdi = default(BackupDeviceItem);
            bdi = new BackupDeviceItem(fileName, DeviceType.File);

            backupMgr.Devices.Add(bdi);
            backupMgr.Incremental = false;

            backupMgr.SqlBackup(botServer);

            var desiredPath = backupPath + "/" + fileName;
            var savePath = "/var/opt/mssql/data/" + fileName;

            Thread.Sleep(TimeSpan.FromSeconds(10));

            if(File.Exists(savePath))
            {
                Console.WriteLine("Backup file exists -- moving");
                File.Move(savePath, desiredPath);
            } else
            {
                Console.WriteLine("Backup file does not exist");

                try
                {
                    File.Create(savePath + "-testfile.txt");
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Uploading to Google Drive");
            using (var fileStream = new FileStream(desiredPath, FileMode.Open))
            {
                _googleDriveService.UploadFile($"FULL_DATABASE_BACKUP_{DateTime.Now.ToString("D")}", "Backups", fileStream);
            }

        }
    }
}
