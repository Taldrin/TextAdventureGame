using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public interface IImageStore
    {
        public const int MaxCacheTimeDays = 2;

        Task<int> CleanupImagesOlderThan(DateTime time);
        Task<StoredImage> SaveImageAsync(Stream imageStream);
    }
}
