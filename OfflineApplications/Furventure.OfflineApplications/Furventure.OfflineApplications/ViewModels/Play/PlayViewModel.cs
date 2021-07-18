using Furventure.OfflineApplications.ViewModels.Play;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Furventure.OfflineApplications.ViewModels
{
    public class PlayViewModel : BaseViewModel
    {
        public PlayViewModel()
        {
            Title = "Play";

            VisibleMessages = new List<BaseListItemViewModel>
            {
                new MessageListItemViewModel
                {
                    Message = "Test Message"
                },
                new MessageListItemViewModel
                {
                    Message = "Test Message2"
                },
                new MessageListItemViewModel
                {
                    Message = "Test Message3"
                },
                new PlayerListItemViewModel
                {
                    Message = "My Message"
                }
            };

            GameButtons = new List<ButtonBaseViewModel>
            {
                new GameButtonViewModel
                {
                    Text = "Test Button 1"
                }
            };


        }

        public List<BaseListItemViewModel> VisibleMessages
        {
            get => _visibleMessage;
            set => SetField(ref _visibleMessage, value);
        }
        private List<BaseListItemViewModel> _visibleMessage;

        public List<ButtonBaseViewModel> GameButtons
        {
            get => _gameButtons;
            set => SetField(ref _gameButtons, value);
        }
        private List<ButtonBaseViewModel> _gameButtons;
    }
}