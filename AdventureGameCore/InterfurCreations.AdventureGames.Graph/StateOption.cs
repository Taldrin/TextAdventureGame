using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InterfurCreations.AdventureGames.Graph
{
    public class StateOption
    {
        public string Id;
        public string StateText;
        public DrawState ResultState;
        public XElement XmlElement;
        // Direct transitions mean there is no choice to be made - directly move to the ResultState
        public bool IsDirectTransition;
    }
}
