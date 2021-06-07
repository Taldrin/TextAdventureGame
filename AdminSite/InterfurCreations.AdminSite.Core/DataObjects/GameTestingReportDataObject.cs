using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Core.DataObjects
{
    public class GameTestingReportDataObject
    {
        public List<GameTestingReportItemWithSave> Errors { get; set; }
        public List<GameTestingReportItemWithSave> Warnings { get; set; }
        public List<GameTestingReportItemWithSave> Grammar { get; set; }
        public List<GameTestingReportItemWithSave> EndStates { get; set; }
        public List<string> StatesNeverVisited { get; set; }
        public List<string> OptionsNeverTaken { get; set; }
        public List<string> Variables { get; set; }
        public int TotalActionsTaken { get; set; }
    }

    public class GameTestingReportItemWithSave
    {
        public string Data;
        public string SaveInfo;
    }
}
