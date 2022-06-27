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
            string token = File.ReadAllText(@"C:\Users\Ivanovsv\Desktop\Lessons\Lesson_9\token.txt");

            bot = new TelegramBotClient(token);
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
            
        }
        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            if (e.Message.Text == "/start")
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id,
               $"Для просмотра папки с файлами отправте сообщение show-f, для скачивания файла d-f-'номер файла'  ");
            }

            //сохранение документа
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine(e.Message.Document.FileId);
                Console.WriteLine(e.Message.Document.FileName);
                Console.WriteLine(e.Message.Document.FileSize);

                DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
                return;
            }

            //сохранение аудио
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio)
            {
                Console.WriteLine(e.Message.Audio.FileId);
                Console.WriteLine(e.Message.Audio.FileName);
                Console.WriteLine(e.Message.Audio.FileSize);

                DownLoad(e.Message.Audio.FileId, e.Message.Audio.FileName);

                return;
            }

            //сохранение картинки
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                Console.WriteLine(e.Message.Photo[3].FileId);

                Console.WriteLine(e.Message.Photo[3].FileSize);

                DownLoad(e.Message.Photo[3].FileId, $"{e.Message.Photo[3].FileSize}.jpg");
                return;
            }

            if (e.Message.Text == "show-f")
            {
                string listFile = ShowContent();
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{listFile}");

                return;
            }


            if (e.Message.Text.StartsWith("d-f-"))
            {
                //получаю массив названий файлов в папке
                string[] masiveFails = MasiveFiles();
                //получаю номер строки из массива
                int numberString = Convert.ToInt32(e.Message.Text.Substring(4));

                try
                {
                    string nameFail = masiveFails[numberString - 1];

                    using (var strim = File.OpenRead(nameFail))
                    {
                        InputOnlineFile iof = new InputOnlineFile(strim);
                        iof.FileName = nameFail;
                        string iofType = Path.GetExtension(nameFail);

                        if (iofType == ".jpg")
                        {
                            var sendPhote = bot.SendPhotoAsync(e.Message.Chat.Id, iof);
                        }

                        bot.SendDocumentAsync(e.Message.Chat.Id, iof);

                        bot.SendTextMessageAsync(e.Message.Chat.Id, "отправлен файл");


                    }

                }
                catch { }
                return;
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

        /// <summary>
        /// загружает файл
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        static async void DownLoad(string fileId, string path)
        {
            try
            {
                var file = await bot.GetFileAsync(fileId);
                FileStream fs = new FileStream(@"C:\Users\Ivanovsv\Desktop\Lessons\Lesson_9\DownLoad\" + path, FileMode.Create);
                await bot.DownloadFileAsync(file.FilePath, fs);

                fs.Close();
                fs.Dispose();
            }
            catch { }
        }
        /// <summary>
        /// показывает содержимое закаченной папки
        /// </summary>
        /// <returns></returns>
        static string ShowContent()
        {
            string result = string.Empty;

            string[] listFile = MasiveFiles();

            int counter = 1;
            foreach (var v in listFile)
            {
                string text = v.Remove(0, 52);
                result += $"{counter} {text} \n";

                counter++;
            }

            return result;
        }

        static string[] MasiveFiles()
        {
            string directoryName = @"C:\Users\Ivanovsv\Desktop\Lessons\Lesson_9\DownLoad\";
            string[] masiveFails = Directory.GetFiles(directoryName);
            return masiveFails;
        }


    }
}
