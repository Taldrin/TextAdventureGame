using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class ImageStoreCleanupTask
    {
        private readonly IImageStore _imageStore;
        private readonly IReporter _reporter;

        public ImageStoreCleanupTask(IImageStore imageStore, IReporter reporter)
        {
            _imageStore = imageStore;
            _reporter = reporter;
        }

        public async Task ClearImages()
        {
            var minDate = DateTime.Now.Subtract(TimeSpan.FromDays(IImageStore.MaxCacheTimeDays + 1));
            var amountDeleted = await _imageStore.CleanupImagesOlderThan(minDate);

            _reporter.ReportMessage("Image Store Cleanup task ran, and deleted: " + amountDeleted + " images.");
        }
    }
}
