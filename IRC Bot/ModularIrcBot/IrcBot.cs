using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Reflection;

/* Stuff to implement
 * .note        (leave note for a user the next time they join)
 * !help
 * !lastsaid
 * !define
 * !whois
 * !lastfm      (gets the last 3 songs played by this user)    
 * !similar     (gets similar bands to the one supplied)
 * .suggestion  (gets suggestions for the bot)
 * .tracenick   (follows the nicks a user has used)
 */

namespace ModularIrcBot
{
    public class IrcBot
    {
        public Queue<RawMsg> backlog = new Queue<RawMsg>(1000);
        public delegate void NM(RawMsg rm, Server s);
        public event NM newMessage;

        List<Module> modules = new List<Module>();
        public IrcClient irc;
        List<string> channels = new List<string>();
        OperatorList operatorList;
        Dictionary<string, string> loadedMods = new Dictionary<string, string>();
        public bool Running = false;
        Server server = null;

        public void RegisterNewOperator(string nick, string password)
        {
            operatorList.AddOperator(nick, password);
        }

        public void RemoveOperator(string nick)
        {
            operatorList.RemoveOperator(nick);
        }

        public List<string> GetOperators()
        {
            return operatorList.ListOperators();
        }

        public void ReloadAllModule(string folder)
        {
            foreach (string name in loadedMods.Keys)
                ReloadModule(name);
        }

        public void LoadAllModules(string folder)
        {
            foreach (string file in Directory.GetFiles(folder, "*.dll"))
                LoadModule(file, server);
        }

