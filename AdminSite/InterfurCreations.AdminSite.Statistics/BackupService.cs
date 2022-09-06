using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks
{
    public class BackupService
    {
        private readonly DatabaseAdminFunctions _adminFunctions;

        public BackupService(DatabaseAdminFunctions adminFunctions)
        {
            _adminFunctions = adminFunctions;
        }

        public void PerformDatabaseBackup()
        {
            _adminFunctions.ExecuteBackup();
        }
    }
}
