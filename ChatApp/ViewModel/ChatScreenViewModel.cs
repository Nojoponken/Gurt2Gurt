using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ChatScreenViewModel : INotifyPropertyChanged
    {
        private readonly NetworkManager networkManager;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        private ICommand sendMessage;

        private ObservableCollection<Message> messageHistory;

        public ICommand SendMessage
        {
            get
            {
                sendMessage ??= new SendMessageCommand(this);
                return sendMessage;
            }

            set { sendMessage = value; }
        }

        public ObservableCollection<Message> MessageHistory
        {
            get
            {
                return messageHistory;
            }
        }

        public string PendingConnect
        {
            get
            {
                if (networkManager.Pending)
                {
                    return "Visible";
                }
                return "Hidden";
            }
        } 

        public ChatScreenViewModel()
        {
            this.messageHistory = new ObservableCollection<Message>();

            this.sendMessage = new SendMessageCommand(this);
        }

        public ChatScreenViewModel(ref NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.messageHistory = new ObservableCollection<Message>();

            this.sendMessage = new SendMessageCommand(this);
        }

        public void AddHistory(Message message)
        {
            this.messageHistory.Add(new Message("yo", "john", "user"));
            System.Diagnostics.Debug.WriteLine($"{messageHistory[0]}");
            PropertyChanged.Invoke(this, new(nameof(MessageHistory)));
        }
    }
}
