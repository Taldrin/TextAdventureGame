using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class ImageBuilderAdd
    {
        public Stream ImageStream { get; set; }
        public float Opacity { get; set; }
        public Vector2 Position { get; set; }
        public Vector2? Size { get; set; }
    }
}
