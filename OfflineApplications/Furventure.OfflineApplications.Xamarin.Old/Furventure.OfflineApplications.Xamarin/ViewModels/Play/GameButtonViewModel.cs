using Furventure.OfflineApplications.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Furventure.OfflineApplications.ViewModels.Play
{
    public class GameButtonViewModel : ButtonBaseViewModel
    {
        public GameButtonViewModel()
        {
            Clicked = new RelayCommand(() => Console.WriteLine($"Clicked {Text}"));
        }

        public string Text
        {
            get => _text;
            set => SetField(ref _text, value);
        }
        private string _text;
    }
}
