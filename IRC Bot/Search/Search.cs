using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.Net;
using System.Xml;

namespace Search
{
    public class Search : Module
    {
        string google_api_key = "AIzaSyCvheoYpOa5mMgyI2uukME67SEVtgmr0mc";

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        void irc_OnMSGRecvd(MSG msg)
        {
            if (msg.message.StartsWith(".youtube "))
            {
                Thread t = new Thread(new ParameterizedThreadStart(YoutubeSearch));
                t.Start(msg);
            }
            else if (msg.message.StartsWith(".google ") || msg.message.StartsWith(".g "))
            {
                //Thread t = new Thread(new ParameterizedThreadStart(GoogleSearch));
                //t.Start(msg);
            }
        }

        void GoogleSearch(object o)
        {
            MSG msg = new MSG(((MSG)o).ToString());
            msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
            try
            {                
                WebClient wc = new WebClient();
                string str = wc.DownloadString("http://www.google.com/search?q=" + Uri.EscapeUriString(msg.message) + "&num=5&client=google-csbe&output=xml&cx=004809591179083466614:t_ax44dljjk");
                Console.WriteLine(str);
                Console.WriteLine();

            }
            catch { }
        }

        void YoutubeSearch(object o)
        {
            try
            {
                MSG msg = new MSG(((MSG)o).ToString());
                WebClient wc = new WebClient();
                string str = wc.DownloadString("http://gdata.youtube.com/feeds/api/videos?fields-language=r2&q=" + Uri.EscapeUriString(msg.message.Substring(".youtube ".Length)) + "&fields=entry(media:group(media:title,media:content[@type='application/x-shockwave-flash'](@url)))&max-results=3");
                XmlDocument doc = new XmlDocument();
                str = str.Substring(str.IndexOf('>') + 1).Replace("media:", "");
                str = "<feed>" + str.Substring(str.IndexOf('>') + 1);
                doc.LoadXml(str);
                XmlNodeList nodeList = doc.DocumentElement.SelectNodes("//group");
                foreach (XmlNode node in nodeList)
                {
                    if (node.ChildNodes[0].Attributes["url"] != null && !string.IsNullOrEmpty(node.ChildNodes[1].InnerText))
                    {
                        string url = node.ChildNodes[0].Attributes["url"].Value;
                        url = url.Substring(0, url.IndexOf('?'));
                        url = url.Substring(url.Length - (11));
                        irc.SendMessage(msg.to, "(" + msg.message.Substring(".youtube ".Length) + ") * http://www.youtube.com/watch?v=" + url + " * - " + node.ChildNodes[1].InnerText);
                    }
                }
            }
            catch { }
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        public override string GetName()
        {
            return "Search";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
