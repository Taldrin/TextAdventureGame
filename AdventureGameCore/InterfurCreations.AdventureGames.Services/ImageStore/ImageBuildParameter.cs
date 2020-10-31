using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class ImageBuildParameter
    {
        public ImageBuildParameter()
        {
            Location = Vector2.Zero;
            Opacity = 1;
        } 

        public string Image { get; set; }
        public Vector2 Location { get; set; }
        public Vector2? Size { get; set; }
        public float Opacity { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is ImageBuildParameter imageBuild)
            {
                return imageBuild.Image == Image && imageBuild.Location == Location && imageBuild.Size == Size && imageBuild.Opacity == Opacity;
            }
            return base.Equals(obj);
        }
    }

}
