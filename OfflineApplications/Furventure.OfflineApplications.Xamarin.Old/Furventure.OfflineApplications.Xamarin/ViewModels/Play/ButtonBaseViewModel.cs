using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Furventure.OfflineApplications.ViewModels.Play
{
    public class ButtonBaseViewModel : BaseViewModel
    {
        public ICommand Clicked
        {
            get => _clicked;
            set => SetField(ref _clicked, value);
        }
        private ICommand _clicked;
    }
}
