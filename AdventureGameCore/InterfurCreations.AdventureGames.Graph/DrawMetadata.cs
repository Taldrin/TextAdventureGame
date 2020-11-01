using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Graph
{
    public class DrawMetadata
    {
        public DrawMetadata()
        {
            Achievements = new List<DrawAchievement>();
            PermanentButtons = new List<DrawPermanentButton>();
        }

        public string Description { get; set; }
        public string Category { get; set; }
        public List<DrawAchievement> Achievements { get; set; }
        public List<DrawPermanentButton> PermanentButtons { get; set; }
    }
}
