using ChatApp.Model;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ConnectScreenViewModel
    {
        private NetworkManager _networkManager;

        private ICommand _startServer;

        private string _connectionIP;
        private string _connectionPort;
        private string _listenPort;


        public ICommand StartServer
        {
            get
            {
                _startServer ??= new StartServerCommand(this);
                return _startServer;
            }
            set
            {
                _startServer = value;
            }
        }

        public string ConnectionIP { 
            get { return _connectionIP; } 
            set { _connectionIP = value; } 
        }

        public string ConnectionPort
        {
            get { return _connectionPort; }
            set { _connectionPort = value; }
        }
        public string ListenPort
        {
            get { return _listenPort; }
            set { _listenPort = value; }
        }

        public ConnectScreenViewModel(NetworkManager networkManager) {
            this._networkManager = networkManager;
            this._listenPort = string.Empty;
            this._connectionIP = string.Empty;
            this._connectionPort = string.Empty;
        }

        public void startConnection()
        {
            _networkManager.startConnection();
        }

    }
}
