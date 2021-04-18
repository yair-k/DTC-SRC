using System.Diagnostics;
using System.Drawing;
using System.Threading;
using DTC_AIO;
using Colorful;

namespace DTCCUI.Tools
{
    public class SettingsTools
    {
        public static void Information0()
        {
            Console.Title =
                "                                                                                                      [>] DTCAIO | Information | vragon#0001 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();

            Console.Write("    [", Color.White);
            Console.Write("INFO", Color.Purple);
            Console.Write("] Created By vragon#0001\n", Color.White);

            Console.Write("    [", Color.White);
            Console.Write("INFO", Color.Purple);
            Console.Write("] If you bought this without src you got RATed\n", Color.White);

            Console.Write("    [", Color.White);
            Console.Write("INFO", Color.Purple);
            Console.Write("] Support the creator with BTC: bc1qf9hpraptu9mfmtuf77zy9h2rk43vs2ky66fhz4\n", Color.White);

            Console.Write("    [", Color.White);
            Console.Write("X", Color.Purple);
            Console.Write("] Go Back\n", Color.White);
            Console.Write("\n    [", Color.White);
            Console.Write(">", Color.Purple);
            Console.Write("] ", Color.White);
            var Read = System.Console.ReadLine().ToUpper();
            switch (Read)
            {
                case "X":
                {
                    Program.Menu0();
                    break;
                }
                default:
                    Program.prefix("Invalid Option", "");
                    Thread.Sleep(300);
                    Information0();
                    break;
            }
        }

        public static void Settings0()
        {
            Console.Title =
                "                                                                                                      [>] DTCAIO | Settings | vragon#0001 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            Console.Write("    [", Color.White);
            Console.Write("1", Color.Purple);
            Console.Write("] Send to Discord Webhook\n", Color.White);
            Console.Write("    [", Color.White);
            Console.Write("2", Color.Purple);
            Console.Write("] Discord Bot Settings\n", Color.White);
            Console.Write("    [", Color.White);
            Console.Write("X", Color.Purple);
            Console.Write("] Go Back\n", Color.White);
            Console.Write("\n    [", Color.White);
            Console.Write(">", Color.Purple);
            Console.Write("] ", Color.White);
            var Read = System.Console.ReadLine().ToUpper();
            switch (Read)
            {
                case "1":
                {
                    System.Console.Clear();
                    System.Console.WriteLine();
                    Program.Ascii();
                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    Console.Write("    [", Color.White);
                    Console.Write("Enter Webhook", Color.Purple);
                    Console.Write("]\n", Color.White);
                    Console.Write("    [", Color.White);
                    Console.Write("X", Color.Purple);
                    Console.Write("] Go Back\n", Color.White);
                    Console.Write("\n    [", Color.White);
                    Console.Write(">", Color.Purple);
                    Console.Write("] ", Color.White);
                    var webhook = System.Console.ReadLine();
                    if (webhook != "X")
                    {
                        Settings.webHook = webhook;
                        Settings.sendToWebhook = true;
                        Thread.Sleep(250);
                        Console.Write("    [", Color.White);
                        Console.Write("Success Added webhook!\n Redirecting to Settings...", Color.Purple);
                        Console.Write("]\n", Color.White);
                        Thread.Sleep(250);
                    }
                    else
                    {
                        Settings0();
                    }

                    Settings0();
                    break;
                }
                case "2":
                {
                    switch (Bot.BotRunning)
                    {
                        case true:
                            System.Console.Clear();
                            System.Console.WriteLine();
                            Program.Ascii();
                            System.Console.WriteLine();
                            System.Console.WriteLine();
                            Console.Write("    [", Color.White);
                            Console.Write("Bot Is Already Running!", Color.Red);
                            Console.Write("]\n", Color.White);
                            Thread.Sleep(600);
                            Settings0();
                            break;
                        default:
                            System.Console.Clear();
                            System.Console.WriteLine();
                            Program.Ascii();
                            System.Console.WriteLine();
                            System.Console.WriteLine();
                            Console.Write("    [", Color.White);
                            Console.Write("Enter Bot Token", Color.Purple);
                            Console.Write("]\n", Color.White);
                            Console.Write("    [", Color.White);
                            Console.Write(">", Color.Purple);
                            Console.Write("] ", Color.White);
                            Bot.botToken = System.Console.ReadLine();
                            Console.Write("    [", Color.White);
                            Console.Write("Enter Bot Prefix", Color.Purple);
                            Console.Write("]\n", Color.White);
                            Console.Write("    [", Color.White);
                            Console.Write(">", Color.Purple);
                            Console.Write("] ", Color.White);
                            Bot.Prefix = System.Console.ReadLine();
                            Console.Write("    [", Color.White);
                            Console.Write("Enter User Discord ID", Color.Purple);
                            Console.Write("]\n", Color.White);
                            Console.Write("    [", Color.White);
                            Console.Write(">", Color.Purple);
                            Console.Write("] ", Color.White);
                            Settings.DiscordID = System.Console.ReadLine();
                            StartBot();
                            Bot.BotRunning = true;
                            break;
                    }

                    break;
                }
                case "X":
                    System.Console.Clear();
                    Program.Menu0();
                    break;
                default:
                    Program.prefix("Invalid Option", "");
                    Thread.Sleep(300);
                    Settings0();
                    break;
            }

            Thread.Sleep(1000);
        }

        public static void StartBot()
        {
            var prog = new Bot();
            prog.MainAsync().GetAwaiter().GetResult();
        }
    }
}