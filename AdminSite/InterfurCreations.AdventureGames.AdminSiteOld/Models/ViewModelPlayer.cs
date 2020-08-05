using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotAdminSite.Models
{
    public class ViewModelPlayer
    {
        public string name { get; set; }
        public int actionCount { get; set; }
        public string id { get; set; }
        public DateTime lastAction { get; set; }
    }
}