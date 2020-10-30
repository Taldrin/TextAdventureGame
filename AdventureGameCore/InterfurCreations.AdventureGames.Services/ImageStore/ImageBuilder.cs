using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class ImageBuilder
    {
        private Image _image;

        public ImageBuilder()
        {
            _image = null;
        }

        public ImageBuilder AddImage(ImageBuilderAdd add)
        {
            if (_image == null)
            {
                _image = Image.Load(add.ImageStream);
                return this;
            }

            using (Image image = Image.Load(add.ImageStream))
            {
                if (add.Size != null)
                    image.Mutate(a => a.Resize(new ResizeOptions { Mode = ResizeMode.BoxPad, Size = new Size((int)add.Size.Value.X, (int)add.Size.Value.Y) }));
                _image.Mutate(a => a.DrawImage(image, new Point((int)add.Position.X, (int)add.Position.Y), add.Opacity));

            }
            return this;
        }

        public Stream Build()
        {
            var memoryStream = new MemoryStream();
            _image.SaveAsJpeg(memoryStream);
            return memoryStream;
        }
    }
}
