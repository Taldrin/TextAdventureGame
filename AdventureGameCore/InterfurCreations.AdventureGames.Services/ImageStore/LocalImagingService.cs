using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class LocalImagingService : IImagingService
    {
        private readonly IImageStore _imageStore;
        private readonly string _storePath;

        private HashSet<CachedImage> _imageCache;
        private Dictionary<string, byte[]> _rawImages;

        public LocalImagingService(IImageStore imageStore, IConfigurationService config) 
        {
            _imageStore = imageStore;
            _storePath = config.GetConfig("ImageStorePath", false);
            _imageCache = new HashSet<CachedImage>();

            var filePaths = Directory.GetFiles(_storePath, "*", SearchOption.TopDirectoryOnly);
            _rawImages = new Dictionary<string, byte[]>(filePaths.Length);

            foreach (var path in filePaths)
            {
                try
                {
                    var content = File.ReadAllBytes(path);
                    _rawImages[Path.GetFileNameWithoutExtension(path)] = content;
                }
                catch (IOException ioEx)
                {
                    Console.Error.WriteLine($"Warning: could not read file '{path}': {ioEx.Message}");
                }
            }
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
                        ImageStream = new MemoryStream(_rawImages[Path.GetFileNameWithoutExtension(new Uri(layer.Image).Segments.LastOrDefault())]),
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

        private bool TryFindCachedImage(List<ImageBuildParameter> imageRequest, out CachedImage cachedImage)
        {
            cachedImage = null;

            _imageCache = _imageCache.Where(a => DateTime.Now < a.Added.AddDays(IImageStore.MaxCacheTimeDays)).ToHashSet();

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
}
