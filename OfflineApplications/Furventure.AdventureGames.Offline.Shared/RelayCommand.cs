using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Furventure.AdventureGames.Offline
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        private event EventHandler CanHandle;

        public event EventHandler CanExecuteChanged
        {
            add { CanHandle += value; }
            remove { CanHandle -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
