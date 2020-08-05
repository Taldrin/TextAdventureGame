using InterfurCreations.AdventureGames.Kik.KikObjects;
using InterfurCreations.AdventureGames.Kik.KikObjects.Send;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public static class KikMethods
    {
        public static async Task<KikConfigDataObject> SendConfigAsync(KikHttp kikHttp, KikConfigDataObject config)
        {
            return await kikHttp.PostRequestAsync<KikConfigDataObject>("config", config);
        }

        public static async Task SendMessageAsync(KikHttp kikHttp, KikMessagesSendDataObject message)
        {
            await kikHttp.PostRequestAsync("message", message);
        }
    }
}
