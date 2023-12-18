using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Commands
{
    internal class DenyRequestCommand : ICommand
    {
        private readonly ChatScreenViewModel parent;

        public DenyRequestCommand(ChatScreenViewModel parent)
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
            parent.DeclineIncoming();
        }
    }
}
