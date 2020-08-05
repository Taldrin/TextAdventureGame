using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.HeartbeatMonitor
{
    public interface IHeartbeatMonitor
    {
        void BeginMonitor(string heartbeatUrl);
    }
}
