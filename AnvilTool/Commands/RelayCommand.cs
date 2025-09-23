using System;
using System.Windows.Input;

namespace AnvilTool.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _act;
        public RelayCommand(Action<T> a) { _act = a; }
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _act((T)parameter);
        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _act;
        public RelayCommand(Action<object> a) { _act = a; }
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _act(parameter);
        public event EventHandler CanExecuteChanged;
    }
}
