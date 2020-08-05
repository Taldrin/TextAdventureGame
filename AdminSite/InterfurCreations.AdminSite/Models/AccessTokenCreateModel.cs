using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class AccessTokenCreateModel
    {
        public int HoursAllowed { get; set; }
        public string TokenType { get; set; }
    }
}
