using System;
using System.Windows.Input;

namespace AnvilTool.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _act;
        private readonly Func<object, bool> _can;
        public RelayCommand(Action<T> a, Func<object, bool> canExecute = null) { _act = a; _can = canExecute; }
        public bool CanExecute(object parameter) => _can == null ? true : _can.Invoke(parameter);

        public void Execute(object parameter) => _act((T)parameter);
        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _act;
        private readonly Func<object, bool> _can;
        public RelayCommand(Action<object> a, Func<object, bool> canExecute = null) { _act = a; _can = canExecute; }
        public bool CanExecute(object parameter) => _can == null ? true : _can.Invoke(parameter);
        public void Execute(object parameter) => _act(parameter);
        public event EventHandler CanExecuteChanged;
    }
}
