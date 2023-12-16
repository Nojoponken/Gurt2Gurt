using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    internal class History
    {
        public static ObservableCollection<Conversation> LoadHistory()
        {
            ObservableCollection<Conversation> conversations = new();

            string path = Directory.GetCurrentDirectory() + "/History/";
            DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory() + "/History/");

            Directory.CreateDirectory(path);

            FileInfo[] files = directory.GetFiles("*.json");
            foreach (FileInfo file in files)
            {
                string conversationJSON = File.ReadAllText(path + file.Name);
                Conversation? conversation = JsonSerializer.Deserialize<Conversation>(conversationJSON);
                if (conversation != null)
                {
                    conversations.Add(conversation);
                }
            }

            conversations = new ObservableCollection<Conversation>( conversations.OrderByDescending(c => c.Date));

            return conversations;
        }

        public static void SaveHistory(Conversation conversation)
        {
            string jsonString = JsonSerializer.Serialize<Conversation>(conversation);

            string filename = conversation.Peer1 + "_" + conversation.Peer2 + "_" + conversation.Date.ToString();

            string path = Directory.GetCurrentDirectory() + "/History/";

            string saveName = path + filename;
            saveName = saveName.Replace(' ', '_');
            saveName = saveName.Replace(':', '-');
            
            tryagain:
            if (File.Exists(saveName + ".json"))
            {
                int count = 2;
                while (File.Exists(saveName + "_" + count +".json"))
                {
                    count++;
                }
                saveName = saveName + "_" + count;
            }

            // Avert your eyes
            try { File.WriteAllText(saveName + ".json", jsonString); }
            catch { goto tryagain; }
        }
    }
}
