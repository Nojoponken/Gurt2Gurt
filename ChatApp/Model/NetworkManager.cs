using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    internal class NetworkManager
    {

        public static bool StartConnection()
        {
            System.Diagnostics.Debug.WriteLine("Starting a connection...");
            System.Diagnostics.Debug.WriteLine("Connection established!");
            return true;
        }


        public void SendMessage(string str)
        {
            System.Diagnostics.Debug.WriteLine(str + " is sent!");
        }
    }
}
