using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Commands
{
    internal class DisconnectCommand : ICommand
    {
        private readonly ChatScreenViewModel parent;

        public DisconnectCommand(ChatScreenViewModel parent)
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
            parent.DisconnectChat();
        }
    }
}
