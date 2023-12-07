using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
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

        private ICommand sendMessage;

        private List<Message> messageHistory;

        public ICommand SendMessage
        {
            get
            {
                sendMessage ??= new SendMessageCommand(this);
                return sendMessage;
            }

            set { sendMessage = value; }
        }

        public List<Message> MessageHistory
        {
            get
            {
                return messageHistory;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]  String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ChatScreenViewModel(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.messageHistory = new List<Message>();

            this.sendMessage = new SendMessageCommand(this);
        }

        public void AddHistory(Message message)
        {
            this.messageHistory.Add(new Message("yo", "john"));
            System.Diagnostics.Debug.WriteLine($"{messageHistory[0]}");
            NotifyPropertyChanged();
        }
    }
}
