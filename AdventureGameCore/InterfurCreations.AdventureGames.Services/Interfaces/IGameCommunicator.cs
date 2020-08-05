using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public interface IGameCommunicator
    {
        Task ShowUserMessageAsync(string message);
        Task ShowUserMessageAsync(string message, List<string> replies);
        Task ShowUserImageAsync(string imageUrl, List<string> replies);
        void HardReset();
        string GetGameId();
    }
}
