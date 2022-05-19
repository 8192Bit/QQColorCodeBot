using System;
using System.Linq;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;

namespace QQCCBot {

    enum InfoType {
        info,
        system,
        cc,
        error,
    }

    class Program {
        static void Main(string[] args) {
            Console.WriteLine("\nQQCCBot -- Powered by Mirai.NET \n 8192Bit Intermedia PD \n");
            Print("Loading Marai.Net......",InfoType.system);
            Task t = StartTask();
            Print("Success -- Start message listening.", InfoType.system);
            while(true) {
                if(Console.ReadLine() == "exit") {
                    return;
                }
            }
        }

        static async Task StartTask() {
            MiraiBot bot = new MiraiBot();
            bot.QQ = "2861035582";
            bot.VerifyKey = "114514";
            bot.Address = "localhost:8080";
            await bot.LaunchAsync();

            await MessageManager.SendFriendMessageAsync("2861035582", "<inflex>");

            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(x =>
                {
                    Console.WriteLine($"Got message {x.MessageChain.GetPlainMessage()} from {x.GroupId} sent by {x.Sender.Id}");
                    CCInstProcess(x.MessageChain.GetPlainMessage());
                });

            bot.MessageReceived
                .OfType<FriendMessageReceiver>()
                .Subscribe(x =>
                {
                    Console.WriteLine($"Got message {x.MessageChain.GetPlainMessage()} sent by {x.FriendId} ({x.FriendName})");
                    CCInstProcess(x.MessageChain.GetPlainMessage());
                });

            bot.MessageReceived
                .OfType<StrangerMessageReceiver>()
                .Subscribe(x =>
                {
                    Console.WriteLine($"Got message {x.MessageChain.GetPlainMessage()} sent by {x.StrangerId} ({x.StrangerName})");
                    CCInstProcess(x.MessageChain.GetPlainMessage());
                });
        }

        static void Print(string message, InfoType i) {
            switch(i) {
                case InfoType.info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write('<'+i.ToString()+"> ");

                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case InfoType.cc:
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write('<' + i.ToString() + "> ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case InfoType.system:
                    Console.Write('<' + i.ToString() + "> ");
                    break;
                case InfoType.error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('<' + i.ToString() + "> ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;




            }
            Console.WriteLine(message);

        }

        static void CCInstProcess(string message) {
            if(message == "<inflex>") {
                Print("Service Started.", InfoType.info);
            } else
            if(!message.Trim().StartsWith("!cc")) {
                
                Console.ForegroundColor = ConsoleColor.Red;
                Print("<info> 非法CC代码。", InfoType.cc);
                Console.WriteLine("<info> 非法CC代码。");
                Console.ForegroundColor = ConsoleColor.Gray;
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("<info> 有效CC代码。");
                Console.ForegroundColor = ConsoleColor.Gray;
                string[] s = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if(s[1].ToString() == "Add") {
                    FileStream f = new FileStream(Directory.GetCurrentDirectory()+ @"/CCs/" + s[2]+".txt", FileMode.CreateNew);
                    foreach(char c in s[3]) {
                        f.WriteByte((byte)c);
                    }
                    f.Close();
                } else
                if(s[1].ToString() == "Get") {
                    if(File.Exists(Directory.GetCurrentDirectory() + @"/CCs/" + s[2] + ".txt")){
                        FileStream f = new FileStream(Directory.GetCurrentDirectory() + @"/CCs/" + s[2] + ".txt", FileMode.Open);
                        string ts = null;
                        byte[] bt = { 0x00 };
                        for(int i = 0; i < f.Length; i++) {
                            f.Read(bt, 0, 1);
                            ts += Convert.ToChar(bt[0]);
                        }
                        MessageManager.SendFriendMessageAsync("2861035582", ts.ToString());
                        f.Close();
                    } else {
                        MessageManager.SendFriendMessageAsync("2861035582", "<info> 该用户不存在。");

                    }
                    

                }else
                if(s[1].ToString() == "List") {
                    string ts = null;
                    foreach(string em in Directory.EnumerateFiles(Directory.GetCurrentDirectory() + @"/CCs/")) {
                        ts+=em.Substring(-4, 3);
                    }
                    MessageManager.SendFriendMessageAsync("2861035582", ts);
                } else {
                    MessageManager.SendFriendMessageAsync("2861035582", "<info> 未定义的操作。");
                }

            }
        }
    }
}
// 所有的代码