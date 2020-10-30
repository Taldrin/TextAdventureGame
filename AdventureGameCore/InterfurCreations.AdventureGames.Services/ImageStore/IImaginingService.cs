using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public interface IImagingService
    {
        Task<string> CreateImageAsync(List<ImageBuildParameter> imageLayers);
    }
}
