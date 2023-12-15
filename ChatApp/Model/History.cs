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
                conversations.Add(conversation);
            }

            return conversations;
        }

        public static void SaveHistory(ObservableCollection<Message> messages, string peer1, string peer2)
        {
            DateTime dateTime = DateTime.Now;

            Conversation conversation = new(peer1, peer2, messages, dateTime);
            string jsonString = JsonSerializer.Serialize<Conversation>(conversation);

            string filename = peer1 + "_" + peer2 + "_" + dateTime.ToString();

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
