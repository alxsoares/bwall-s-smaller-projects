using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Net;
using System.Collections;

namespace BrakMeme
{
    public class BrakMeme : Module
    {
        Dictionary<string, DateTime> lastMeme = new Dictionary<string, DateTime>();
        Random rand = new Random();

        public override void AddBindings()
        {
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(bypassAllCertificateStuff);
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        void irc_OnMSGRecvd(MSG msg)
        {
            if (msg.message.ToLower().StartsWith(".decode "))
            {
                lastMeme[msg.fromUser + '@' + msg.fromHost] = DateTime.Now;
                Thread t = new Thread(new ParameterizedThreadStart(GenerateImage));
                t.Start(msg);
            }
        }

        void GenerateImage(object str)
        {
            MSG msg = (MSG)str;
            msg.message = msg.message.Replace(".decode ", "");
            WebClient wc = new WebClient();
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
            nvc.Add("url", msg.message);
            wc.Headers.Set(HttpRequestHeader.UserAgent, "buckey-11");
            string ret = Encoding.ASCII.GetString(wc.UploadValues("https://firebwall.com/decoding/index.php", "POST", nvc));
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("<META HTTP-EQUIV=REFRESH CONTENT=\"1; URL=(?<URL>[^\"]+)\"");
            foreach (string line in ret.Split("\n".ToCharArray()))
            {
                if (regex.IsMatch(line))
                {
                    System.Text.RegularExpressions.Match m = regex.Match(line);
                    irc.SendMessage(msg.to, "Decoded at: " + m.Groups["URL"]);
                    return;
                }
            }
            irc.SendMessage(msg.to, "Failed to get decoded results");
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        public override string GetName()
        {
            return "Decoder";
        }

        public override string GetHelp()
        {
            return null;
        }

        private static bool bypassAllCertificateStuff(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }
    }
}
