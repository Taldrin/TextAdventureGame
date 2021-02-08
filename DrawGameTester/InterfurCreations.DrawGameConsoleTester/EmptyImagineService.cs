using InterfurCreations.AdventureGames.Services.ImageStore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.DrawGameConsoleTester
{
    public class EmptyImagineService : IImagingService
    {
        public async Task<string> CreateImageAsync(List<ImageBuildParameter> imageLayers)
        {
            return "";
        }
    }
}
