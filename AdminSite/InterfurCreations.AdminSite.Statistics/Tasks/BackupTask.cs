using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks.Tasks
{
    public class BackupTask : IBackgroundTask
    {
        private readonly BackupService _backupService;

        public BackupTask(BackupService backupService)
        {
            _backupService = backupService;
        }

        public void Run()
        {
            _backupService.PerformDatabaseBackup();
        }
    }
}
