using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ConnectScreenViewModel : INotifyPropertyChanged
    {

        // -- Fields ----------------- //
        private string username;
        private string ip;
        private string port;
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
        private ICommand startServer;
        private ICommand startClient;

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
        // --------------------------- //

        // -- Properties ----------------------------------------------------------- //
        public string ConnectionIP { get { return ip; } set { ip = value; } }
        public string ConnectionPort { get { return port; } set { port = value; } }
        public string Username { get { return username; } set { username = value; OnPropertyChanged("UsernameExist"); } }
        public string UsernameExist
        {
            get
            {
                if (username == "" || username == null)
                {
                    return "False";
                }
                if (username.Any(c => "<>:\"/\\|?*".Contains(c)))
                {
                    return "False";
                }
                return "True";
            }
        }
        // ------------------------------------------------------------------------- //

        // -- Constructors ----------- //
        public ConnectScreenViewModel()
        {
            this.username = "";
            this.ip = "127.0.0.1";
            this.port = "3000";

            this.startServer = new StartServerCommand(this);
            this.startClient = new StartClientCommand(this);
        }
        // --------------------------- //

        // -- Methods ---------------- //
        public void StartConnection()
        {
            ChatScreen chatscreen = new();

            var server = new NetworkManager(username);
            Task.Run(() => server.StartServer(IPAddress.Parse("127.0.0.1"), int.Parse(port)));
            chatscreen.DataContext = new ChatScreenViewModel(ref server, chatscreen);
            chatscreen.Show();
        }

        internal void FindConnection()
        {
            ChatScreen chatscreen = new();
            var client = new NetworkManager(username);
            Task.Run(() => client.StartClient(ip, int.Parse(port)));
            chatscreen.DataContext = new ChatScreenViewModel(ref client, chatscreen);
            chatscreen.Show();
        }
        // ------------------------- //
    }
}
