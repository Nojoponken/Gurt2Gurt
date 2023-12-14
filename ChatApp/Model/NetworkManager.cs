using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Text.Json;
using System.IO;
using System.Reflection.Metadata;

namespace ChatApp.Model
{
    class NetworkManager
    {
        private string username;
        private string ip;
        private string port;

        private bool pending;
        private string wantConnect;
        private TcpListener? server;
        private TcpClient? client;

        private NetworkStream stream;

        public event EventHandler? IsClient;
        public event EventHandler? CloseClient;
        public event EventHandler? Disconnected;
        public event EventHandler? NewEndpoint;

        public event EventHandler<string>? PendingClient;
        public event EventHandler<string>? AcceptClient;
        public event EventHandler<string>? DenyClient;

        public event EventHandler<Message>? MessageReceived;
        public event EventHandler<Message>? MessageSent;

        public string WantConnect { set { wantConnect = value; } }
        public string Username { get { return username; } }
        public string IP
        {
            get { return ip; }
        }
        
        public string Port
        {
            get { return port; }
        }

        public bool Pending
        {
            get { return pending; }
        }

        public NetworkManager(string username)
        {
            this.username = username;
            this.pending = false;
            this.wantConnect = "waiting";
        }

        public bool StartServer(IPAddress address, int port)
        {
            this.port = port.ToString();
            this.ip = address.ToString();
            NewEndpoint?.Invoke(this, EventArgs.Empty);
            
            server = new TcpListener(address, port);
            server.Start();
            System.Diagnostics.Debug.WriteLine("Starting a connection...");

            wantConnect = "waiting";

            client = server.AcceptTcpClient();
            System.Diagnostics.Debug.WriteLine("Connection established!");



            stream = client.GetStream();
            while (true)
            {
                byte[] receiveBuffer = new byte[1024];
                int data = stream.Read(receiveBuffer, 0, 1024);
                if (data == 0) continue;

                string received = Encoding.UTF8.GetString(receiveBuffer, 0, data);
                Message? message = JsonSerializer.Deserialize<Message>(received);
                PendingClient?.Invoke(this, message.Author);

                while (true)
                {
                    if (wantConnect == "accept")
                    {
                        Message response = new("ACCEPT", username, "system");
                        string jsonString = JsonSerializer.Serialize(response);
                        byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
                        stream.Write(sendBuffer, 0, jsonString.Length);
                        AcceptClient?.Invoke(this, message.Author);
                        HandleChat();
                        break;
                    }
                    if (wantConnect == "deny")
                    {
                        Message response = new("DENY", username, "system");
                        string jsonString = JsonSerializer.Serialize(response);
                        byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
                        stream.Write(sendBuffer, 0, jsonString.Length);
                        DenyClient?.Invoke(this, message.Author);
                        client.Close();
                        server.Stop();
                        StartServer(address, port);
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
                CloseClient?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("No server found, try connecting to another port");
                return false;
            }
            // Make stream reader/writer
            stream = client.GetStream();

            Message message = new("REQUEST", username, "system");
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
                    client.Close();
                    CloseClient?.Invoke(this, EventArgs.Empty);
                    return false;
                }
                if (data != 0)
                {
                    string response = Encoding.UTF8.GetString(receiveBuffer, 0, data);
                    System.Diagnostics.Debug.WriteLine($"Found Connection, message: {data}");

                    Message responseMessage = JsonSerializer.Deserialize<Message>(response);

                    if (responseMessage.Type == "system")
                    {
                        if (responseMessage.Content == "ACCEPT")
                        {
                            AcceptClient?.Invoke(this, responseMessage.Author);
                            HandleChat();
                            return true;
                        }
                        else if (responseMessage.Content == "DENY")
                        {
                            client.Close();
                            CloseClient?.Invoke(this, EventArgs.Empty);
                            MessageBox.Show("Connection request was denied.");
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

                try
                {
                    data = stream.Read(receiveBuffer, 0, 1024);
                }
                catch (Exception ex)
                {
                    client?.Close();
                    server?.Stop();
                    Disconnected.Invoke(this, EventArgs.Empty);
                    return false;
                }
                
                if (data != 0)
                {
                    string messageString = Encoding.UTF8.GetString(receiveBuffer, 0, data);

                    Message message = JsonSerializer.Deserialize<Message>(messageString);
                    System.Diagnostics.Debug.WriteLine($"New message: {message.Content}");

                    if (message.Type == "user")
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                    else if (message is { Type: "system", Content: "DISCONNECT" })
                    {
                        client?.Close();
                        server?.Stop();
                        Disconnected.Invoke(this, EventArgs.Empty);
                        break;
                    }
                }
            }

            return true;
        }

        public bool SendMessage(string content)
        {
            Message message = new(content, username, "user");
            string jsonString = JsonSerializer.Serialize(message);

            byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
            if (stream != null)
            {
                stream.Write(sendBuffer, 0, jsonString.Length);
                MessageSent?.Invoke(this, message);
            }

            return true;
        }

        public bool Disconnect() 
        {
            Message message = new("DISCONNECT", username, "system");
            string jsonString = JsonSerializer.Serialize(message);

            byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonString);
            try
            { 
                stream.Write(sendBuffer, 0, jsonString.Length);
            }
            catch (Exception ex) { 
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex}");
            }

            client?.Close();
            server?.Stop();
            Disconnected.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
