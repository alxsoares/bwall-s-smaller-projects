using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.IO;

namespace NickServLogin
{
    public class NickServLogin : Module
    {
        string lastOper = null;
        Dictionary<string, string> logins = new Dictionary<string, string>();

        public override void AddBindings()
        {
            irc.OnPMRecvd += new IrcClient.MSGRecvd(irc_OnNoticeRecvd);
            irc.OnNoticeRecvd += new IrcClient.MSGRecvd(irc_OnNoticeRecvd);
        }

        void irc_OnNoticeRecvd(MSG msg)
        {
            if (msg.from.ToLower() == "nickserv")
            {
                if (lastOper != null)
                {
                    irc.SendMessage(lastOper, msg.message);
                }
                else
                    Console.WriteLine(msg.ToString());
                if (msg.message.StartsWith("This nickname is registered") && logins.ContainsKey(msg.to))
                    irc.SendMessage("nickserv", "identify " + logins[msg.to]);
            }            
        }

        public override void Load()
        {
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "nickserv.passwd"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "nickserv.passwd"))
                    logins.Add(line.Split(' ')[0], line.Split(' ')[1]);
            }
        }

        public override void Save()
        {
            string toFile = "";
            foreach (KeyValuePair<string, string> login in logins)
            {
                toFile += login.Key + " " + login.Value + "\n";
            }
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "nickserv.passwd", toFile);
        }

        void irc_OnPMRecvd(MSG msg)
        {

        }

        public override void RemoveBindings()
        {
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(irc_OnNoticeRecvd);
            irc.OnNoticeRecvd -= new IrcClient.MSGRecvd(irc_OnNoticeRecvd);
        }

        public override string GetName()
        {
            return "NickServLogin";
        }

        public override string GetHelp()
        {
            return null;
        }

        public override void AuthedCommand(MSG msg, int operLevel)
        {
            try
            {
                if (operLevel == 0)
                {
                    msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
                    if (msg.message.ToLower().StartsWith("register"))
                    {
                        lastOper = msg.from;
                        irc.SendMessage("nickserv", msg.message);
                        if (msg.message.Split(' ').Length > 1)
                            logins[msg.to] = msg.message.Split(' ')[1];
                    }
                    else if (msg.message.ToLower().StartsWith("identify"))
                    {
                        lastOper = msg.from;
                        irc.SendMessage("nickserv", msg.message);
                        if (msg.message.Split(' ').Length > 1)
                            logins[msg.to] = msg.message.Split(' ')[1];
                    }
                }
            }
            catch { }
        }
    }
}
