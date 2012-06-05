using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Collections.Specialized;

namespace TitleBot
{
    public class IrcClient
    {
        string nick = "tempNick";
        string username = "tempUserName";
        string realname = "tempRealName";
        string hostname = "tempHostName";
        TcpClient client = null;
        Stream stream = null;
        Thread recvThread = null;

        public IrcClient()
        {

        }

        public IrcClient(string nick, string username, string realname, string hostname)
        {
            this.nick = nick;
            this.username = username;
            this.realname = realname;
            this.hostname = hostname;
        }

        public void StartBot(string domain, int port, bool ssl)
        {
            client = new TcpClient(domain, port);
            if(!client.Connected)
                return;
            if (ssl)
            {
                stream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(CertificateValidationCallback));
                ((SslStream)stream).AuthenticateAsClient(domain);
            }
            else
                stream = client.GetStream();
            stream.Write(ASCIIEncoding.ASCII.GetBytes("NICK " + nick + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("NICK " + nick + "\n"));
            string userMessage = "USER " + username + " " + hostname + " " + hostname + " :" + realname + "\n";
            stream.Write(ASCIIEncoding.ASCII.GetBytes(userMessage), 0, ASCIIEncoding.ASCII.GetByteCount(userMessage));
            recvThread = new Thread(new ThreadStart(ReceiveThread));
            recvThread.Start();
        }

        bool CertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void JoinChannel(string channel)
        {
            stream.Write(ASCIIEncoding.ASCII.GetBytes("JOIN " + channel + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("JOIN " + channel + "\n"));
        }

        public void SendMessage(string target, string message)
        {
            stream.Write(ASCIIEncoding.ASCII.GetBytes("PRIVMSG " + target + " :" + message + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("PRIVMSG " + target + " :" + message + "\n"));
        }

        void ReceiveThread()
        {
            byte[] receiveBuf = new byte[1024];
            while (true)
            {
                if (stream.Read(receiveBuf, 0, 1024) != 0)
                {
                    string receivedTotal = ASCIIEncoding.ASCII.GetString(receiveBuf).Replace("\0", "");
                    foreach (string received in receivedTotal.Split("\n".ToCharArray()))
                    {
                        if (received.StartsWith("PING :"))
                        {
                            stream.Write(ASCIIEncoding.ASCII.GetBytes("PONG :" + received.Substring("PING :".Length) + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("PONG :" + received.Substring("PING :".Length) + "\n"));
                        }
                        else if (received.StartsWith(":") && received.Contains("!"))
                        {
                            if (received.Contains("PRIVMSG"))
                            {
                                //PRIVMSG
                                string header = received.Substring(1, received.IndexOf(':', 1));
                                string message = received.Substring(received.IndexOf(':', 1) + 1);
                                string from = header.Substring(0, header.IndexOf('!'));
                                string fromUser = header.Substring(header.IndexOf('!') + 1, header.IndexOf('@'));
                                string fromHost = header.Substring(header.IndexOf('@') + 1, header.IndexOf(' '));
                                string to = header.Split(' ')[2];
                                if (to.Contains("#"))
                                {
                                    //Message to Channel
                                    if (OnMSGRecvd != null)
                                        OnMSGRecvd(new MSG(from, fromUser, fromHost, to, message));
                                }
                                else
                                {
                                    //Private Message
                                    if (OnPMRecvd != null)
                                        OnPMRecvd(new MSG(from, fromUser, fromHost, to, message));
                                }
                            }
                            else if (received.Contains("JOIN"))
                            {
                                //Response: :bwall!~crimsonre@kmk-14-694-071-019.rochester.res.rr.com JOIN :#chat
                                //Add channel to list of channels
                            }
                        }
                    }
                }
            }
        }

        public MSGRecvd OnPMRecvd;
        public MSGRecvd OnMSGRecvd;
        public delegate void MSGRecvd(MSG msg);

    }

    public class MSG
    {
        public string from;
        public string fromUser;
        public string fromHost;
        public string to;
        public string message;

        public MSG(string from, string fromUser, string fromHost, string to, string message)
        {
            this.from = from;
            this.fromHost = fromHost;
            this.fromUser = fromUser;
            this.to = to;
            this.message = message;
        }
    }
}
