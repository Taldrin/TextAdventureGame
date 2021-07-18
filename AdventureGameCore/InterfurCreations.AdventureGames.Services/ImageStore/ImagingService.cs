using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class ImagingService : IImagingService
    {
        private readonly IImageStore _imageStore;

        private HashSet<CachedImage> _imageCache;

        public ImagingService(IImageStore imageStore) 
        {
            _imageStore = imageStore;
            _imageCache = new HashSet<CachedImage>();
        }

        public async Task<string> CreateImageAsync(List<ImageBuildParameter> imageLayers)
        {
            string imageUrl = null;
            if(!TryFindCachedImage(imageLayers, out var cachedImage))
            {
                var builder = new ImageBuilder();
                foreach (var layer in imageLayers)
                {
                    builder.AddImage(new ImageBuilderAdd
                    {
                        Position = layer.Location,
                        Opacity = layer.Opacity,
                        ImageStream = await GetStreamForImageAsync(layer.Image),
                        Size = layer.Size
                    });
                }
                var image = builder.Build();
                var stored = await _imageStore.SaveImageAsync(image);

                _imageCache.Add(new CachedImage
                {
                    Url = stored.id,
                    Params = imageLayers,
                    Added = DateTime.Now
                });
                imageUrl = stored.id;
            } else
            {
                imageUrl = cachedImage.Url;
            }
            Log.LogMessage($"Finished building image with URL: {imageUrl}");
            return imageUrl;
        }

        private async Task<Stream> GetStreamForImageAsync(string image)
        {
            var client = new HttpClient();
            var bytes = await client.GetStreamAsync(image);
            return bytes;
        }

        private bool TryFindCachedImage(List<ImageBuildParameter> imageRequest, out CachedImage cachedImage)
        {
            cachedImage = null;

            _imageCache = new HashSet<CachedImage>(_imageCache.Where(a => DateTime.Now < a.Added.AddDays(_imageStore.MaxCacheTimeDays)));

            foreach (var image in _imageCache)
            {
                if (image.Params.Count != imageRequest.Count) continue;
                bool allMatch = true;
                for(int i = 0; i < image.Params.Count; i++)
                {
                    if (!image.Params[i].Equals(imageRequest[i]))
                    {
                        allMatch = false;
                    }
                }

                if(allMatch)
                {
                    cachedImage = image;
                    return true;
                }
            }
            return false;
        }
    }

    public class CachedImage
    {
        public string Url;
        public List<ImageBuildParameter> Params;
        public DateTime Added;
    }
}
