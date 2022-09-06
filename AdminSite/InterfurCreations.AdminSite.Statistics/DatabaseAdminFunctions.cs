using InterfurCreations.AdventureGames.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks
{
    public class DatabaseAdminFunctions
    {
        private readonly IConfigurationService _configService;

        public DatabaseAdminFunctions(IConfigurationService configService)
        {
            _configService = configService;
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

            if(File.Exists(savePath))
            {
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

        }
    }
}
