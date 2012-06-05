using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.Web;
using System.Net;
using System.IO;

namespace TwitterUpdate
{
    public class Twatter : Module
    {
        Thread t;
        string xlastID = "";
        string blastID = "";

        public override void AddBindings()
        {
            try
            {
                if (File.Exists(server.Data + Path.DirectorySeparatorChar + "twitterID.fox"))
                {
                    xlastID = File.ReadAllText(server.Data + Path.DirectorySeparatorChar + "twitterID.fox");
                }
                if (File.Exists(server.Data + Path.DirectorySeparatorChar + "twitterID.bwall"))
                {
                    blastID = File.ReadAllText(server.Data + Path.DirectorySeparatorChar + "twitterID.bwall");
                }
            }
            catch { }
            t = new Thread(new ThreadStart(t_Elapsed));
            t.Start();
        }

        void t_Elapsed()
        {
            while (true)
            {
                Thread.Sleep(15000);
                try
                {
                    WebClient wc = new WebClient();
                    string response = wc.DownloadString("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=xMused&count=1");
                    response = response.Substring(response.IndexOf("<id>") + "<id>".Length);
                    string thisID = response.Substring(0, response.IndexOf("</id>"));
                    if (thisID != xlastID)
                    {
                        File.WriteAllText(server.Data + Path.DirectorySeparatorChar + "twitterID.fox", thisID);
                        xlastID = thisID;
                        response = response.Substring(response.IndexOf("<text>") + "<text>".Length);
                        string text = response.Substring(0, response.IndexOf("</text>"));
                        irc.SendMessage("#neworder", "xMused: " + text);
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
                Thread.Sleep(45000);
                Thread.Sleep(15000);
                try
                {
                    WebClient wc = new WebClient();
                    string response = wc.DownloadString("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=bwallHatesTwits&count=1");
                    response = response.Substring(response.IndexOf("<id>") + "<id>".Length);
                    string thisID = response.Substring(0, response.IndexOf("</id>"));
                    if (thisID != blastID)
                    {
                        File.WriteAllText(server.Data + Path.DirectorySeparatorChar + "twitterID.bwall", thisID);
                        blastID = thisID;
                        response = response.Substring(response.IndexOf("<text>") + "<text>".Length);
                        string text = response.Substring(0, response.IndexOf("</text>"));
                        irc.SendMessage("#neworder", "schizo: " + text);
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
                Thread.Sleep(45000);
            }
        }

        public override void RemoveBindings()
        {
            t.Abort();
        }

        public override string GetName()
        {
            return "Twitter Updater";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
