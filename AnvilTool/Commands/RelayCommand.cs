using System;
using System.Windows;
using System.Windows.Input;

namespace AnvilTool.Commands
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _execute;
        private Predicate<object> _canExecute;

        public RelayCommand(Action<object> Execute, Predicate<object> CanExecute)
        {
            _execute = Execute;
            _canExecute = CanExecute;
        }

        public RelayCommand(Action<object> Execute) : this(Execute, null)
        {

        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) ? true : _canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _execute.Invoke(parameter);
            });
        }

        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
