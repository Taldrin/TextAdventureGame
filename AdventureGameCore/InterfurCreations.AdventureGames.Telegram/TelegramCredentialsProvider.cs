using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Telegram
{
    public class TelegramCredentialsProvider : ICredentialsPathProvider
    {
        public string GetPath()
        {
            return "credentials.json";
        }
    }
}
