using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;

namespace Lesson_9._2
{
    class Program
    {
        static TelegramBotClient bot;
        static void Main(string[] args)
        {
            string token = File.ReadAllText(@"H:\C#lesson\lesson_9\token.txt");

            bot = new TelegramBotClient(token);
            bot.OnMessage += MessageList;
            bot.StartReceiving();
            Console.ReadKey();
            
        }
        private static void MessageList(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string pathDownLoadFail = @"H:\C#lesson\lesson_9\DownLoad\";
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine(e.Message.Document.FileId);
                Console.WriteLine(e.Message.Document.FileName);
                Console.WriteLine(e.Message.Document.FileSize);

                DownLoad(e.Message.Document.FileId, pathDownLoadFail+ e.Message.Document.FileName);
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                
                Console.WriteLine(e.Message.Photo[e.Message.Photo.Length - 1].FileId);
                Console.WriteLine(e.Message.Photo.Length);
                Console.WriteLine(e.Message.Photo[e.Message.Photo.Length - 1].FileSize);                

                DownLoad(e.Message.Photo[e.Message.Photo.Length - 1].FileId, pathDownLoadFail + e.Message.Photo[e.Message.Photo.Length - 1].FileSize + ".jpg");
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio)
            {
                Console.WriteLine(e.Message.Audio.FileId);
                Console.WriteLine(e.Message.Audio.FileName);
                Console.WriteLine(e.Message.Audio.FileSize);

                DownLoad(e.Message.Audio.FileId, pathDownLoadFail + e.Message.Audio.FileName);
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Video)
            {
                Console.WriteLine(e.Message.Video.FileId);
                Console.WriteLine(e.Message.Video.FileName);
                Console.WriteLine(e.Message.Video.FileSize);

                string path2;

                if (e.Message.Video.FileName != null)
                {
                    path2 = e.Message.Video.FileName;
                }
                else
                {
                    path2 =Convert.ToString( e.Message.Video.FileSize)+".mp4";
                }

                DownLoad(e.Message.Video.FileId, pathDownLoadFail + path2);
            }

            if (e.Message.Text == "/start")
            {
                string messgeText = "Для просмотра файлов нажмите 1\nДля скачивания файлов нажмите2";
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messgeText}");
            }

            if (e.Message.Text == "1")
            {
                string messgeText = ShowContent();
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messgeText}");
            }

            bool flag = true;

            //if (e.Message.Text == "2")
            //{              

            //    string messgeText = ShowContent();
            //    string messgeText2 = "Какой файл вы хотите скачать?\nВыберите строку\nДля выхожа нажмите 'q'";
            //    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messgeText}");
            //    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messgeText2}");
            //    if (e.Message.Text == "q")
            //    {
            //        flag = false;
            //    }
            //    try
            //    {
            //        if (flag)
            //        {
            //            bot.OnMessage += MessageListDownLoad;                        

            //        }
            //        else
            //        {
            //            bot.OnMessage -= MessageListDownLoad;
            //        }
            //    }
            //    catch { }
                
            //}

            if (e.Message.Text == null)
            {

                return;
            }
            else
            {
                var messgeText = $" {e.Message.Text} {e.Message.Chat.FirstName}";
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messgeText}");
            }



        }

        

        private static void MessageListDownLoad(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {            
            string[] masiveFails = MasiveFiles();
            int numberString = Convert.ToInt32(e.Message.Text);
            try
            {

                string nameFail = masiveFails[numberString - 1];

                using (var stream = File.OpenRead(nameFail))
                {
                    InputOnlineFile iof = new InputOnlineFile(stream);
                    iof.FileName = nameFail;
                    string iofType = Path.GetExtension(nameFail);

                   if (iofType == ".jpg")
                    {
                        var sendFont = bot.SendPhotoAsync(e.Message.Chat.Id, iof);
                    }
                    var send = bot.SendDocumentAsync(e.Message.Chat.Id, iof);
                    var send1 = bot.SendTextMessageAsync(e.Message.Chat.Id,iofType );
                }


                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{nameFail}");
            }
            catch
            {

            }
        }

        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream( path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
        }

        static  string ShowContent()
        {
            string content = string.Empty;
            string corectContent = string.Empty;
            string[] masiveFails = MasiveFiles();

            int numberString = 1;
            for (int i = 0; i < masiveFails.Length; i++)
            {
                //удоляю из строики часть где прописан путь к файлам
                corectContent = masiveFails[i].Remove(0, 30);
                //записываю всё в строку
                content = $"{content}{i+numberString} {corectContent}\n";
            }
            numberString = 1;

            return content;
        }

        static string [] MasiveFiles ()
        {
            string[] masiveFails = Directory.GetFiles(@"H:\C#lesson\lesson_9\DownLoad");
            return masiveFails;
        }

       
    }
}
