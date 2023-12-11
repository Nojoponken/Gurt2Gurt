using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model
{
    internal class ServerManager : NetworkManager
    {

        public override bool StartConnection(string adress, Int32 port)
        {
            IPAddress local_adress = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(local_adress, port);
            server.Start();
            System.Diagnostics.Debug.WriteLine("Starting a connection...");

            while (true)
            {
                if (server.Pending())
                {
                    break;
                }
            }
            using TcpClient client = server.AcceptTcpClient();
            System.Diagnostics.Debug.WriteLine("Connection established!");


            NetworkStream stream = client.GetStream();
            Byte[] bytes = new byte[256];
            String data = null;
            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {

                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                data = data.ToUpper();
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                stream.Write(msg, 0, msg.Length);
                System.Diagnostics.Debug.WriteLine($"Started Connection, message: {data}");
            }
            MessageBox.Show("server off");
            return true;
        }
    }
}
