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
        //public static List<ObservableCollection<Message>>? sLoadHistory()
        //{
        //    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    string fileName = "myhistory.json";
        //    // Reads from myhistory.json to messages
        //    if (File.Exists(filePath))
        //    {
        //        string json = File.ReadAllText(Path.Combine(filePath, fileName));
        //        List<ObservableCollection<Message>> loadedMessages = null;
        //        try
        //        {
        //            loadedMessages = JsonSerializer.Deserialize<List<ObservableCollection<Message>>>(json);
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
        //        }
        //        if (loadedMessages != null)
        //        {
        //            return loadedMessages.ToList();

        //        }
        //        return null;
        //    }
        //    else
        //    {
        //        File.Create(Path.Combine(filePath, fileName));
        //        return null;
        //    }
        //}

        public static List<ObservableCollection<Message>> LoadHistory()
        {
            List<ObservableCollection<Message>> conversations= new();

            // Load all conversations
            string path = Directory.GetCurrentDirectory() + @"\Conversations\";
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Conversations\");
            
            File.Create(path);
            
            FileInfo[] files = dir.GetFiles("*.JSON");
            foreach (FileInfo file in files)
            {
                string JSON = File.ReadAllText(path + file.Name);
                ObservableCollection<Message> c = JsonSerializer.Deserialize<ObservableCollection<Message>>(JSON);
                conversations.Add(c);
            }

            // return
            return conversations;
        }

        public static void SaveHistory(ObservableCollection<Message> conversation, string filename)
        {
            DateTime time = DateTime.Now;
            string dt = time.ToString("s", DateTimeFormatInfo.InvariantInfo);

            string jsonString = JsonSerializer.Serialize<ObservableCollection<Message>>(conversation);
            string path = Directory.GetCurrentDirectory() + @"\Conversations\";

            for (int count = 0; File.Exists(filename); count++)
            {
                filename = filename + count;
            }

            File.WriteAllText(path + filename + ".json", jsonString);
            //conversation.Messages.Clear(); - handle thread
        }


        //public static void sSaveHistory(List<ObservableCollection<Message>> conversations)
        //{
        //    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    string fileName= "myhistory.json";
        //    // Saves messages to myhistory.json
        //    string json = JsonSerializer.Serialize(conversations);
        //    try
        //    {
        //        File.WriteAllText(Path.Combine(filePath, fileName), json);
        //    }
        //    catch
        //    {
        //        SaveHistory(conversations);
        //    }
        //}
    }
}
