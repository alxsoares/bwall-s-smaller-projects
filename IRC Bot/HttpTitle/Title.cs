using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace HttpTitle
{
    class TitleModule : Module
    {
        List<KeyValuePair<DateTime, KeyValuePair<string, string>>> links = new List<KeyValuePair<DateTime, KeyValuePair<string, string>>>();
        List<KeyValuePair<DateTime, KeyValuePair<string, string>>> youtubes = new List<KeyValuePair<DateTime, KeyValuePair<string, string>>>();

        public override string GetName()
        {
            return "TitleGrabber";
        }

        public override string GetHelp()
        {
            return "Gets the title of a posted link, no command required.";
        }

        public override string GetSpecificHelp(string category)
        {
            return null;
        }

        public override void AddBindings()
        {
            youtubes.Clear();
            links.Clear();
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(CheckMessage);
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(CheckMessage);
        }

        void CheckMessage(MSG msg)
        {
            if (msg.from == "blackra1n" || msg.from == irc.nick)
                return;
            Regex http = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            Regex https = new Regex("https://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            Match a = http.Match(msg.message);
            if (a.Success)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ReportURL));
                msg.message = a.Value;
                t.Start(msg);
                return;
            }
            Match b = https.Match(msg.message);
            if (b.Success)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ReportURL));
                msg.message = b.Value;
                t.Start(msg);
                return;
            }
        }

        string RemoveNonPrintableCharacters(string s)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                byte b = (byte)c;
                if (b >= 32)
                    result.Append(c);
            }
            return result.ToString();
        }

        public override void Load()
        {
            Regex extract = new Regex("(?<date>[^ ]*)( )(?<link>[^ ]*)( )(?<title>.*)");
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "youtubes"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "youtubes"))
                {
                    Match m = extract.Match(line);
                    youtubes.Add(new KeyValuePair<DateTime, KeyValuePair<string, string>>(DateTime.FromBinary(long.Parse(m.Groups["date"].Value)), new KeyValuePair<string, string>(m.Groups["link"].Value, m.Groups["title"].Value)));
                }
            }
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "link"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "links"))
                {
                    Match m = extract.Match(line);
                    links.Add(new KeyValuePair<DateTime, KeyValuePair<string, string>>(DateTime.FromBinary(long.Parse(m.Groups["date"].Value)), new KeyValuePair<string, string>(m.Groups["link"].Value, m.Groups["title"].Value)));
                }
            }
        }

        public override void Save()
        {
            Directory.CreateDirectory(server.Data);
            string toFile = "";
            foreach (KeyValuePair<DateTime, KeyValuePair<string, string>> y in youtubes)
                toFile += y.Key.ToBinary().ToString() + " " + y.Value.Key + " " + y.Value.Value + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "youtubes", toFile);
            toFile = "";
            foreach (KeyValuePair<DateTime, KeyValuePair<string, string>> y in links)
                toFile += y.Key.ToBinary().ToString() + " " + y.Value.Key + " " + y.Value.Value + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "links", toFile);
        }

        void ReportURL(object m)
        {
            try
            {
                MSG msg = (MSG)m;

                //Youtube
                if (msg.message.Contains("youtube.com"))
                {
                    string newUrl = "http://www.youtube.com/watch?v=" + msg.message.Substring(msg.message.IndexOf("v=") + 2, "WQriZQbTcjk".Length);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.youtube.com/oembed?url=" + newUrl + "&format=xml");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string type = response.GetResponseHeader("CONTENT-TYPE");
                    if (type == null || string.IsNullOrEmpty(type) || !type.Contains("text/xml"))
                        return;
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string buffer = "";
                    while (true)
                    {
                        if (reader.EndOfStream)
                            return;
                        if (reader.Read() == (int)'<')
                        {
                            if (reader.Read() != (int)'t')
                                continue;
                            if (reader.Read() != (int)'i')
                                continue;
                            if (reader.Read() != (int)'t')
                                continue;
                            if (reader.Read() != (int)'l')
                                continue;
                            if (reader.Read() != (int)'e')
                                continue;
                            while (reader.Read() != (int)'>') ;
                            while (true)
                            {
                                char input = (char)reader.Read();
                                if (reader.EndOfStream)
                                    return;
                                if (input == '<')
                                {
                                    irc.SendMessage(msg.to, "*" + RemoveNonPrintableCharacters(buffer) + "* - " + msg.message);
                                    youtubes.Add(new KeyValuePair<DateTime, KeyValuePair<string, string>>(msg.when, new KeyValuePair<string, string>(msg.message, RemoveNonPrintableCharacters(buffer))));
                                    while (youtubes.Count >= 20)
                                        youtubes.RemoveAt(0);
                                    return;
                                }
                                else
                                    buffer += input;
                            }
                        }
                    }
                }


                //Just a web page

                HttpWebRequest requestw = (HttpWebRequest)WebRequest.Create(msg.message);
                HttpWebResponse responsew = (HttpWebResponse)requestw.GetResponse();
                string typew = responsew.GetResponseHeader("CONTENT-TYPE");
                if (typew == null || string.IsNullOrEmpty(typew) || !typew.Contains("text/html"))
                    return;
                StreamReader readerw = new StreamReader(responsew.GetResponseStream());
                string bufferw = "";                
                while (true)
                {
                    if (readerw.EndOfStream)
                        return;
                     if (readerw.Read() == (int)'<')
                     {
                        if (readerw.Read() != (int)'t')
                            continue;
                        if (readerw.Read() != (int)'i')
                            continue;
                        if (readerw.Read() != (int)'t')
                            continue;
                        if (readerw.Read() != (int)'l')
                            continue;
                        if (readerw.Read() != (int)'e')
                            continue;
                        while (readerw.Read() != (int)'>') ;
                        while (true)
                        {
                            char input = (char)readerw.Read();
                            if (readerw.EndOfStream)
                                return;
                            if (input == '<')
                            {
                                irc.SendMessage(msg.to, "*" + RemoveNonPrintableCharacters(bufferw) + "* - " + msg.message);
                                links.Add(new KeyValuePair<DateTime, KeyValuePair<string, string>>(msg.when, new KeyValuePair<string, string>(msg.message, RemoveNonPrintableCharacters(bufferw))));
                                while (links.Count >= 20)
                                    links.RemoveAt(0);
                                return;
                            }
                            else
                                bufferw += input;
                        }                                                            
                    }                   
                }
            }
            catch { }
        }
    }
}
