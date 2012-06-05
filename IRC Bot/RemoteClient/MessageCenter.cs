using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Encryption;

namespace RemoteClient
{
    public class MSG : IComparable<MSG>
    {
        public DateTime time;
        public string msg;

        public MSG()
        {
            time = DateTime.Now;
        }

        public MSG(DateTime t, string m)
        {
            time = t;
            msg = m;
            RawMsg(msg);
        }

        public class Subject
        {
            public string nick = null;
            public string user = null;
            public string host = null;
            public string input = null;

            public Subject()
            {


            }

            public Subject(string input)
            {             
                if (input[0] == ':')
                    input = input.Substring(1);
                this.input = input;
                int first = input.IndexOf('!');
                nick = input.Substring(0, first);
                first++;
                int second = input.IndexOf('@');
                user = input.Substring(first, second - first);
                host = input.Substring(second + 1);
            }

            public override string ToString()
            {
                return input;
            }
        }

        public Subject subject = new Subject();
        public string command = null;
        public string location = null;
        public string target = null;
        public string payload = null;
        public string header = null;
        public string line;

        void RawMsg(string line)
        {
            this.line = line;
            int headerLength = line.IndexOf(':', 1);
            if (headerLength > 1)
            {
                header = line.Substring(0, headerLength);
                payload = line.Substring(headerLength + 1);
            }
            else
            {
                //no payload
                header = line;
            }
            string[] splits = header.Split(' ');
            subject = new Subject(splits[0]);
            command = splits[1];
            if (splits.Length > 2)
            {
                location = splits[2];
                if (splits.Length > 3)
                    target = splits[3];
            }
        }

        public int CompareTo(MSG other)
        {
            return time.CompareTo(other.time);
        }

        public override string ToString()
        {
            switch (command)
            {
                case "PRIVMSG":
                    return time.ToLongTimeString() + "\t" + subject.nick + ": " + payload;
                case "NOTICE":
                    return time.ToLongTimeString() + "\t" + subject.nick + "(NOTICE): " + payload;
                case "JOIN":
                    return time.ToLongTimeString() + "\t" + subject.nick + " joined";
                case "NICK":
                    return time.ToLongTimeString() + "\t" + subject.nick + " is now " + target;
                case "QUIT":
                    return time.ToLongTimeString() + "\t" + subject.nick + " quit: " + payload;
                case "PART":
                    return time.ToLongTimeString() + "\t" + subject.nick + " parted";
                case "KICK":
                    return time.ToLongTimeString() + "\t" + target + " was kicked";
                case "MODE":
                    return time.ToLongTimeString() + "\t(MODE) " + target + " " + payload;
            }
            return "";
        }
    }

    public class MessageCenter
    {
        Dictionary<string, Dictionary<string, List<MSG>>> log = new Dictionary<string, Dictionary<string, List<MSG>>>();
        Thread recvThread;
        Stream clientStream;
        public delegate void NewMessage(string server, string channel);
        public event NewMessage nMessage;
        Encryption.Encryption sendEnc = new Encryption.Encryption();
        Encryption.Encryption recvEnc = new Encryption.Encryption();

        public string server;
        public string user;
        public string password;

        public bool Connect()
        {
            try
            {
                TcpClient client = new TcpClient(server, 9809);
                //SslStream stream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(CertificateValidationCallback));
                //stream.AuthenticateAsClient(server);
                Stream stream = client.GetStream();
                string send = user + ":" + password;
                sendEnc.SetKey("fuckoffanddie");
                recvEnc.SetKey("fuckoffanddie");
                byte[] ts = sendEnc.CryptData(Encoding.UTF8.GetBytes(send));
                stream.Write(ts, 0, ts.Length);
                clientStream = stream;
                recvThread = new Thread(new ThreadStart(rt));
                recvThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            try { clientStream.Close(); }
            catch { }
            try { recvThread.Abort(); }
            catch { }
        }

        void rt()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[2048];
                        int len = clientStream.Read(buffer, 0, 2048);
                        buffer = recvEnc.CryptData(buffer, len);
                        string o = Encoding.UTF8.GetString(buffer, 0, len);
                        string server = o.Substring(0, o.IndexOf("\n"));
                        o = o.Substring(o.IndexOf("\n") + 1);
                        DateTime dt = new DateTime(long.Parse(o.Substring(0, o.IndexOf("\n"))));
                        o = o.Substring(o.IndexOf("\n") + 1);
                        MSG m = new MSG(dt, o);
                        if (m.command == "PRIVMSG")
                        {
                            if (m.location.StartsWith("#"))
                            {
                                //its to a channel
                                if (!log.ContainsKey(server))
                                {
                                    log[server] = new Dictionary<string, List<MSG>>();
                                }
                                if (!log[server].ContainsKey(m.location))
                                {
                                    log[server][m.location] = new List<MSG>();
                                }
                                log[server][m.location].Add(m);
                                log[server][m.location].Sort();
                                if (nMessage != null)
                                {
                                    nMessage(server, m.location);
                                }
                            }
                            else
                            {
                                //its a pm
                                if (!log.ContainsKey(server))
                                {
                                    log[server] = new Dictionary<string, List<MSG>>();
                                }
                                if (!log[server].ContainsKey(m.subject.ToString()))
                                {
                                    log[server][m.subject.ToString()] = new List<MSG>();
                                }
                                log[server][m.subject.ToString()].Add(m);
                                log[server][m.subject.ToString()].Sort();
                                if (nMessage != null)
                                {
                                    nMessage(server, m.subject.ToString());
                                }
                            }
                        }
                        else
                        {
                            //its anything else?
                            if (!log.ContainsKey(server))
                            {
                                log[server] = new Dictionary<string, List<MSG>>();
                            }
                            if (!log[server].ContainsKey(m.payload))
                            {
                                log[server][m.payload] = new List<MSG>();
                            }
                            log[server][m.payload].Add(m);
                            log[server][m.payload].Sort();
                            if (nMessage != null)
                            {
                                nMessage(server, m.payload);
                            }
                        }
                    }
                    catch { }
                }                
            }
            catch { }
        }

        public TreeNode GetServerList()
        {
            TreeNode root = new TreeNode();
            foreach (string server in log.Keys)
            {
                TreeNode s = new TreeNode(server);
                s.Text = server;
                foreach (string chan in log[server].Keys)
                {
                    TreeNode c = new TreeNode(chan);
                    c.Text = chan;
                    s.Nodes.Add(c);
                }
                root.Nodes.Add(s);
            }
            return root;
        }

        public string GetChanText(string server, string chan)
        {
            string ret = "";
            foreach (MSG m in log[server][chan])
            {
                ret += m.ToString() + "\r\n";
            }
            return ret;
        }

        public void SendCommand(string server, string channel, string msg)
        {
            string tosend = server + "\n" + "PRIVMSG" + "\n" + channel + "\n" + msg;
            byte[] bts = Encoding.UTF8.GetBytes(tosend);
            bts = sendEnc.CryptData(bts);
            clientStream.Write(bts, 0, bts.Length);
        }

        bool CertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
