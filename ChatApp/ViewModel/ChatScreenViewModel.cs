using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ChatScreenViewModel
    {
        private readonly NetworkManager networkManager;

        private ICommand sendMessage;

        private History messageHistory;

        public ICommand SendMessage
        {
            get
            {
                sendMessage ??= new SendMessageCommand(this);
                return sendMessage;
            }

            set { sendMessage = value; }
        }

        public Message[] MessageHistory => messageHistory.Messages;

        public ChatScreenViewModel(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.messageHistory = new History();

            this.sendMessage = new SendMessageCommand(this);

            this.messageHistory.AddMessage(new Message("yo", "bro"));
        }

        public void AddHistory(Message message)
        {
            messageHistory.AddMessage(message);
        }
    }
}
