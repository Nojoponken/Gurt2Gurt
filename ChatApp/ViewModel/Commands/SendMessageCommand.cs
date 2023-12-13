using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Commands
{
    internal class SendMessageCommand : ICommand
    {
        private readonly ChatScreenViewModel parent;

        public SendMessageCommand(ChatScreenViewModel parent)
        {
            this.parent = parent;
            //this.author = author;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            parent.Send();
        }
    }
}