        public void LoadModule(string file, Server server)
        {
            if (file.Contains("ModularIrcBot.dll") || file.Contains("GraphGenerator.dll"))
                return;
            try
            {
                if (loadedMods.ContainsValue(file))
                    return;
                Assembly assembly = Assembly.Load(File.ReadAllBytes(file));
                Type[] type = assembly.GetTypes();
                foreach (Type t in type)
                {
                    if (typeof(Module).IsAssignableFrom(t))
                    {
                        Module mod = (Module)Activator.CreateInstance(t);
                        mod.StartModule(irc, server);
                        modules.Add(mod);
                        loadedMods.Add(mod.GetName(), file);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(file + " failed to load: " + e.Message);
            }
        }

        public void UnloadModule(string name)
        {
            for (int x = 0; x < loadedMods.Count; x++)
            {
                if (name == modules[x].GetName())
                {
                    modules[x].StopModule();
                    modules.RemoveAt(x);
                    loadedMods.Remove(name);
                }
            }
        }

        public void ReloadModule(string name)
        {
            string location = loadedMods[name];
            UnloadModule(name);
            LoadModule(location, server);
        }

        public IrcBot(Server server)
        {
            this.server = server;
            irc = new IrcClient(server.nick, server.user, server.real, server.host);
            operatorList = new OperatorList(irc, server);
            LoadAllModules(Config.I.vars["ModuleLocation"]);
        }

        public void StartBot()
        {           
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(CheckMessage);
            irc.OnPMRecvd += new IrcClient.MSGRecvd(PrivMessage);
            irc.OnCTCPRecvd += new IrcClient.CTCPRecvd(irc_OnCTCPRecvd);
            irc.OnArchMsg += new IrcClient.ArchRecvd(irc_OnArchMsg);
            irc.OnDisconnect += new ThreadStart(irc_OnDisconnect);
            irc.StartBot(server.domain, server.port, server.useSSL, server.SSLFolder, server);
            Running = true;
            foreach (string chan in server.channels)
                irc.JoinChannel(chan);
        }

        void irc_OnDisconnect()
        {
            //bool reconnect = false;
            //while (!reconnect)
            //{
            //    try
            //    {
            //        irc = new IrcClient(server.nick + "_", server.user, server.real, server.host);
            //        operatorList = new OperatorList(irc, server);
            //        ReloadAllModule(null);
            //        StartBot();
            //        reconnect = true;
            //    }
            //    catch { Thread.Sleep(5000); }
            //}
        }

        void irc_OnArchMsg(RawMsg amsg)
        {
            backlog.Enqueue(amsg);
            if (newMessage != null)
                newMessage(amsg, server);
        }

        void irc_OnCTCPRecvd(CTCP ctcp)
        {
            if (ctcp.command.ToLower() == "version")
            {
                irc.SendCTCPResponse(ctcp.from, ctcp.command, server.Version);
            }
        }

        public void Quit()
        {
            operatorList.Save();
            foreach (Module mod in modules)
                mod.StopModule();
            irc.Quit("Peace out!");
            Running = false;
        }

        void CheckMessage(MSG msg)
        {
            if (msg.message.StartsWith(".stats"))
            {
                if (msg.message == ".stats")
                    irc.SendMessage(msg.from, "Use \".stats <Module>\" for stats from a module or \".stats <Module> <Nick>\" to get all the stats for a nick in that module");
                else if (msg.message.Split(' ').Length == 2)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == msg.message.Split(' ')[1].ToLower())
                        {
                            string stats = mod.GetStats();
                            if (!string.IsNullOrEmpty(stats))
                            {
                                foreach (string line in stats.Split("\n".ToCharArray()))
                                {
                                    if (!string.IsNullOrEmpty(line))
                                        irc.SendMessage(msg.to, mod.GetName() + " - " + line);
                                }
                            }
                            else
                            {
                                irc.SendMessage(msg.to, "No stats for " + mod.GetName());
                            }
                        }
                    }
                }
                else if (msg.message.Split(' ').Length == 3)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == msg.message.Split(' ')[1].ToLower())
                        {
                            string stats = mod.GetSpecificStats(msg.message.Split(' ')[2].ToLower());
                            if (!string.IsNullOrEmpty(stats))
                            {
                                foreach (string line in stats.Split("\n".ToCharArray()))
                                {
                                    if (!string.IsNullOrEmpty(line))
                                        irc.SendMessage(msg.to, mod.GetName() + " - " + line);
                                }
                            }
                            else
                            {
                                irc.SendMessage(msg.to, "No stats for " + mod.GetName() + " " + msg.message.Split(' ')[2]);
                            }
                        }
                    }
                }
            }
            else if (msg.message == ".help")
            {
                //Generate and send help info
                irc.SendMessage(msg.to, "Here to help you!");
                irc.SendMessage(msg.to, "List of Modules:");
                foreach (Module mod in modules)
                    irc.SendMessage(msg.to, "   " + mod.GetName());
                irc.SendMessage(msg.to, "Say \".help <module name>\" to get help on specific modules");
            }
            else if (msg.message.StartsWith(".help ") && false)
            {
                string[] split = msg.message.Split(' ');
                if (split.Length == 2)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == split[1].ToLower())
                        {
                            foreach (string line in mod.GetHelp().Split("\n".ToCharArray()))
                                irc.SendMessage(msg.to, mod.GetName() + " - " + line);
                        }
                    }
                }
                else if (split.Length == 3)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == split[1].ToLower())
                        {
                            if (mod.GetSpecificHelp(split[2]) != null)
                            {
                                foreach (string line in mod.GetSpecificHelp(split[2].ToLower()).Split("\n".ToCharArray()))
                                    irc.SendMessage(msg.to, mod.GetName() + " - " + split[2] + " - " + line);
                            }
                        }
                    }
                }
            }
            else if (msg.message == ".commands")
            {
                foreach (Module mod in modules)
                {
                    if (mod.GetCommands() != null)
                    {
                        foreach (string line in mod.GetCommands().Split("\n".ToCharArray()))
                            irc.SendMessage(msg.to, mod.GetName() + " - " + line);
                    }
                }
            }
            else if (msg.message.StartsWith(".commands "))
            {
                string[] split = msg.message.Split(' ');
                if (split.Length == 2)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == split[1].ToLower())
                        {
                            foreach (string line in mod.GetCommands().Split("\n".ToCharArray()))
                                irc.SendMessage(msg.to, mod.GetName() + " - " + line);
                        }
                    }
                }
            }
        }

        void PrivMessage(MSG msg)
        {
            if (msg.message.StartsWith(".stats"))
            {
                if (msg.message == ".stats")
                    irc.SendMessage(msg.from, "Use \".stats <Module>\" for stats from a module or \".stats <Module> <Nick>\" to get all the stats for a nick in that module");
                else if (msg.message.Split(' ').Length == 2)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == msg.message.Split(' ')[1].ToLower())
                        {
                            string stats = mod.GetStats();
                            if (!string.IsNullOrEmpty(stats))
                            {
                                foreach (string line in stats.Split("\n".ToCharArray()))
                                {
                                    if (!string.IsNullOrEmpty(line))
                                        irc.SendMessage(msg.from, mod.GetName() + " - " + line);
                                }
                            }
                            else
                            {
                                irc.SendMessage(msg.from, "No stats for " + mod.GetName());
                            }
                        }
                    }
                }
                else if (msg.message.Split(' ').Length == 3)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == msg.message.Split(' ')[1].ToLower())
                        {
                            string stats = mod.GetSpecificStats(msg.message.Split(' ')[2].ToLower());
                            if (!string.IsNullOrEmpty(stats))
                            {
                                foreach (string line in stats.Split("\n".ToCharArray()))
                                {
                                    if (!string.IsNullOrEmpty(line))
                                        irc.SendMessage(msg.from, mod.GetName() + " - " + line);
                                }
                            }
                            else
                            {
                                irc.SendMessage(msg.from, "No stats for " + mod.GetName() + " " + msg.message.Split(' ')[2]);
                            }
                        }
                    }
                }
            }
            else if (msg.message == ".help")
            {
                //Generate and send help info
                irc.SendMessage(msg.from, "Here to help you!");
                irc.SendMessage(msg.from, "List of Modules:");
                foreach (Module mod in modules)
                    irc.SendMessage(msg.from, "   " + mod.GetName());
                irc.SendMessage(msg.from, "Say \".help <module name>\" to get help on specific modules");
            }
            else if (msg.message.StartsWith(".help ") && false)
            {
                string[] split = msg.message.Split(' ');
                if (split.Length == 2)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == split[1].ToLower())
                        {
                            foreach (string line in mod.GetHelp().Split("\n".ToCharArray()))
                                irc.SendMessage(msg.from, mod.GetName() + " - " + line);
                        }
                    }
                }
                else if (split.Length == 3)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == split[1].ToLower())
                        {
                            if (mod.GetSpecificHelp(split[2]) != null)
                            {
                                foreach (string line in mod.GetSpecificHelp(split[2].ToLower()).Split("\n".ToCharArray()))
                                    irc.SendMessage(msg.from, mod.GetName() + " - " + split[2] + " - " + line);
                            }
                        }
                    }
                }
            }
            else if (msg.message == ".commands")
            {
                foreach (Module mod in modules)
                {
                    if (mod.GetCommands() != null)
                    {
                        foreach (string line in mod.GetCommands().Split("\n".ToCharArray()))
                            irc.SendMessage(msg.from, mod.GetName() + " - " + line);
                    }
                }
            }
            else if (msg.message.StartsWith(".commands "))
            {
                string[] split = msg.message.Split(' ');
                if (split.Length == 2)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod.GetName().ToLower() == split[1].ToLower())
                        {
                            foreach (string line in mod.GetCommands().Split("\n".ToCharArray()))
                                irc.SendMessage(msg.from, mod.GetName() + " - " + line);
                        }
                    }
                }
            }
            if (msg.message.StartsWith("ident "))
            {
                if (operatorList.LogIn(msg.from, msg.fromUser, msg.fromHost, msg.message.Substring("ident ".Length)))
                    irc.SendMessage(msg.from, "You are now logged in.");
                else
                    irc.SendMessage(msg.from, "Login failed.");
                return;
            }
            if (!operatorList.IsLoggedIn(msg.from, msg.fromUser, msg.fromHost))
                return;
            foreach (Module mod in modules)
            {
                if (msg.message.ToLower().StartsWith(mod.GetName().ToLower() + " "))
                    mod.AuthedCommand(msg, operatorList.GetOperatorLevel(msg.from, msg.fromUser, msg.fromHost));
            }
            if (msg.message.StartsWith("changepass"))
            {
                if (msg.message.Split(' ').Length == 3)
                {
                    irc.SendMessage(msg.from, operatorList.ChangePassword(msg.from, msg.fromUser, msg.fromHost, msg.message.Split(' ')[1], msg.message.Split(' ')[2]));
                }
                else
                    irc.SendMessage(msg.from, "changepass <current password> <new password>");
            }
            if (msg.message == "quit")
            {
                irc.Quit("Peace out!");
                operatorList.Save();
                foreach (Module mod in modules)
                    mod.StopModule();
                Running = false;
            }
            if (msg.message.Split(' ')[0] == "say" && msg.message.Split(' ').Length > 2)
                irc.SendMessage(msg.message.Split(' ')[1], msg.message.Substring(msg.message.IndexOf(msg.message.Split(' ')[1]) + msg.message.Split(' ')[1].Length + 1));
            if (msg.message.Split(' ')[0] == "join" && msg.message.Split(' ').Length == 2)
                irc.JoinChannel(msg.message.Split(' ')[1]);
            if (msg.message.Split(' ')[0] == "part" && msg.message.Split(' ').Length == 2)
                irc.PartFromChannel(msg.message.Split(' ')[1], "I'm a bot foo");
            if (msg.message.Split(' ')[0] == "kick" && msg.message.Split(' ').Length > 3)
                irc.KickUser(msg.message.Split(' ')[1], msg.message.Split(' ')[2], msg.message.Substring(msg.message.IndexOf(msg.message.Split(' ')[2]) + msg.message.Split(' ')[2].Length + 1));
            if (msg.message.Split(' ')[0] == "raw" && msg.message.Split(' ').Length > 1)
                irc.SendRaw(msg.message.Substring(4));
            try
            {
                if (msg.message.Split(' ')[0] == "module" && msg.message.Split(' ').Length >= 2)
                {
                    switch (msg.message.Split(' ')[1])
                    {
                        case "list":
                            {
                                foreach (string file in Directory.GetFiles(Config.I.vars["ModuleLocation"], "*.dll"))
                                {
                                    if (loadedMods.ContainsValue(file))
                                    {
                                        foreach (KeyValuePair<string, string> info in loadedMods)
                                        {
                                            if (info.Value == file)
                                            {
                                                irc.SendMessage(msg.from, "[Loaded]" + info.Key + " @ " + file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        irc.SendMessage(msg.from, "[Not Loaded]" + file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1));
                                    }
                                }
                                break;
                            }
                        case "reload":
                            {
                                if (msg.message.Split(' ').Length == 3)
                                {
                                    if (loadedMods.ContainsKey(msg.message.Split(' ')[2]))
                                        ReloadModule(msg.message.Split(' ')[2]);
                                }
                                break;
                            }
                        case "unload":
                            {
                                if (msg.message.Split(' ').Length == 3)
                                {
                                    if (loadedMods.ContainsKey(msg.message.Split(' ')[2]))
                                        UnloadModule(msg.message.Split(' ')[2]);
                                }
                                break;
                            }
                        case "load":
                            {
                                if (msg.message.Split(' ').Length == 3)
                                    LoadModule(Config.I.vars["ModuleLocation"] + Path.DirectorySeparatorChar + msg.message.Split(' ')[2], server);
                                break;
                            }
                        case "save":
                            {
                                Thread t = new Thread(new ThreadStart(SaveAllModuleInfo));
                                t.Start();
                                break;
                            }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("[" + DateTime.Now.ToString() + "] IrcBot.Module:(" + e.Source + ") " + e.Message); }
        }

        public void SaveAllModuleInfo()
        {
            foreach (Module mod in modules)
            {
                Console.WriteLine("Saving " + mod.GetName());
                mod.Save();
                Console.WriteLine("Saved " + mod.GetName());
            }
        }
    }
}
