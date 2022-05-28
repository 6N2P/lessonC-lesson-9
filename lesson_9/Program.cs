using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace lesson_9
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread task = new Thread(MessageHandler);
            task.Start();
        }

        static void MessageHandler()
        {
            string token = File.ReadAllText(@"H:\C#lesson\lesson_9\token.txt");

            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };

            int updeit_id = 0;
            string startUrl = $@"https://api.telegram.org/bot{token}/";

            while(true)
            {
                string url = $"{startUrl}getUpdates?offset={updeit_id}";
                var r = wc.DownloadString(url);

                //Console.WriteLine(r);
                //Console.ReadKey();

                var msgs = JObject.Parse(r)["result"].ToArray();

                foreach(dynamic msg in msgs)
                {
                    updeit_id = Convert.ToInt32(msg.update_id)+1;

                    string userMessage = msg.message.text;
                    string userId = msg.message.from.id;
                    string useFirstName = msg.message.from.first_name;

                    string text = $"{useFirstName} {userId} {userMessage}";

                    Console.WriteLine(text);


                    if (userMessage == "hi")
                    {
                        string responseText = $"Здравствуйте, {useFirstName}";
                        url = $"{startUrl}sendMessage?chat_id={userId}&text={responseText}";
                        Console.WriteLine("+");
                        wc.DownloadString(url);
                    }

                }
                Thread.Sleep(100);
            }
        }
    }
}
