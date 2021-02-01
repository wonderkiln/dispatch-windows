using System;
using System.Windows.Input;
namespace Dispatch.Helpers
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action action, bool executable = true) : base((parameter) => action(), executable)
        {
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> action;

        public event EventHandler CanExecuteChanged;

        private bool isExecutable;
        public bool IsExecutable
        {
            get
            {
                return isExecutable;
            }
            set
            {
                isExecutable = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }


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
