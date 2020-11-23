using System;
using System.Windows.Input;

namespace Dispatch.Helpers
{
    public class RelayCommand : ICommand
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
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        private readonly Action<object> ExecuteAction;

        public RelayCommand(Action<object> action, bool executable = true)
        {
            ExecuteAction = action;
            IsExecutable = executable;
        }

        public bool CanExecute(object parameter)
        {
            return IsExecutable;
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }
    }
}
