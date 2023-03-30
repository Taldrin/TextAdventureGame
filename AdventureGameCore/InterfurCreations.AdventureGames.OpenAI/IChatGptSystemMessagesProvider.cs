using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.OpenAI
{
    public interface IChatGptSystemMessagesProvider
    {
        string GetFirstAppendMessage();
        string GetInitialMessage();
    }
}
