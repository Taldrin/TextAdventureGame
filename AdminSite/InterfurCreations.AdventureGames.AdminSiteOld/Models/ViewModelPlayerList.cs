using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotAdminSite.Models
{
    public class ViewModelPlayerList
    {
        public List<ViewModelPlayer> Players { get; set; }
        public int TotalActionsCount { get; set; }
    }
}