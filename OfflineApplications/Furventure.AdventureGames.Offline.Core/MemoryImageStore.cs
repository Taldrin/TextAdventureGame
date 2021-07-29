using InterfurCreations.AdventureGames.Services.ImageStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.Offline.Core
{
    public class MemoryImageStore : IImageStore
    {
        public int MaxCacheTimeDays => 1;
        private readonly MemoryCache _cache;
        private readonly CacheItemPolicy _policy;

        public MemoryImageStore()
        {
            _cache = new MemoryCache("imageCache");
            _policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            };
        }

        public Task<int> CleanupImagesOlderThan(DateTime time)
        {
            throw new NotImplementedException();
        }

        public async Task<StoredImage> SaveImageAsync(Stream imageStream)
        {
            var key = Guid.NewGuid().ToString();
            using (var memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);
                _cache.Add(key, memoryStream.ToArray(), _policy);
                return new StoredImage { id = key };
            }

        }
    }
}
