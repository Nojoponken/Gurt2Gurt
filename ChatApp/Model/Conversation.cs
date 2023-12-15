using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    internal class Conversation
    {
        string peer1;
        string peer2;
        ObservableCollection<Message> messages;
        DateTime date;
        public Conversation(string peer1, string peer2, ObservableCollection<Message> messages, DateTime date)
        {
            this.peer1 = peer1;
            this.peer2 = peer2;
            this.messages = messages;
            this.date = date;

        }

        public string Peer1 { get { return peer1; } }
        public string Peer2 { get { return peer2; } }
        public ObservableCollection<Message> Messages { get { return messages; } }
        public DateTime Date { get { return date; } }

    }
}
