using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InterfurCreations.AdventureGames.Graph
{
    public class DrawState
    {
        public DrawState()
        {
            StateOptions = new List<StateOption>();
            StateAttachements = new List<StateAttachment>();
        }

        public  string Id;
        public  string StateText;
        public  List<StateOption> StateOptions;
        public  List<StateAttachment> StateAttachements;
        public  bool IsImage;

        public XElement XmlElement;
    }
}
