using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Text.Json;

namespace ChatApp.Model
{
    class NetworkManager
    {
        // -- Fields ----------------- //
        private string username;
        private string? ip;
        private string? port;
        private string? peer;

        private bool pending;
        private bool connected;
        private string wantConnect;
        private TcpListener? server;
        private TcpClient? client;

        private NetworkStream? stream;
        // --------------------------- //

        // -- Constructors ----------- //
        public event EventHandler? IsClient;
        public event EventHandler<string>? CloseClient;
        public event EventHandler<string>? Disconnected;
        public event EventHandler? Buzzed;
        public event EventHandler? NewEndpoint;

        public event EventHandler<string>? PendingClient;
        public event EventHandler<string>? AcceptClient;
        public event EventHandler<string>? DenyClient;

        public event EventHandler<Message>? MessageReceived;
        public event EventHandler<Message>? MessageSent;
        // --------------------------- //

        // -- Properties --------------------------------------- //
        public bool Connected { get { return connected; } }
        public string WantConnect { set { wantConnect = value; } }
        public string Username { get { return username; } }
        public string? IP { get { return ip; } }
        public string? Port { get { return port; } }
        public bool Pending { get { return pending; } }
        // ----------------------------------------------------- //

        // -- Constructors ----------- //
        public NetworkManager(string username)
        {
            this.username = username;
            this.pending = false;
            this.wantConnect = "waiting";
        }
        // --------------------------- //

        // -- Methods ---------------- //
        public bool StartServer(IPAddress address, int port)
        {
            this.port = port.ToString();
            this.ip = address.ToString();
            NewEndpoint?.Invoke(this, EventArgs.Empty);

            server = new TcpListener(address, port);
            server.Start();

            wantConnect = "waiting";

            client = server.AcceptTcpClient();
            stream = client.GetStream();
            while (true)
            {
                byte[] receiveBuffer = new byte[1024];
                int data = stream.Read(receiveBuffer, 0, 1024);
                if (data == 0) continue;

                string received = Encoding.UTF8.GetString(receiveBuffer, 0, data);
                Message? message = JsonSerializer.Deserialize<Message>(received);

                if (message == null) continue;
                PendingClient?.Invoke(this, message.Author);

                while (true)
                {
                    if (wantConnect == "accept")
                    {
                        Message response = new("ACCEPT", username, "system", DateTime.Now);
                        string jsonString = JsonSerializer.Serialize(response);
                        byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
                        stream.Write(sendBuffer, 0, jsonString.Length);
                        AcceptClient?.Invoke(this, message.Author);
                        peer = message.Author;
                        connected = true;
                        HandleChat();
                        break;
                    }
                    if (wantConnect == "deny")
                    {
                        Message response = new("DENY", username, "system", DateTime.Now);
                        string jsonString = JsonSerializer.Serialize(response);
                        byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
                        stream.Write(sendBuffer, 0, jsonString.Length);
                        DenyClient?.Invoke(this, message.Author);
                        StartServer(address, port);
                        Cleanup();
                        break;
                    }
                }
                break;
            }
            return true;
        }

        public bool StartClient(string address, int port)
        {

            this.port = port.ToString();
            this.ip = address;
            NewEndpoint?.Invoke(this, EventArgs.Empty);


            IsClient?.Invoke(this, EventArgs.Empty);
            try
            {
                client = new TcpClient(address, port);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                CloseClient?.Invoke(this, "No server found, try connecting to another port");
                return false;
            }
            // Make stream reader/writer
            stream = client.GetStream();

            Message message = new("REQUEST", username, "system", DateTime.Now);
            string jsonString = JsonSerializer.Serialize(message);

            byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
            stream.Write(sendBuffer, 0, jsonString.Length);

            while (true)
            {
                byte[] receiveBuffer = new byte[1024];
                int data;
                try
                {
                    data = stream.Read(receiveBuffer, 0, 1024);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                    client.Close();
                    CloseClient?.Invoke(this, "No response");
                    return false;
                }
                if (data != 0)
                {
                    string response = Encoding.UTF8.GetString(receiveBuffer, 0, data);

                    Message? responseMessage = JsonSerializer.Deserialize<Message>(response);

                    if (responseMessage == null) continue;
                    if (responseMessage.Type == "system")
                    {
                        if (responseMessage.Content == "ACCEPT")
                        {
                            AcceptClient?.Invoke(this, responseMessage.Author);
                            peer = responseMessage.Author;
                            connected = true;
                            HandleChat();
                            return true;
                        }
                        else if (responseMessage.Content == "DENY")
                        {
                            client.Close();
                            CloseClient?.Invoke(this, "Connection request was denied.");
                            return true;
                        }
                    }
                }
            }
        }

        public bool HandleChat()
        {
            while (true)
            {
                byte[] receiveBuffer = new byte[1024];

                int data;

                if (!stream.CanRead)
                {
                    Cleanup();
                    return false;
                }

                data = stream.Read(receiveBuffer, 0, 1024);

                if (data != 0)
                {
                    string messageString = Encoding.UTF8.GetString(receiveBuffer, 0, data);

                    Message? message = JsonSerializer.Deserialize<Message>(messageString);

                    if (message?.Type == "user")
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                    else if (message is { Type: "system", Content: "DISCONNECT" })
                    {
                        Cleanup();
                        if (peer == null) peer = "";
                        Disconnected?.Invoke(this, peer);
                        break;
                    }
                    else if (message is { Type: "system", Content: "BUZZ" })
                    {
                        Buzzed?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            return true;
        }

        public bool SendMessage(string content)
        {
            Message message = new(content, username, "user", DateTime.Now);
            string jsonString = JsonSerializer.Serialize(message);

            byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
            if (stream != null)
            {
                stream.Write(sendBuffer, 0, jsonString.Length);
                MessageSent?.Invoke(this, message);
            }

            return true;
        }

        public bool SendBuzz()
        {
            Message message = new("BUZZ", username, "system", DateTime.Now);
            string jsonString = JsonSerializer.Serialize(message);

            byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
            if (stream != null)
            {
                stream.Write(sendBuffer, 0, jsonString.Length);
            }

            return true;
        }

        public bool Disconnect()
        {
            Message message = new("DISCONNECT", username, "system", DateTime.Now);
            string jsonString = JsonSerializer.Serialize(message);

            byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
            stream?.Write(sendBuffer, 0, jsonString.Length);

            Cleanup();
            if (peer == null) peer = "";
            Disconnected?.Invoke(this, peer);

            return true;
        }

        private void Cleanup()
        {
            connected = false;
            client?.Close();
            server?.Stop();
            stream = null;
        }
        // --------------------------- //
    }
}
