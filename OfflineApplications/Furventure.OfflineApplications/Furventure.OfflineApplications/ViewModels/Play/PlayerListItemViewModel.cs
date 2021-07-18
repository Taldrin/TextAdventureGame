using System;
using System.Collections.Generic;
using System.Text;

namespace Furventure.OfflineApplications.ViewModels.Play
{
    public class PlayerListItemViewModel : BaseListItemViewModel
    {
        public PlayerListItemViewModel(string message)
        {
            Message = message;
        }

        public PlayerListItemViewModel()
        {
        }

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }
        private string _message;
    }
}
