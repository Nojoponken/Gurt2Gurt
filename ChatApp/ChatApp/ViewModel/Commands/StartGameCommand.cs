using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.ViewModel;

namespace ChatApp.ViewModel.Command
{
    internal class StartGameCommand : ICommand
    {
        private MainWindowViewModel parent = null;


        public StartGameCommand(MainWindowViewModel parent)
        {
            this.parent = parent;

        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

            parent.showGameBoard();
        }
    }
}
