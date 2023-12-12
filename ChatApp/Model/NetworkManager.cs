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


            NetworkStream stream = client.GetStream();
            StreamReader reader = new(stream);
            StreamWriter writer = new(stream);

            while (true)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    Message message = JsonSerializer.Deserialize<Message>(data);
                    while (true)
                    {
                        if (want_connect == "accept")
                        {
                            Message response = new("ACCEPT", "John", "system");
                            string json_string = JsonSerializer.Serialize(response);
                            writer.Write(json_string);
                            break;
                        }
                        if (want_connect == "deny")
                        {
                            Message response = new("DENY", "John", "system");
                            string json_string = JsonSerializer.Serialize(response);
                            writer.Write(json_string);

                            reader.Close();
                            writer.Close();
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
            NetworkStream stream = client.GetStream();
            StreamReader reader = new(stream);
            StreamWriter writer = new(stream);

            Message message = new("REQUEST", "John", "system");
            string json_string = JsonSerializer.Serialize(message);

            writer.WriteLine(json_string);

            System.Diagnostics.Debug.WriteLine($"Found Connection, message: {message}");
            while (true)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    Message response_message = JsonSerializer.Deserialize<Message>(data);
                    if (response_message.Type == "system")
                    {
                        if (response_message.Content == "ACCEPT")
                        {
                            MessageBox.Show("success");
                        }
                        else if (response_message.Content == "DENY")
                        {
                            reader.Close();
                            writer.Close();
                            client.Close();
                        }
                    }
                }
            }
            MessageBox.Show("client off");
            return true;
        }
    }
}
