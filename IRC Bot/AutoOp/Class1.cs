using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.IO;

namespace AutoOp
{
    public class AutoOp : Module
    {
        Dictionary<string, string> oplist = new Dictionary<string, string>();

        public override void Load()
        {
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "autoop.db"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "autoop.db"))
                {
                    oplist.Add(line.Split(' ')[0], line.Split(' ')[1]);
                }
            }
            else 
            {
                File.Create(server.Data + Path.DirectorySeparatorChar + "autoop.db").Close();
            }
        }

        public override void AuthedCommand(MSG msg, int operLevel)
        {
            msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
            if (msg.message.StartsWith("add ") && msg.message.Split(' ').Length == 3)
            {
                oplist.Add(msg.message.Split(' ')[1], msg.message.Split(' ')[2]);              
                EncFile.AppendText(server.Data + Path.DirectorySeparatorChar + "autoop.db", msg.message.Split(' ')[1] + " " + msg.message.Split(' ')[2] + "\n");
            }
        }

        public override void AddBindings()
        {
            irc.OnPMRecvd += new IrcClient.MSGRecvd(irc_OnPMRecvd);
            irc.OnCTCPRecvd += new IrcClient.CTCPRecvd(irc_OnCTCPRecvd);
            irc.OnJoinRecvd += new IrcClient.JoinRecvd(irc_OnJoinRecvd);
        }

        void irc_OnJoinRecvd(JoinMsg join)
        {
            if (oplist.ContainsKey(join.who))
            {
                irc.SendMessage(join.who, "to get ops, respond with \"op (channel) (password)\"");
            }
        }

        void irc_OnCTCPRecvd(CTCP ctcp)
        {
            if (ctcp.command == "ID")
            {
                irc.SendCTCPResponse(ctcp.from, "ID", "buckey");
            }
        }

        void irc_OnPMRecvd(MSG msg)
        {
            //op channel pass
            if (msg.message.StartsWith("op ") && msg.message.Split(' ').Length == 3)
            {
                if (oplist.ContainsKey(msg.from) && oplist[msg.from] == msg.message.Split(' ')[2])
                {
                    irc.SetChannelModes(msg.message.Split(' ')[1], "+o", msg.from);
                }
            }
        }

        public override void RemoveBindings()
        {
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(irc_OnPMRecvd);
            irc.OnCTCPRecvd -= new IrcClient.CTCPRecvd(irc_OnCTCPRecvd);
            irc.OnJoinRecvd -= new IrcClient.JoinRecvd(irc_OnJoinRecvd);
        }

        public override string GetName()
        {
            return "AutoOp";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
