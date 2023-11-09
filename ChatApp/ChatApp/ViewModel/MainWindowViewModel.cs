using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.ViewModel.Command;
using ChatApp.Model;
using ChatApp.View;

namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel
    {

        private NetworkManager NetworkManager { get; set; }
        private ICommand startGame;
        private string text;
        
        public MainWindowViewModel(NetworkManager networkManager)
        {
            NetworkManager = networkManager;
        }
        
        public string MyText { 
            get {

                return text; } 
            set 
            { 
                text = value;
            
            } 
        }


        public ICommand StartGame
        {
            get
            {
                if (startGame == null)
                    startGame = new StartGameCommand(this);
                return startGame;
            }
            set
            {
                startGame = value;
            }
        }

        private void startConnection()
        {
            NetworkManager.startConnection();
        }

        public void startGameBoard()
        {
            ConnectScreen board = new ConnectScreen();
            board.DataContext = this;
            board.ShowDialog();
            startConnection();
        }

        private ICommand enterCommand;
        public ICommand EnterCommand
        {
            get {
                if (enterCommand == null)
                {
                    return new KeyEnterCommand(this);
                }
                else {
                    return enterCommand;

                }
            }
            set { enterCommand = value; }
        }

        public void sendMessage()
        {
            NetworkManager.sendMessage(MyText);
        }

        public void showGameBoard()
        {
            if (NetworkManager.startConnection())
            {
                ConnectScreen connectScreen = new ConnectScreen();
                connectScreen.DataContext = this;
                connectScreen.ShowDialog();
            }
   
        }
    }
}
