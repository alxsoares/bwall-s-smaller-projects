using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ModularIrcBot;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace LastFm
{
    class LastFm : Module
    {
        Dictionary<string, string> registrar = new Dictionary<string, string>();

        void CheckMessage(MSG msg)
        {
            if ((msg.message.StartsWith(".np") || msg.message.StartsWith(".similar") || msg.message.StartsWith(".lastfm")) && msg.message.Split(' ').Length > 1)
            {
                Thread t = new Thread(new ParameterizedThreadStart(GetLastFm));
                t.Start(msg);
            }
            else if (msg.message == ".np" && registrar.ContainsKey(msg.from))
            {
                Thread t = new Thread(new ParameterizedThreadStart(GetLastFm));
                t.Start(msg);
            }
            else if (msg.message.StartsWith(".reglastfm "))
            {
                registrar[msg.from] = msg.message.Split(' ')[1];
                Save();
            }
        }

        public override void Save()
        {
            string output = "";
            foreach (KeyValuePair<string, string> user in registrar)
                output += user.Key + ":" + user.Value + "\n";
            Directory.CreateDirectory(server.Data);
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "lastfm.db", output);
        }

        public override void Load()
        {
            try
            {
                if (File.Exists(server.Data + Path.DirectorySeparatorChar + "lastfm.db"))
                {
                    foreach (string line in EncFile.ReadAllText(server.Data + Path.DirectorySeparatorChar + "lastfm.db").Split("\n".ToCharArray()))
                    {
                        if(!string.IsNullOrEmpty(line))
                            registrar.Add(line.Split(':')[0], line.Split(':')[1]);
                    }
                }
            }
            catch { }
        }

        void GetLastFm(object o)
        {
            try
            {
                MSG msg = new MSG(((MSG)o).ToString());
                WebClient wc = new WebClient();
                wc.Encoding = ASCIIEncoding.ASCII;
                if (msg.message == ".np" && registrar.ContainsKey(msg.from))
                {
                    string str = wc.DownloadString("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=" + registrar[msg.from] + "&api_key=10e60166f9879621346d97c256600336&limit=1");
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(str);
                    XmlNodeList nodeList = doc.SelectNodes("//track");
                    XmlNode nodex = nodeList[0];
                    if (nodex.ChildNodes.Count != 0 && nodex.Attributes["nowplaying"] != null && nodex.Attributes["nowplaying"].Value == "true")
                    {
                        string band = nodex.ChildNodes[0].InnerText;
                        string song = nodex.ChildNodes[1].InnerText;
                        irc.SendMessage(msg.to, msg.from + " is now playing: " + band + " - " + song);
                    }
                    return;
                }
                string command = msg.message.Split(' ')[0];              
                string argument = msg.message.Substring(command.Length + 1);

                switch (command)
                {
                    case ".similar":
                        {
                            string str = wc.DownloadString("http://ws.audioscrobbler.com/2.0/artist/" + argument.Replace(" ", "+") + "/similar.txt");                            
                            int count = 0;
                            foreach (string line in str.Split("\n".ToCharArray()))
                            {                                
                                count++;
                                string band = line.Split(',')[line.Split(',').Length - 1];
                                irc.SendMessage(msg.to, (count).ToString() + ". " + band);
                                if (count == 10)
                                    break;
                            }
                            break;
                        }
                    case ".lastfm":
                        {
                            string str = wc.DownloadString("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=" + argument + "&api_key=10e60166f9879621346d97c256600336&limit=3");                            
                            int count = 0;
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(str);
                            XmlNodeList nodeList = doc.SelectNodes("//track");
                            foreach (XmlNode nodex in nodeList)
                            {
                                count++;
                                if (nodex.ChildNodes.Count != 0)
                                {
                                    string band = nodex.ChildNodes[0].InnerText;
                                    string song = nodex.ChildNodes[1].InnerText;
                                    irc.SendMessage(msg.to, count.ToString() + ". " + band + " - " + song);
                                }
                            }                            
                            break;
                        }
                    case ".np":
                        {
                            string str = wc.DownloadString("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=" + argument + "&api_key=10e60166f9879621346d97c256600336&limit=1");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(str);
                            XmlNodeList nodeList = doc.SelectNodes("//track");
                            XmlNode nodex = nodeList[0];
                            if (nodex.ChildNodes.Count != 0 && nodex.Attributes["nowplaying"] != null && nodex.Attributes["nowplaying"].Value == "true")
                            {
                                string band = nodex.ChildNodes[0].InnerText;
                                string song = nodex.ChildNodes[1].InnerText;
                                irc.SendMessage(msg.to, argument + " is now playing: " + band + " - " + song);
                            }
                            break;
                        }
                }
            }
            catch (Exception e) { Console.WriteLine("[" + DateTime.Now.ToString() + "] LastFM:(" + e.Source + ") " + e.Message); }
        }

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(CheckMessage);
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(CheckMessage);
        }

        public override string GetName()
        {
            return "LastFM";
        }

        public override string GetHelp()
        {
            return "This module uses LastFm to power the following 3 commands\n"
                + "\".similar <band>\" gets the bands similar to <band>\n"
                + "\".lastfm <username>\" gets the last 3 songs this lastfm user has listened to\n"
                + "\".np <username>\" gets the song that this lastfm user is currently listening to\n"
                + "\".reglastfm <username>\" registers your nick so you can run \".np\" without a lastfm username";
        }

        public override string GetSpecificHelp(string category)
        {
            return null;
        }

        public override string GetCommands()
        {
            return ".similar\n.lastfm\n.np\n.reglastfm";
        }
    }
}
