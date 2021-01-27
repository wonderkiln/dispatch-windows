using System;
using System.Windows.Input;
namespace Dispatch.Helpers
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecutable;
        public bool IsExecutable
        {
            get
            {
                return _isExecutable;
            }
            set
            {
                _isExecutable = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly Action<T> action;

        public RelayCommand(Action<T> action, bool executable = true)
        {
            this.action = action;
            IsExecutable = executable;
        }

        public bool CanExecute(object parameter)
        {
            return IsExecutable;
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }
    }
}
