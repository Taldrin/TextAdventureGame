using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class ImageBuildDataTracker
    {
        private List<ImageBuildParameter> _currentParams;

        public ImageBuildDataTracker()
        {
            _currentParams = new List<ImageBuildParameter>();
        }

        public void AddParam(ImageBuildParameter newParam)
        {
            _currentParams.Add(newParam);
        }

        public List<ImageBuildParameter> GetParams()
        {
            return _currentParams;
        }
    }
}
