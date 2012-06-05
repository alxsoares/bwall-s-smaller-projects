using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.IO;

namespace SuggestionBox
{
    public class SuggestionBox : Module
    {
        List<MSG> newSuggestions = new List<MSG>();
        List<MSG> oldSuggestions = new List<MSG>();

        public override void AuthedCommand(MSG msg, int operLevel)
        {
            msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
            if (msg.message == ".getsuggestion")
            {
                if (newSuggestions.Count == 0)
                {
                    irc.SendMessage(msg.from, "Sorry, no new suggestions.");
                    return;
                }
                MSG m = newSuggestions[0];
                newSuggestions.RemoveAt(0);
                irc.SendMessage(msg.from, m.ToString());
                oldSuggestions.Add(m);
                Save();
            }
        }

        void Message(MSG msg)
        {
            if (msg.message.StartsWith(".suggestion "))
            {
                msg.message = msg.message.Substring(".suggestion ".Length);
                newSuggestions.Add(msg);
                Save();
            }
            else if(msg.message.ToLower().Contains(server.nick.ToLower()))
            {
                newSuggestions.Add(msg);
            }
            else if (msg.message.StartsWith(".getsuggestion") && msg.from == "CrimsonRed")
            {
                if(newSuggestions.Count == 0)
                {
                    irc.SendMessage(msg.from, "Sorry, no new suggestions.");
                    return;
                }
                MSG m = newSuggestions[0];
                newSuggestions.RemoveAt(0);
                irc.SendMessage(msg.from, m.ToString());
                oldSuggestions.Add(m);
                Save();
            }
        }

        public override void Save()
        {            
            string toFile = "";
            for (int x = 0; x < newSuggestions.Count; x++)
                toFile += newSuggestions[x].ToString() + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "suggestions.new", toFile);
            toFile = "";
            for (int x = 0; x < oldSuggestions.Count; x++)
                toFile += oldSuggestions[x].ToString() + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "suggestions.old", toFile);
        }

        public override void Load()
        {
            newSuggestions.Clear();
            oldSuggestions.Clear();
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "suggestions.new"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "suggestions.new"))
                    newSuggestions.Add(new MSG(line));
            }
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "suggestions.old"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "suggestions.old"))
                    oldSuggestions.Add(new MSG(line));
            }
        }

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(Message);
            irc.OnPMRecvd += new IrcClient.MSGRecvd(Message);
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(Message);
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(Message);
        }

        public override string GetName()
        {
            return "Suggestions";
        }

        public override string GetHelp()
        {
            return "Lets you leave me suggestions for this bot with \".suggestion <suggestion>\"";
        }

        public override string GetCommands()
        {
            return ".suggestion";
        }
    }
}
