using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;

namespace CTCPVersion
{
    public class CTCPVersion : Module
    {
        void RespondToCTCP(MSG msg)
        {
            if (msg.message == "\x01VERSION\x01")
                irc.SendNotice(msg.from, "\x01VERSION xchat 2.8.8 Ubuntu\x01");
        }

        public override void AddBindings()
        {
            irc.OnPMRecvd += new IrcClient.MSGRecvd(RespondToCTCP);
            //irc.OnNoticeRecvd += new IrcClient.MSGRecvd(RespondToCTCP);
        }

        public override void RemoveBindings()
        {
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(RespondToCTCP);
            //irc.OnNoticeRecvd -= new IrcClient.MSGRecvd(RespondToCTCP);
        }

        public override string GetName()
        {
            return "CTCPV";
        }

        public override string GetHelp()
        {
            return "It does it all on its own.";
        }
    }
}
