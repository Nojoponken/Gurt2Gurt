using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    internal class History
    {
        private readonly Message[] messages;

        public Message[] Messages { get { return messages; } }

        public History()
        {
            this.messages = Array.Empty<Message>();
        }

        public void AddMessage(Message messageToAdd)
        {
            _ = messages.Append(messageToAdd);
        }
    }
}
