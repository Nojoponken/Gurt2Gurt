using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{

    class ChatScreenViewModel : INotifyPropertyChanged
    {
        // -- Color constants -------- //
        const string Gray = "#ddd";
        const string Red = "#faa";
        const string Green = "#afa";
        const string Blue = "#acf";
        // --------------------------- //

        // -- Fields ----------------- //
        private bool isClient;
        private Window userWindow;
        private NetworkManager networkManager;

        private Conversation currentConversation;
        private ObservableCollection<Conversation> messageHistory;
        private string searchQuery;
        private string messageContent;

        private string? username;
        private string? ip;
        private string? port;

        private string status;
        private string statusColor;
        private string pendingVisibility;
        private string disconnectVisibility;
        private string restartServerVisibility;
        private string connected;
        // --------------------------- //

        // -- INotifyPropertyChanged implementation -------------------------------- //
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        // ------------------------------------------------------------------------- //

        // -- ICommands -------------- //
        private ICommand? acceptRequest;
        private ICommand? denyRequest;
        private ICommand? sendMessage;
        private ICommand? buzz;
        private ICommand? disconnect;
        private ICommand? restartServer;

        public ICommand? AcceptRequest
        {
            get
            {
                acceptRequest ??= new AcceptRequestCommand(this);
                return acceptRequest;
            }
            set { acceptRequest = value; }
        }

        public ICommand? DenyRequest
        {
            get
            {
                denyRequest ??= new DenyRequestCommand(this);
                return denyRequest;
            }
            set { denyRequest = value; }
        }

        public ICommand? SendMessage
        {
            get
            {
                sendMessage ??= new SendMessageCommand(this);
                return sendMessage;
            }
            set { sendMessage = value; }
        }

        public ICommand? Buzz
        {
            get
            {
                buzz ??= new BuzzCommand(this);
                return buzz;
            }
            set { buzz = value; }
        }

        public ICommand? Disconnect
        {
            get
            {
                disconnect ??= new DisconnectCommand(this);
                return disconnect;
            }
            set { disconnect = value; }
        }

        public ICommand? RestartServer
        {
            get
            {
                restartServer ??= new RestartServerCommand(this);
                return restartServer;
            }
            set { restartServer = value; }
        }
        // --------------------------- //

        // -- Properties ----------------------------------------------------------- //
        public Conversation CurrentConversation { get { return currentConversation; } set { currentConversation = value; OnPropertyChanged(); } }
        public ObservableCollection<Conversation> MessageHistory { get { return new ObservableCollection<Conversation>(messageHistory.Where(i => i.Peer2.Contains(SearchQuery)).ToList()); } set { messageHistory = value; OnPropertyChanged(); } }
        public string SearchQuery { get { return searchQuery; } set { searchQuery = value; OnPropertyChanged(); OnPropertyChanged(nameof(MessageHistory)); } }
        public string MessageContent { get { return messageContent; } set { messageContent = value; OnPropertyChanged(); } }

        public string? Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public string? IP { get { return ip; } set { ip = value; OnPropertyChanged(); } }
        public string? Port { get { return port; } set { port = value; OnPropertyChanged(); } }

        public string Status { get { return status; } set { status = value; OnPropertyChanged(); } }
        public string StatusColor { get { return statusColor; } set { statusColor = value; OnPropertyChanged(); } }
        public string PendingVisibility { get { return pendingVisibility; } set { pendingVisibility = value; OnPropertyChanged(); } }
        public string DisconnectVisibility { get { return disconnectVisibility; } set { disconnectVisibility = value; OnPropertyChanged(); } }
        public string RestartServerVisibility { get { return restartServerVisibility; } set { restartServerVisibility = value; OnPropertyChanged(); } }
        public string Connected { get { return connected; } set { connected = value; OnPropertyChanged(); OnPropertyChanged(nameof(Disconnected)); } }
        public string Disconnected
        {
            get
            {
                if (connected == "True")
                { return "False"; }
                else
                { return "True"; }
            }
        }
        // ------------------------------------------------------------------------- //

        // -- Constructors ----------- //
        public ChatScreenViewModel() { }

        public ChatScreenViewModel(ref NetworkManager networkManager, ChatScreen window)
        {
            userWindow = window;
            this.isClient = false;
            this.networkManager = networkManager;

            networkManager.IsClient += OnIsClient;
            networkManager.CloseClient += OnCloseClient;
            networkManager.NewEndpoint += OnNewEndpoint;

            networkManager.PendingClient += OnPendingClient;
            networkManager.AcceptClient += OnAcceptClient;
            networkManager.DenyClient += OnDenyClient;

            networkManager.MessageReceived += OnMessageReceived;
            networkManager.MessageSent += OnMessageSent;
            networkManager.Buzzed += OnBuzzed;

            networkManager.Disconnected += OnDisconnected;
            userWindow.Closed += OnClosing;

            this.username = networkManager.Username;
            this.IP = networkManager.IP;
            this.Port = networkManager.Port;

            this.searchQuery = "";
            this.messageContent = "";

            this.status = "Listening for connection...";
            this.statusColor = Gray;
            this.pendingVisibility = "Hidden";
            this.disconnectVisibility = "Hidden";
            this.restartServerVisibility = "Hidden";
            this.connected = "False";

            this.messageHistory = History.LoadHistory();
            if (messageHistory == null)
            {
                this.messageHistory = new ObservableCollection<Conversation>();
            }
            OnPropertyChanged("MessageHistory");

        }
        // --------------------------- //

        // -- Methods ---------------- //
        public void StartListener()
        {
            if (networkManager.IP == null || networkManager.Port == null) return;

            Task.Run(() => networkManager.StartServer(IPAddress.Parse(networkManager.IP), int.Parse(networkManager.Port)));
            Status = "Listening for connection...";
            StatusColor = Gray;
            RestartServerVisibility = "Hidden";
        }

        public void Send()
        {
            if (messageContent.Length > 0)
            {
                networkManager.SendMessage(MessageContent);
            }
        }

        internal void SendBuzz()
        {
            networkManager.SendBuzz();
        }

        public void AcceptIncoming()
        {
            networkManager.WantConnect = "accept";
        }

        public void DeclineIncoming()
        {
            networkManager.WantConnect = "deny";
        }

        public void DisconnectChat()
        {
            networkManager.Disconnect();
        }
        // -------------------------- //

        // -- Handle events --------- //
        private void OnIsClient(object? sender, EventArgs eventArgs)
        {
            isClient = true;
            Status = "Waiting for response...";
            StatusColor = Blue;
        }

        private void OnBuzzed(object? sender, EventArgs eventArgs)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                userWindow.Left = userWindow.Left + 10.0; // Nice window in the middle
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        userWindow.Left = userWindow.Left - 1.0;
                    }
                    for (int j = 0; j < 20; j++)
                    {
                        userWindow.Left = userWindow.Left + 1.0;
                    }
                }
                userWindow.Left = userWindow.Left - 10.0;
            });
        }

        private void OnPendingClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");
            Status = $"{user} wants to connect";
            StatusColor = Blue;

            PendingVisibility = "Visible";
        }

        private void OnAcceptClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");
            userWindow.Dispatcher.Invoke(() =>
            {
                CurrentConversation = new Conversation(Username, user, new ObservableCollection<Message>(), DateTime.Now);

            });
            Status = $"Chatting with {user}";
            StatusColor = Green;

            PendingVisibility = "Hidden";
            DisconnectVisibility = "Visible";
            Connected = "True";
        }

        private void OnDenyClient(object? sender, string user)
        {
            System.Diagnostics.Debug.WriteLine($"{user}");

            Status = $"Listening for connection...";
            StatusColor = Gray;

            PendingVisibility = "Hidden";
        }

        public void OnMessageReceived(object? sender, Message message)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                currentConversation.Messages.Add(message);
            });
            OnPropertyChanged(nameof(CurrentConversation));
        }

        public void OnMessageSent(object? sender, Message message)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                currentConversation.Messages.Add(message);
            });
            MessageContent = "";
            System.Diagnostics.Debug.WriteLine($"{message.Content}");
            OnPropertyChanged(nameof(CurrentConversation));
        }

        public void OnDisconnected(object? sender, string peer)
        {
            if (Username == null) Username = "";
            if (!isClient)
            {
                History.SaveHistory(currentConversation);
            }
            messageHistory = History.LoadHistory();
            OnPropertyChanged("MessageHistory");

            Status = "Disconnected";
            StatusColor = Red;
            DisconnectVisibility = "Hidden";
            Connected = "False";
            if (!isClient)
            {
                RestartServerVisibility = "Visible";
            }
        }

        private void OnNewEndpoint(object? sender, EventArgs eventArgs)
        {
            IP = networkManager.IP;
            Port = networkManager.Port;
        }

        private void OnCloseClient(object? sender, EventArgs eventArgs)
        {
            userWindow.Dispatcher.Invoke(() =>
            {
                userWindow.Close();
            });
        }

        public void OnClosing(object? sender, EventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine($"lmao");
            if (networkManager.Connected)
            {
                networkManager.Disconnect();
            }
        }
        // -------------------------- //
    }
}
