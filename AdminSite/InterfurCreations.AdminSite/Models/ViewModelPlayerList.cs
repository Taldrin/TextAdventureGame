using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotAdminSite.Models
{
    public class ViewModelPlayerList
    {
        public ViewModelPlayerList()
        {
            Players = new List<ViewModelPlayer>();
        }
        public List<ViewModelPlayer> Players { get; set; }
        public int TotalPlayersCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int NumberOfPages { get; set; }
        public string PlayerPlatformFilter { get; set; }
        public string PlayerNameFilter { get; set; }
    }
}