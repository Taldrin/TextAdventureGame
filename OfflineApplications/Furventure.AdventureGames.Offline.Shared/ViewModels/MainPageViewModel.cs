using Autofac;
using Furventure.AdventureGames.Offline.Core;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Furventure.AdventureGames.Offline.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            ButtonSelectedCommand = new RelayCommand(a => HandleMessage(a.ToString()));
            start();
        }

        private void start()
        {
            HandleMessage("Start");
        }

        public void HandleMessage(string message)
        {
            using (var scope = OfflineAppInitialiser.Container.BeginLifetimeScope())
            { 
                var messageProcessor = scope.Resolve<OfflineMessageProcessor>();
                var result = messageProcessor.ProcessMessage(message);

                if (DisplayedMessages != null)
                    DisplayedMessages.AddRange(result.MessagesToShow.Select(a => a.Message).ToList());
                else
                    DisplayedMessages = result.MessagesToShow.Select(a => a.Message).ToList();

                var saved = DisplayedMessages.Select(a => a);
                DisplayedMessages = saved.ToList();
                DisplayedButtons = result.OptionsToShow.Select(a => new ButtonViewModel(this, a)).ToList();

                scope.Resolve<IDatabaseContextProvider>().GetContext().SaveChanges();
            }
        }

        public List<string> DisplayedMessages
        {
            get => _displayedMessages;
            set => SetField(ref _displayedMessages, value);
        }
        private List<string> _displayedMessages;

        public List<ButtonViewModel> DisplayedButtons
        {
            get => _displayedButtons;
            set => SetField(ref _displayedButtons, value);
        }
        private List<ButtonViewModel> _displayedButtons;

        public ICommand ButtonSelectedCommand
        {
            get => _buttonSelectedCommand;
            set => SetField(ref _buttonSelectedCommand, value);
        }
        private ICommand _buttonSelectedCommand;
    }
}
