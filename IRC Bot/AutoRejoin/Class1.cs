using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;

namespace AutoRejoin
{
    public class AutoRejoin : Module
    {
        public override void AddBindings()
        {
            irc.OnKickRecvd += new IrcClient.KickRecvd(irc_OnKickRecvd);
        }

        void irc_OnKickRecvd(KickMsg kick)
        {
            irc.JoinChannel(kick.channel);
        }

        public override void RemoveBindings()
        {
            irc.OnKickRecvd -= new IrcClient.KickRecvd(irc_OnKickRecvd);
        }

        public override string GetName()
        {
            return "AutoRejoin";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
