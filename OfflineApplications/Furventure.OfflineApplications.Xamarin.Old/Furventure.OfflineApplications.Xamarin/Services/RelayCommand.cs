using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Furventure.OfflineApplications.Services
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");

            if (canExecute != null)
            {
                this._canExecute = canExecute;
            }
        }

        public RelayCommand(Action execute): this(execute, null)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _execute.Invoke();
            }
        }
    }
}
