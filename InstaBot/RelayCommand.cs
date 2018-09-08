using System;
using System.Windows.Input;

namespace InstaBot
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _command;

        public RelayCommand(Action<object> command)
        {
            _command = command;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _command(parameter);
        }
    }
}
