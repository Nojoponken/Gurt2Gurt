using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public abstract class NetworkManager
    {
        public abstract bool StartConnection(string adress,Int32 port);

        public void SendMessage(string str)
        {
            System.Diagnostics.Debug.WriteLine(str + " is sent!");
        }
    }
}
