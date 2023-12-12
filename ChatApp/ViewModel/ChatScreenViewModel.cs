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
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ChatScreenViewModel : INotifyPropertyChanged
    {
        private readonly NetworkManager networkManager;
        private ObservableCollection<Message> messageHistory;
        private string pending;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ICommand sendMessage;
        public ICommand SendMessage
        {
            get
            {
                sendMessage ??= new SendMessageCommand(this);
                return sendMessage;
            }
            set { sendMessage = value; }
        }

        public ObservableCollection<Message> MessageHistory { get { return messageHistory; } }

        public string Pending { get { return pending; } set { pending = value; OnPropertyChanged(); } }

        //public ChatScreenViewModel()
        //{
        //    this.messageHistory = new ObservableCollection<Message>();
        //    this.sendMessage = new SendMessageCommand(this);
        //    this.pending = "Hidden";

        //    networkManager.PendingClient += OnPendingClient;

        //}

        public ChatScreenViewModel(ref NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.messageHistory = new ObservableCollection<Message>();
            this.sendMessage = new SendMessageCommand(this);
            this.pending = "Hidden";


            networkManager.PendingClient += OnPendingClient;
        }

        public void AddHistory(Message message)
        {
            this.messageHistory.Add(message);
            System.Diagnostics.Debug.WriteLine($"{message.Content}");
            OnPropertyChanged(nameof(MessageHistory));
        }

        private void OnPendingClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");

            Pending = "Visible";
        }
    }
}
