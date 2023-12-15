using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Commands
{
    internal class ChangeConversationCommand : ICommand
    {
        private readonly ChatScreenViewModel parent;

        public ChangeConversationCommand(ChatScreenViewModel parent)
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
            ObservableCollection<Message> newMessages = (ObservableCollection<Message>)parameter;
            System.Diagnostics.Debug.WriteLine(parameter.ToString());
            parent.CurrentConversation = newMessages;
        }
    }
}
