using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Commands
{
    internal class RestartServerCommand : ICommand
    {
        private readonly ChatScreenViewModel parent;

        public RestartServerCommand(ChatScreenViewModel parent)
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
            parent.StartListener();
        }
    }
}
