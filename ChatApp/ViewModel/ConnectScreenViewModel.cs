using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ConnectScreenViewModel
    {
        private NetworkManager networkManager;

        private ICommand startServer;
        private ICommand startClient;

        private string connectionIP;
        private string connectionPort;
        private string listenPort;


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

        public string ConnectionIP
        {
            get { return connectionIP; }
            set { connectionIP = value; }
        }

        public string ConnectionPort
        {
            get { return connectionPort; }
            set { connectionPort = value; }
        }
        public string ListenPort
        {
            get { return listenPort; }
            set { listenPort = value; }
        }

        public ConnectScreenViewModel(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.listenPort = string.Empty;
            this.connectionIP = string.Empty;
            this.connectionPort = string.Empty;

            this.startServer = new StartServerCommand(this);
            this.startClient = new StartClientCommand(this);
        }

        public void StartConnection()
        {
            Task.Run(() => NetworkManager.StartConnection(Int32.Parse(listenPort)));
            ChatScreen chatscreen = new();
            chatscreen.Show();
        }

        internal void FindConnection()
        {
            Task.Run(() => NetworkManager.FindConnection(connectionIP, Int32.Parse(connectionPort)));
            ChatScreen chatscreen = new();
            chatscreen.Show();
        }
    }
}
