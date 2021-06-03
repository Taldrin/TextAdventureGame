using InterfurCreations.AdminSite.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class ViewModelTestResult
    {
        public string GameName { get; set; }
        public GameTestingReportDataObject GameTestReport { get; set; }
    }
}
