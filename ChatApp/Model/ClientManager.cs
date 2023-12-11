using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model
{
    internal class ClientManager : NetworkManager
    {

        public override bool StartConnection(string adress, Int32 port)
        {
            String message = "in your walls ";
            using TcpClient client = new TcpClient(adress, port);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);


            System.Diagnostics.Debug.WriteLine($"Found Connection, message: {message}");

            MessageBox.Show("client off");
            return true;
        }

    }
}
