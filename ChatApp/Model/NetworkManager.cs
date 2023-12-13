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

        private bool pending;
        private string want_connect;
        private TcpListener server;
        private TcpClient client;

        private NetworkStream stream;

        public event EventHandler? IsClient;
        public event EventHandler? CloseClient;
        public event EventHandler? Disconnected;

        public event EventHandler<string>? PendingClient;
        public event EventHandler<string>? AcceptClient;
        public event EventHandler<string>? DenyClient;

        public event EventHandler<Message>? MessageRecieved;
        public event EventHandler<Message>? MessageSent;

        public string WantConnect { set { want_connect = value; } }
        public string Username { get { return username; } }
        public bool Pending { get { return pending; } }

        public NetworkManager(string username)
        {
            this.username = username;
        }

        public bool StartServer(IPAddress address, Int32 port)
        {

            this.server = new TcpListener(address, port);
            server.Start();
            System.Diagnostics.Debug.WriteLine("Starting a connection...");

            want_connect = "waiting";

            using TcpClient client = server.AcceptTcpClient();
            System.Diagnostics.Debug.WriteLine("Connection established!");



            stream = client.GetStream();
            while (true)
            {
                byte[] recieve_buffer = new byte[1024];
                int data = stream.Read(recieve_buffer, 0, 1024);
                if (data != 0)
                {
                    System.Diagnostics.Debug.WriteLine("INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE");

                    string recieved = Encoding.UTF8.GetString(recieve_buffer, 0, data);
                    Message message = JsonSerializer.Deserialize<Message>(recieved);
                    PendingClient?.Invoke(this, message.Author);

                    while (true)
                    {
                        if (want_connect == "accept")
                        {
                            Message response = new("ACCEPT", username, "system");
                            string json_string = JsonSerializer.Serialize(response);
                            byte[] send_buffer = Encoding.UTF8.GetBytes(json_string);
                            stream.Write(send_buffer, 0, json_string.Length);
                            AcceptClient?.Invoke(this, message.Author);
                            HandleChat();
                            break;
                        }
                        if (want_connect == "deny")
                        {
                            Message response = new("DENY", username, "system");
                            string json_string = JsonSerializer.Serialize(response);
                            byte[] send_buffer = Encoding.UTF8.GetBytes(json_string);
                            stream.Write(send_buffer, 0, json_string.Length);
                            DenyClient?.Invoke(this, message.Author);
                            client.Close();
                            server.Stop();
                            StartServer(address, port);
                            break;
                        }
                    }
                    break;
                }
            }
            return true;
        }

        public bool StartClient(string adress, Int32 port)
        {
            IsClient?.Invoke(this, EventArgs.Empty);
            try
            {
                client = new TcpClient(adress, port);
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
            string json_string = JsonSerializer.Serialize(message);

            byte[] send_buffer = Encoding.UTF8.GetBytes(json_string);
            stream.Write(send_buffer, 0, json_string.Length);

            while (true)
            {
                byte[] recieve_buffer = new byte[1024];
                int data = stream.Read(recieve_buffer, 0, 1024);
                if (data != 0)
                {
                    string response = Encoding.UTF8.GetString(recieve_buffer, 0, data);
                    System.Diagnostics.Debug.WriteLine($"Found Connection, message: {data}");

                    Message response_message = JsonSerializer.Deserialize<Message>(response);

                    if (response_message.Type == "system")
                    {
                        if (response_message.Content == "ACCEPT")
                        {
                            AcceptClient?.Invoke(this, response_message.Author);
                            HandleChat();
                            return true;
                        }
                        else if (response_message.Content == "DENY")
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
                byte[] recieve_buffer = new byte[1024];
                int data = stream.Read(recieve_buffer, 0, 1024);
                if (data != 0)
                {
                    string message_string = Encoding.UTF8.GetString(recieve_buffer, 0, data);

                    Message message = JsonSerializer.Deserialize<Message>(message_string);
                    System.Diagnostics.Debug.WriteLine($"New message: {message.Content}");

                    if (message.Type == "user")
                    {
                        MessageRecieved?.Invoke(this, message);
                    }
                    else if (message.Type == "system" && message.Content == "DISCONNECT")
                    {
                        client.Close();
                        server.Stop();
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
            string json_string = JsonSerializer.Serialize(message);

            byte[] send_buffer = Encoding.UTF8.GetBytes(json_string);
            if (stream != null)
            {
                stream.Write(send_buffer, 0, json_string.Length);
                MessageSent?.Invoke(this, message);
            }

            return true;
        }

        public bool Disconnect() 
        {
            Message message = new("DISCONNECT", username, "system");
            string json_string = JsonSerializer.Serialize(message);

            byte[] send_buffer = Encoding.UTF8.GetBytes(json_string);
            if (stream != null)
            {
                stream.Write(send_buffer, 0, json_string.Length);
                MessageSent?.Invoke(this, message);
            }

            client.Close();
            server.Stop();
            Disconnected.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
