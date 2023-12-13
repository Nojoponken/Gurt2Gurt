using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{

    class ChatScreenViewModel : INotifyPropertyChanged
    {
        // Colors
        const string Gray = "#ddd";
        const string Red = "#faa";
        const string Green = "#afa";
        const string Blue = "#acf";

        // Fields
        private readonly NetworkManager networkManager;
        private ObservableCollection<Message> messageHistory;
        private string messageContent;
        private string username;
        private string pendingVisibility;
        private string disconnectVisibility;
        private string status;
        private string statusColor;

        private bool pending;
        private bool connected;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Window userWindow;


        private ICommand sendMessage;
        private ICommand denyRequest;
        private ICommand acceptRequest;
        private ICommand disconnect;

        public ICommand SendMessage
        {
            get
            {
                sendMessage ??= new SendMessageCommand(this);
                return sendMessage;
            }
            set { sendMessage = value; }
        }
        public ICommand DenyRequest
        {
            get
            {
                denyRequest ??= new DenyRequestCommand(this);
                return denyRequest;
            }
            set { denyRequest = value; }
        }
        public ICommand AcceptRequest
        {
            get
            {
                acceptRequest ??= new AcceptRequestCommand(this);
                return acceptRequest;
            }
            set { acceptRequest = value; }
        }
        public ICommand Disconnect
        {
            get
            {
                disconnect ??= new DisconnectCommand(this);
                return disconnect;
            }
            set { disconnect = value; }
        }


        public ObservableCollection<Message> MessageHistory { get { return messageHistory; } }

        public string MessageContent { get { return messageContent; } set { messageContent = value; OnPropertyChanged(); } }
        public string Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public string PendingVisibility { get { return pendingVisibility; } set { pendingVisibility = value; OnPropertyChanged(); } }
        public string DisconnectVisibility { get { return disconnectVisibility; } set { disconnectVisibility = value; OnPropertyChanged(); } }
        public string Status { get { return status; } set { status = value; OnPropertyChanged(); } }
        public string StatusColor { get { return statusColor; } set { statusColor = value; OnPropertyChanged(); } }
        public bool Pending { get { return pending; } set { pending = value; OnPropertyChanged(); } }
        public bool Connected { get { return connected; } set { connected = value; OnPropertyChanged(); } }

        public ChatScreenViewModel() { }

        public ChatScreenViewModel(ref NetworkManager networkManager, ChatScreen window)
        {
            userWindow = window;
            this.networkManager = networkManager;
            this.username = networkManager.Username;
            this.messageHistory = new ObservableCollection<Message>();
            this.pendingVisibility = "Hidden";
            this.disconnectVisibility = "Hidden";
            this.pending = false;
            this.connected = false;
            this.status = "Listening for connection...";
            this.statusColor = Gray;

            networkManager.IsClient += OnIsClient;
            networkManager.CloseClient += OnCloseClient;

            networkManager.PendingClient += OnPendingClient;
            networkManager.AcceptClient += OnAcceptClient;
            networkManager.DenyClient += OnDenyClient;

            networkManager.MessageRecieved += OnMessageRecieved;
            networkManager.MessageSent += OnMessageSent;

            networkManager.Disconnected += OnDisconnected;
            userWindow.Closed += OnClosing;

        }

        public void OnClosing(object? sender, EventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine($"lmao");
            networkManager.Disconnect();
        }

        public void OnDisconnected(object? sender, EventArgs eventArgs)
        {
            Status = "Disconnected";
            StatusColor = Red;
            DisconnectVisibility = "Hidden";
            Connected = false;
        }

        public void DisconnectChat()
        {
            networkManager.Disconnect();
        }

        public void Send()
        {
            networkManager.SendMessage(MessageContent);
        }

        public void DeclineIncoming()
        {
            networkManager.WantConnect = "deny";
        }

        public void AcceptIncoming()
        {
            networkManager.WantConnect = "accept";
        }



        public void OnMessageRecieved(object? sender, Message message)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                messageHistory.Add(message);
            });
            OnPropertyChanged(nameof(MessageHistory));
        }

        public void OnMessageSent(object? sender, Message message)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                messageHistory.Add(message);
            });
            MessageContent = "";
            System.Diagnostics.Debug.WriteLine($"{message.Content}");
            OnPropertyChanged(nameof(MessageHistory));
        }


        private void OnPendingClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");
            Status = $"{user} wants to connect";
            StatusColor = Blue;

            PendingVisibility = "Visible";
            Pending = true;
        }

        private void OnAcceptClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");

            Status = $"Chatting with {user}";
            StatusColor = Green;

            PendingVisibility = "Hidden";
            Pending = false;
            DisconnectVisibility = "Visible";
            connected = true;
        }


        private void OnDenyClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");

            Status = $"Listening for connection...";
            StatusColor = Gray;

            PendingVisibility = "Hidden";
            Pending = false;
        }

        private void OnIsClient(object? sender, EventArgs empty)
        {
            Status = "Waiting for response...";
            StatusColor = Blue;
        }

        private void OnCloseClient(object? sender, EventArgs empty)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                userWindow.Close();
            });
        }
    }
}
