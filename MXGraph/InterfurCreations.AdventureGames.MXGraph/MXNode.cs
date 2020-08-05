using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.MXVisualGraph
{
    public class MXNode
    {
        public MXNode()
        {
            Connections = new List<MXNodeConnection>();
        }

        public string NodeText { get; set; }
        public List<MXNodeConnection> Connections { get; set; }
        public int x;
        public int y;
        public string NodeID { get; set; }
    }
}
