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
    internal class NetworkManager
    {

        public static bool StartConnection(Int32 port)
        {
            IPAddress adress = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(adress, port);
            server.Start();
            System.Diagnostics.Debug.WriteLine("Starting a connection...");

            using TcpClient client = server.AcceptTcpClient();
            System.Diagnostics.Debug.WriteLine("Connection established!");

            NetworkStream stream = client.GetStream();
            Byte[] bytes = new byte[256];
            String data = null;
            int i;
            while((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                data = data.ToUpper();
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                stream.Write(msg, 0, msg.Length);
                System.Diagnostics.Debug.WriteLine($"Started Connection, message: {data}");
            }

            while (true)
            {
                if(!client.Connected)
                {
                    server.Stop();
                    Window.ContextMenuClosingEvent*()
                    break;
                }
            }
            return true;
        }

        public static bool FindConnection(string adress, Int32 port) {
            String message = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!";
            using TcpClient client = new TcpClient(adress, port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            System.Diagnostics.Debug.WriteLine($"Found Connection, message: {message}");
            
            while (true)
            {
                
            }
            return true;
        }


        public void SendMessage(string str)
        {
             System.Diagnostics.Debug.WriteLine(str + " is sent!");
        }
    }
}
