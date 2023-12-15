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
        public static List<ObservableCollection<Message>> LoadHistory()
        {
            List<ObservableCollection<Message>> conversations = new();

            string path = Directory.GetCurrentDirectory() + "/History/";
            DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory() + "/History/");

            Directory.CreateDirectory(path);

            FileInfo[] files = directory.GetFiles("*.json");
            foreach (FileInfo file in files)
            {
                string conversationJSON = File.ReadAllText(path + file.Name);
                ObservableCollection<Message>? conversation = JsonSerializer.Deserialize<ObservableCollection<Message>>(conversationJSON);
                conversations.Add(conversation);
            }

            return conversations;
        }

        public static void SaveHistory(ObservableCollection<Message> conversation, string filename)
        {
            string jsonString = JsonSerializer.Serialize<ObservableCollection<Message>>(conversation);
            string path = Directory.GetCurrentDirectory() + "/History/";

            string saveName = path + filename;
            saveName = saveName.Replace(' ', '_');
            saveName = saveName.Replace(':', '-');
            
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
            tryagain:
            try { File.WriteAllText(saveName + ".json", jsonString); }
            catch { goto tryagain; }
        }
    }
}
