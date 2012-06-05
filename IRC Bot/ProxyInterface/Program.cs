using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Encryption;

namespace ProxyInterface
{
    class Program
    {
        static List<KeyValuePair<Server, IrcBot>> bots;
        static Stream clientStream;
        static Encryption.Encryption sendEnc = new Encryption.Encryption();
        static Encryption.Encryption recvEnc = new Encryption.Encryption();

        static void Main(string[] args)
        {
            bots = new List<KeyValuePair<Server, IrcBot>>();
            Config.I.LoadServers();
            foreach (Server server in Config.I.servers)
            {
                IrcBot irc = new IrcBot(server);
                irc.StartBot();
                irc.newMessage += Value_newMessage;
                bots.Add(new KeyValuePair<Server, IrcBot>(server, irc));
            }
            TcpListener listener = new TcpListener(IPAddress.Any, 9809);
            listener.Start();
            while(true)
            {
                TcpClient t = listener.AcceptTcpClient();
                Thread thread = new Thread(new ParameterizedThreadStart(ClientThread));
                thread.Start(t);
            }
        }

        static void ClientThread(object o)
        {
            try
            {
                TcpClient s = (TcpClient)o;
                //Stream stream = new SslStream(s, false, new RemoteCertificateValidationCallback(CertificateValidationCallback));
                //((SslStream)stream).AuthenticateAsServer(new X509Certificate2("cert.pfx", "a"));
                Stream stream = s.GetStream();
                sendEnc.SetKey("fuckoffanddie");
                recvEnc.SetKey("fuckoffanddie");
                byte[] buff = new byte[1024];
                int recvd = stream.Read(buff, 0, 1024);
                buff = recvEnc.CryptData(buff, recvd);
                if (ASCIIEncoding.ASCII.GetString(buff, 0, recvd) != "bwall:lamepass")
                {                    
                    stream.Close();
                    s.Close();
                    return;
                }
                clientStream = stream;
                foreach (KeyValuePair<Server, IrcBot> ib in bots)
                {
                    foreach (RawMsg rm in ib.Value.backlog.ToArray())
                    {
                        string toSend = ib.Key.name + "\n" + rm.now.Ticks.ToString() + "\n" + rm.line;
                        byte[] ts = sendEnc.CryptData(Encoding.UTF8.GetBytes(toSend));
                        clientStream.Write(ts, 0, ts.Length);
                    }
                }
                while (s.Connected && recvd != 0)
                {
                    recvd = clientStream.Read(buff, 0, 1024);
                    buff = recvEnc.CryptData(buff, recvd);
                    string msg = Encoding.UTF8.GetString(buff, 0, recvd);
                    string server = msg.Substring(0, msg.IndexOf("\n"));
                    msg = msg.Substring(msg.IndexOf("\n") + 1);
                    string cmd = msg.Substring(0, msg.IndexOf("\n"));
                    msg = msg.Substring(msg.IndexOf("\n") + 1);
                    switch (cmd)
                    {
                        case "PRIVMSG":
                            foreach (KeyValuePair<Server, IrcBot> ib in bots)
                            {
                                if (ib.Key.name == server)
                                {
                                    string target = msg.Substring(0, msg.IndexOf("\n"));
                                    msg = msg.Substring(msg.IndexOf("\n") + 1);
                                    ib.Value.irc.SendMessage(target, msg);
                                    break;
                                }
                            }
                            break;
                    }
                }
                try
                {
                    ((TcpClient)o).Close();
                }
                catch { }
            }
            catch 
            {
                try
                {
                    ((TcpClient)o).Close();
                }
                catch { }
            }
        }

        static void Value_newMessage(RawMsg amsg, Server s)
        {
            try
            {
                if (clientStream != null)
                {
                    string toSend = s.name + "\n" + amsg.now.Ticks.ToString() + "\n" + amsg.line;
                    byte[] ts = sendEnc.CryptData(Encoding.UTF8.GetBytes(toSend));
                    clientStream.Write(ts, 0, ts.Length);
                }
            }
            catch { }
        }
    }
}
