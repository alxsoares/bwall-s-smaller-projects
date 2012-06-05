using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;

namespace Roulette
{
    class Roulette : Module
    {
        int bullet = 0;
        int position = 0;
        Dictionary<string, KeyValuePair<int, int>> stats = new Dictionary<string, KeyValuePair<int, int>>();

        void CheckMessage(MSG msg)
        {
            if (msg.message == ".fire")
            {
                KeyValuePair<int, int> stat = new KeyValuePair<int, int>(0, 0);
                if (!stats.TryGetValue(msg.from, out stat))
                {

                }
                if (bullet == position)
                {
                    stats[msg.from] = new KeyValuePair<int, int>(stat.Key + 1, stat.Value);
                    irc.SendMessage(msg.to, msg.from + "'s brains are all over the walls!");
                    irc.KickUser(msg.to, msg.from, "You dead foo...");
                    Reload();
                    irc.SendMessage(msg.to, "Reloading");
                    return;
                }
                else
                {
                    stats[msg.from] = new KeyValuePair<int, int>(stat.Key, stat.Value + 1);
                    irc.SendMessage(msg.to, msg.from + " is lucky...this time.");
                    position++;
                }
            }
            else if (msg.message == ".reload")
            {
                Reload();
                irc.SendMessage(msg.to, "Reloading");
            }
        }

        public override string GetStats()
        {
            string ret = "";
            foreach (string nick in stats.Keys)
            {
                ret += GetSpecificStats(nick) + "\n";
            }
            return ret;
        }

        public override string GetSpecificStats(string nick)
        {
            if (stats.ContainsKey(nick))
            {
                return nick + " has survived " + stats[nick].Value.ToString() + " shots, but has died " + stats[nick].Key.ToString();
            }
            return null;
        }

        void Reload()
        {
            position = 0;
            bullet = new Random().Next(6);
        }

        public override void AddBindings()
        {
            Reload();
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(CheckMessage);
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(CheckMessage);
        }

        public override string GetName()
        {
            return "Roulette";
        }

        public override string GetHelp()
        {
            return "A simple roulette game with two commands, .fire and .reload";
        }

        public override string GetSpecificHelp(string category)
        {
            switch (category)
            {
                case ".fire":
                    return "Gives the user a shot at death...";
                case ".reload":
                    return "Reloads the revolver, should only be used in case of error.";
            }
            return null;
        }

        public override string GetCommands()
        {
            return ".fire\n.reload";
        }
    }
}
