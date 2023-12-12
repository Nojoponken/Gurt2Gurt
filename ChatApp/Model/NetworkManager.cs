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

namespace ChatApp.Model
{
    class NetworkManager
    {

        private IPAddress address;
        private Int32 port;

        private bool pending;
        private string want_connect;
        private TcpListener server;
        private NetworkStream stream;

        public event EventHandler<string>? PendingClient;

        public string WantConnect { set { want_connect = value; } }
        public bool Pending { get { return pending; } }

        public NetworkManager(IPAddress address, Int32 port)
        {
            this.address = address;
            this.port = port;
        }

        public bool StartServer()
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
                if (data != null)
                {
                    System.Diagnostics.Debug.WriteLine("INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE INVOKE");

                    string recieved = Encoding.UTF8.GetString(recieve_buffer, 0, data);
                    Message message = JsonSerializer.Deserialize<Message>(recieved);
                    PendingClient?.Invoke(this, message.Author);

                    while (true)
                    {
                        if (want_connect == "accept")
                        {
                            Message response = new("ACCEPT", "John", "system");
                            string json_string = JsonSerializer.Serialize(response);
                            // TODO : grabb grej du vet
                            break;
                        }
                        if (want_connect == "deny")
                        {
                            Message response = new("DENY", "John", "system");
                            string json_string = JsonSerializer.Serialize(response);
                            /// TODO: send respsodfa
                            client.Close();
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
            using TcpClient client = new(adress, port);

            // Make stream reader/writer
            stream = client.GetStream();
            
            Message message = new("REQUEST", "John", "system");
            string json_string = JsonSerializer.Serialize(message);


            byte[] send_buffer = Encoding.UTF8.GetBytes(json_string);
            stream.Write(send_buffer, 0, json_string.Length);



            while (true)
            {
                byte[] recieve_buffer = new byte[1024];
                int data = stream.Read(recieve_buffer, 0, 1024);
                if (data != null)
                {
                    string response = Encoding.UTF8.GetString(recieve_buffer, 0, data);
                    Message response_message = JsonSerializer.Deserialize<Message>(response);

                    System.Diagnostics.Debug.WriteLine($"Found Connection, message: {response}");
                    if (response_message.Type == "system")
                    {
                        if (response_message.Content == "ACCEPT")
                        {
                            MessageBox.Show("success");
                            return true;
                        }
                        else if (response_message.Content == "DENY")
                        {
                            client.Close();
                            MessageBox.Show("client off");
                            return true;
                        }
                    }
                }
            }
        }
    }
}
