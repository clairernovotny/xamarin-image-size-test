using System;
using System.Windows.Input;

namespace ImageBrowser
{
    /// <summary>
    /// Basic ICommand implementation
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action execute)
            : this(execute, null)
        {
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute();
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (execute != null)
            {
                execute();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var Handler = CanExecuteChanged;
            if (Handler != null)
            {
                Handler(this, new EventArgs());
            }
        }
    }
}
