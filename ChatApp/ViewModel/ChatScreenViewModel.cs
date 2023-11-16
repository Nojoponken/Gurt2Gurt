using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    class ChatScreenViewModel
    {
        private NetworkManager _networkManager;
           
        public ChatScreenViewModel(NetworkManager networkManager) 
        { 
            this._networkManager = networkManager; 
        }
    }
}
