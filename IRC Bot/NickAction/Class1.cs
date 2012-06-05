using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.IO;

namespace NickAction
{
    public class NickAction : Module
    {
        List<string> actions = new List<string>();
        string action = "shakes hands with $nick";
        string lastNick = "";
        Random rand = new Random();

        public override void Load()
        {
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "actions.db"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "actions.db"))
                {
                    actions.Add(line);
                }
            }
            else
            {
                File.Create(server.Data + Path.DirectorySeparatorChar + "actions.db").Close();
                EncFile.AppendText(server.Data + Path.DirectorySeparatorChar + "actions.db", "shakes hands with $nick\n");
                actions.Add("shakes hands with $nick");
            }
        }

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        void irc_OnMSGRecvd(MSG msg)
        {
            if (msg.message.StartsWith(".addact "))
            {
                action = msg.message.Substring(".addact ".Length);
                if (!action.Contains("$nick"))
                    action += " $nick";
                actions.Add((string)action.Clone());
                EncFile.AppendText(server.Data + Path.DirectorySeparatorChar + "actions.db", action + "\n");
            }
            else if (msg.message.StartsWith(".act"))
            {
                if (msg.message.Split(' ').Length > 1)
                {
                    action = actions[rand.Next(actions.Count)];
                    irc.SendMe(msg.to, action.Replace("$nick", msg.message.Split(' ')[1]));
                }
                else
                {
                    action = actions[rand.Next(actions.Count)];
                    irc.SendMe(msg.to, action.Replace("$nick", msg.from));
                }
            }
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        public override string GetName()
        {
            return "NickAction";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
