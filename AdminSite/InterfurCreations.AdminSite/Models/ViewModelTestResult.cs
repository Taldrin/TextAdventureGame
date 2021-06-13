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
        public string CustomTestStartState { get; set; }
        public int CustomTestMaxActions { get; set; }
        public int CustomTestMinutesToRunFor { get; set; }
        public int CustomTestTimesToRun { get; set; }
        public string CustomTestStartData { get; set; }
    }
}
