using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ConnectScreenViewModel
    {
        private ICommand startServer;
        private ICommand startClient;

        private string connectionIP;
        private string connectionPort;
        private string username;


        public ICommand StartServer
        {
            get
            {
                startServer ??= new StartServerCommand(this);
                return startServer;
            }
            set { startServer = value; }
        }

        public ICommand StartClient
        {
            get
            {
                startClient ??= new StartClientCommand(this);
                return startClient;
            }
            set { startClient = value; }
        }

        // Basic getters and setters
        public string ConnectionIP { get { return connectionIP; } set { connectionIP = value; } }
        public string ConnectionPort { get { return connectionPort; } set { connectionPort = value; } }
        public string Username { get { return username; } set { username = value; } }

        public ConnectScreenViewModel()
        {
            this.connectionIP = "127.0.0.1";
            this.connectionPort = "3000";

            this.startServer = new StartServerCommand(this);
            this.startClient = new StartClientCommand(this);
        }

        public void StartConnection()
        {
            ChatScreen chatscreen = new();

            var server = new NetworkManager(username);
            Task.Run(() => server.StartServer(IPAddress.Parse("127.0.0.1"), int.Parse(connectionPort)));
            chatscreen.DataContext = new ChatScreenViewModel(ref server, chatscreen);
            chatscreen.Show();
        }

        internal void FindConnection()
        {
            ChatScreen chatscreen = new();
            var client = new NetworkManager(username);
            Task.Run(() => client.StartClient(connectionIP, int.Parse(connectionPort)));
            chatscreen.DataContext = new ChatScreenViewModel(ref client, chatscreen);
            chatscreen.Show();
        }
    }
}
