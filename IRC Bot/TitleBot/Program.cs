using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ModularIrcBot;

namespace TitleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            List<KeyValuePair<Server, IrcBot>> bots = new List<KeyValuePair<Server, IrcBot>>();
            Config.I.LoadServers();
            foreach (Server server in Config.I.servers)
            {
                IrcBot irc = new IrcBot(server);
                Console.WriteLine("Starting " + server.name);
                irc.StartBot();
                bots.Add(new KeyValuePair<Server, IrcBot>(server, irc));
            }
            while (true)
            {
                Console.WriteLine("1. Select a bot");
                Console.WriteLine("2. Add a bot to a server");
                Console.WriteLine("3. Modify Configuration");
                Console.WriteLine("s. Save Configuration");
                Console.WriteLine("q. Quit");
                switch (Console.ReadLine())
                {
                    case "1":
                        {
                            for (int x = 0; x < bots.Count; x++)
                            {
                                Console.WriteLine((x + 1).ToString() + ". " + bots[x].Key.name);
                            }
                            int bot = -1;
                            while (bot == -1)
                            {
                                int.TryParse(Console.ReadLine(), out bot);
                                if (bot < 1 || bot > bots.Count)
                                    bot = -1;
                            }
                            bot--;
                            bool loop = true;
                            while (loop)
                            {
                                Console.WriteLine("1. Add an Operator");
                                Console.WriteLine("2. Remove an Operator");
                                Console.WriteLine("3. List Operators");
                                Console.WriteLine("b. Go Back");
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.Write("Nick: ");
                                        string n = Console.ReadLine();
                                        Console.Write("Password: ");
                                        string p = Console.ReadLine();
                                        bots[bot].Value.RegisterNewOperator(n, p);
                                        break;
                                    case "2":
                                        Console.Write("Operator to Remove: ");
                                        string ni = Console.ReadLine();
                                        bots[bot].Value.RemoveOperator(ni);
                                        break;
                                    case "3":
                                        foreach (string op in bots[bot].Value.GetOperators())
                                            Console.WriteLine(op);
                                        break;
                                    case "b":
                                        loop = false;
                                        break;
                                }
                            }
                            break;
                        }
                    case "2":
                        Server serv = new Server();
                        IrcBot b = new IrcBot(serv);
                        bots.Add(new KeyValuePair<Server, IrcBot>(serv, b));
                        b.StartBot();
                        Config.I.servers.Add(serv);
                        Config.I.Save();
                        Console.WriteLine("Saved");
                        break;
                    case "3":
                        {
                            bool loop = true;
                            while(loop)
                            {
                                Console.WriteLine("EncryptFiles = " + Config.I.EncryptFiles.ToString());
                                foreach(KeyValuePair<string, string> option in Config.I.vars)
                                    Console.WriteLine(option.Key + " = " + option.Value);
                                Console.WriteLine("Enter the option you would like to change or b to go back.");
                                string opt = Console.ReadLine();
                                if (opt == "b")
                                    break;
                                if (Config.I.vars.ContainsKey(opt))
                                {
                                    Console.WriteLine("Enter value");
                                    Config.I.vars[opt] = Console.ReadLine();
                                }
                                else if (opt == "EncryptFiles")
                                {
                                    Console.WriteLine("Enter value[true/false]");
                                    if (!bool.TryParse(Console.ReadLine(), out Config.I.EncryptFiles))
                                        Console.WriteLine("Invalid value.");
                                }
                                else
                                    Console.WriteLine("That is not an option to edit.");
                            }
                            break;
                        }
                    case "s":
                        {
                            Config.I.Save();
                            break;
                        }
                    case "q":
                        {
                            for (int x = 0; x < bots.Count; x++)
                                bots[x].Value.Quit();
                            return;
                        }
                }
            }
        }
    }
}
