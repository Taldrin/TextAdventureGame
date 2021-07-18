using System;
using System.Collections.Generic;
using System.Text;

namespace Furventure.OfflineApplications.ViewModels.Play
{
    public class MessageListItemViewModel : BaseListItemViewModel
    {
        public MessageListItemViewModel(string message)
        {
            Message = message;
        }

        public MessageListItemViewModel()
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
