using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChatApp.Model
{
    internal class Message
    {
        private string content;
        private string author;
        private string type;

        public string Content { get { return content; } set { content = value; } }
        public string Author { get { return author; } set { author = value; } }
        public string Type { get { return type; } set { type = value; } }

        public Message(string content, string author, string type)
        {
            this.content = content;
            this.author = author;
            this.type = type;
        }
    }
}
