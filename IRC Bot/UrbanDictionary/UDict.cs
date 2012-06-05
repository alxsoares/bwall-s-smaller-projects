using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;

namespace UrbanDictionary
{
    class UrbanDictionary : Module
    {
        void CheckMessage(MSG msg)
        {
            if (msg.message.StartsWith(".udict") && msg.message.Split(' ').Length > 1)
            {
                Thread t = new Thread(new ParameterizedThreadStart(GetUDef));
                t.Start(msg);
            }
        }

        string StripHref(string input)
        {
            Regex link = new Regex("(a href=.*?>)(?<text>.*?)(</a>)");
            foreach (Match match in link.Matches(input))
            {
                input = input.Replace(match.Value, match.Groups["text"].Value);
            }
            return input;
        }

        void GetUDef(object o)
        {
            try
            {
                MSG msg = (MSG)o;
                WebClient wc = new WebClient();
                string str = wc.DownloadString("http://www.urbandictionary.com/define.php?term=" + msg.message.Split(' ')[1]);
                Regex defReg = new Regex(@"(div class=""definition"">)(?<def>.*?)(</div>)");
                int count = 0;
                foreach (Match match in defReg.Matches(str))
                {
                    count++;
                    string def = StripHref(match.Groups["def"].Value).Replace("<br/>", "\n").Replace("\r", "");
                    string[] lines = def.Split("\n".ToCharArray());
                    foreach (string line in lines)
                    {
                        irc.SendMessage(msg.to, count.ToString() + ". " + line);
                        Thread.Sleep(500);
                    }
                }
            }
            catch { }
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
            return "UrbanDictionary";
        }

        public override string GetHelp()
        {
            return "Gets the first definition from UrbanDictionary.com with \".udict <word>\"";
        }

        public override string GetSpecificHelp(string category)
        {
            return null;
        }

        public override string GetCommands()
        {
            return ".udict";
        }
    }
}
