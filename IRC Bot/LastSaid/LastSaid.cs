using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ModularIrcBot;

namespace LastSaid
{
    class LastSaid : Module
    {
        Dictionary<string, MSG> database = new Dictionary<string, MSG>();
        List<MSG> log = new List<MSG>();
        List<MSG> quotes = new List<MSG>();

        int GetUserLines(string channel, string user, int lines)
        {
            for (int x = log.Count - 1; x >= 0; x--)
            {
                if (log[x].to == channel && log[x].from == user)
                {
                    if (lines == 0)
                    {
                        return x;
                    }
                    lines--;
                }
            }
            return -1;
        }

        List<MSG> GetMessagesFromThisChannel(string channel)
        {
            List<MSG> ret = new List<MSG>();
            foreach (MSG m in quotes)
            {
                if (m.to == channel)
                    ret.Add(m);
            }
            return ret;
        }

        List<MSG> GetMessagesFromUserInThisChannel(string channel, string nick)
        {
            List<MSG> ret = new List<MSG>();
            foreach (MSG m in quotes)
            {
                if (m.to == channel && m.from == nick)
                    ret.Add(m);
            }
            return ret;
        }

        public override string GetStats()
        {
            //Add a random quote
            int numQ = quotes.Count;
            List<string> users = new List<string>();
            for (int x = 0; x < quotes.Count; x++)
            {
                if (!users.Contains(quotes[x].from))
                    users.Add(quotes[x].from);
            }
            string randquote = "";
            if (quotes.Count != 0)
            {
                MSG q = quotes[new Random().Next(quotes.Count)];
                randquote = q.ToString();
            }
            return numQ.ToString() + " quotes from " + users.Count + " users.\n" + randquote;
        }

        public override string GetSpecificStats(string nick)
        {
            //Add a random quote to each
            if (nick.StartsWith("#"))
            {
                //quotes from channel
                if (nick.Contains(" "))
                {
                    //quotes from user in channel
                    string randquote = "";
                    List<MSG> qu = GetMessagesFromUserInThisChannel(nick.Split(' ')[0], nick.Split(' ')[1]);
                    if (qu.Count != 0)
                    {
                        MSG q = qu[new Random().Next(qu.Count)];
                        randquote = q.ToString();
                    }
                    return GetMessagesFromUserInThisChannel(nick.Split(' ')[0], nick.Split(' ')[1]).Count.ToString() + " quotes by " + nick.Split(' ')[1] + " in " + nick.Split(' ')[0] + "\n" + randquote;
                }
                else
                {
                    string randquote = "";
                    List<MSG> qu = GetMessagesFromThisChannel(nick);
                    if (qu.Count != 0)
                    {
                        MSG q = qu[new Random().Next(qu.Count)];
                        randquote = q.ToString();
                    }
                    return GetMessagesFromThisChannel(nick).Count.ToString() + " quotes in " + nick + "\n" + randquote;
                }
            }
            else
            {
                //quotes from user
                List<MSG> qu = new List<MSG>();
                string randquote = "";
                foreach (MSG m in quotes)
                {
                    if (m.from == nick)
                    {
                        qu.Add(m);
                    }
                }
                if (qu.Count != 0)
                {
                    MSG q = qu[new Random().Next(qu.Count)];
                    randquote = q.ToString();
                }
                return qu.Count.ToString() + " quotes from " + nick + "\n" + randquote;
            }
        }

        void CheckMessage(MSG msg)
        {
            if (msg.message.StartsWith(".lastsaid ") && msg.message.Split(' ').Length > 1)
            {
                if (database.ContainsKey(msg.message.Split(' ')[1]))
                {
                    MSG data = database[msg.message.Split(' ')[1]];
                    irc.SendMessage(msg.to, data.ToString());
                }
                else
                    irc.SendMessage(msg.to, "Sorry, haven't seen them say anything...");
            }
            else if (msg.message.StartsWith(".addquote"))
            {
                if (msg.message.Split(' ').Length == 1)
                {
                    irc.SendMessage(msg.to, "say \".addquote <nick>\" to add what that user just said to the database");
                    irc.SendMessage(msg.to, "say \".addquote <nick> <lines spoken>\" to save a quote from thing they have said back");
                }
                else if (msg.message.Split(' ').Length == 2)
                {
                    int pos = GetUserLines(msg.to, msg.message.Split(' ')[1], 0);
                    if (pos != -1)
                    {
                        quotes.Add(log[pos]);
                    }
                }
                else if (msg.message.Split(' ').Length == 3)
                {
                    int pos = GetUserLines(msg.to, msg.message.Split(' ')[1], int.Parse(msg.message.Split(' ')[2]));
                    if (pos != -1)
                    {
                        quotes.Add(log[pos]);
                    }
                }
            }
            else if (msg.message.StartsWith(".randquote"))
            {
                if (msg.message.Split(' ').Length == 1)
                {
                    List<MSG> qList = GetMessagesFromThisChannel(msg.to);
                    if (qList.Count != 0)
                    {
                        MSG q = qList[new Random().Next(qList.Count)];
                        irc.SendMessage(q.to, q.ToString());
                    }
                }
                else
                {
                    List<MSG> qList = GetMessagesFromUserInThisChannel(msg.to, msg.message.Split(' ')[1]);
                    if (qList.Count != 0)
                    {
                        MSG q = qList[new Random().Next(qList.Count)];
                        irc.SendMessage(q.to, q.ToString());
                    }
                }
            }
            else
            {
                log.Add(msg);
                database[msg.from] = msg;
                while (log.Count > 100)
                    log.RemoveAt(0);
            }
        }

        public override void Save()
        {
            Directory.CreateDirectory(server.Data);
            string quotefile = "";
            for (int x = 0; x < quotes.Count; x++)
                quotefile += quotes[x].ToString() + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "quotes", quotefile);
            string datafile = "";
            foreach (KeyValuePair<string, MSG> last in database)
                datafile += last.Value.ToString() + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "lastsaid", datafile);
        }

        public override void Load()
        {
            database.Clear();
            quotes.Clear();
            log.Clear();
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "quotes"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "quotes"))
                {
                    MSG c = new MSG(line);
                    quotes.Add(c);
                }
            }
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "lastsaid"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "lastsaid"))
                {
                    MSG c = new MSG(line);
                    database[c.from] = c;
                }
            }
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
            return "LastSaid";
        }

        public override string GetHelp()
        {
            return "Prints the last thing someone said along with the time.  Use \".lastsaid <nick>\"\n"
                + "\".addquote <nick>\" to add the last thing said by that nick to the quote database";
        }

        public override string GetSpecificHelp(string category)
        {
            return null;
        }

        public override string GetCommands()
        {
            return ".lastsaid\n.addquote\n.randquote";
        }
    }
}
