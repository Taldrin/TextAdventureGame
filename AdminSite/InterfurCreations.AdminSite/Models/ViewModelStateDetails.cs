using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class ViewModelStateDetails
    {
        public string GameName { get; set; }
        public string StateId { get; set; }
        public string StateText { get; set; }
        public List<ViewModelOptionDetails> StateOptions { get; set; }
        public List<string> StateAttachments { get; set; }
    }

    public class ViewModelOptionDetails
    {
        public string OptionId { get; set; }
        public string OptionText { get; set; }
        public string ResultStateId { get; set; }
        public string ResultStateText { get; set; }
    }

}
