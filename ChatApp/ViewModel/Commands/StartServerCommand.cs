using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Commands
{
    internal class StartServerCommand : ICommand
    {
        private readonly ConnectScreenViewModel parent;

        public StartServerCommand(ConnectScreenViewModel parent)
        {
            this.parent = parent;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            parent.StartConnection();
        }
    }
}
