using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Furventure.AdventureGames.Offline.ViewModels
{
    public class ButtonViewModel : ViewModelBase
    {
        private readonly MainPageViewModel _vm;

        public ButtonViewModel(MainPageViewModel vm, string message)
        {
            Message = message;
            ButtonPressed = new RelayCommand(_ => OnButtonPressed());
            _vm = vm;
        }

        public void OnButtonPressed()
        {
            _vm.HandleMessage(Message);
        }

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }
        private string _message;

        public ICommand ButtonPressed
        {
            get => _buttonPressed;
            set => SetField(ref _buttonPressed, value);
        }
        private ICommand _buttonPressed;
    }
}
