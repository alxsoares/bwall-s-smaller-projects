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
using System.Text.RegularExpressions;

namespace ModularIrcBot
{
    public class IrcClient
    {
        public string nick = "tempNick";
        string username = "tempUserName";
        string realname = "tempRealName";
        string hostname = "tempHostName";
        TcpClient client = null;
        Stream stream = null;
        Thread recvThread = null;
        List<string> channels = new List<string>();
        bool quitOnPurpose = false;
        bool ready = false;
        string sslCertFolder = null;
        Server server = null;
        private Thread sendThread = null;
        private Queue<string> sendQueue = new Queue<string>();
        private object padlock = new object();

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

        public void StartBot(string domain, int port, bool ssl, string sslCertFolder, Server server)
        {
            this.server = server;
            this.sslCertFolder = sslCertFolder;
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

            if (sendThread == null)
            {
                sendThread = new Thread(new ThreadStart(SendThread));
                sendThread.Start();
            }
            if (recvThread == null)
            {
                recvThread = new Thread(new ThreadStart(ReceiveThread));
                recvThread.Start();
                int count = 0;
                while (!ready && client.Connected && count < 10)
                {
                    Thread.Sleep(500);
                    count++;
                }
            }
        }

        bool CertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            if (Directory.Exists(sslCertFolder))
            {
                foreach (string file in Directory.GetFiles(sslCertFolder))
                {
                    X509Certificate cert = new X509Certificate(file);
                    if (cert.Equals(certificate))
                        return true;
                }
                return false;
            }
            else
            {
                Directory.CreateDirectory(sslCertFolder);
                File.WriteAllBytes(sslCertFolder + Path.DirectorySeparatorChar + "cert", certificate.Export(X509ContentType.Cert));
                return true;
            }            
        }

        void SendThread()
        {
            while (true)
            {
                Thread.Sleep(1000);
                lock (padlock)
                {
                    if (sendQueue.Count > 0)
                    {
                        string send = sendQueue.Dequeue();
                        try
                        {
                            stream.Write(Encoding.UTF8.GetBytes(send + "\n"), 0, Encoding.UTF8.GetByteCount(send + "\n"));
                        }
                        catch { }
                    }
                }
            }
        }

        public void SendRaw(string command)
        {
            lock (padlock)
            {
                sendQueue.Enqueue(command);
            }
            //stream.Write(ASCIIEncoding.ASCII.GetBytes(command + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount(command + "\n"));
        }

        public void SetChannelModes(string channel, string modes, string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
                SendRaw("MODE " + channel + " " + modes);
            else
                SendRaw("MODE " + channel + " " + modes + " " + arguments);
        }

        public void KickUser(string channel, string user, string message)
        {
            lock (padlock)
            {
                if (channels.Contains(channel))
                    sendQueue.Enqueue("KICK " + channel + " " + user + " :" + message);
            }
                //stream.Write(ASCIIEncoding.ASCII.GetBytes("KICK " + channel + " " + user + " :" + message + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("KICK " + channel + " " + user + " :" + message + "\n"));
        }

        public void JoinChannel(string channel)
        {
            lock (padlock)
            {
                sendQueue.Enqueue("JOIN " + channel);
            }
            //stream.Write(ASCIIEncoding.ASCII.GetBytes("JOIN " + channel + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("JOIN " + channel + "\n"));
        }

        public void PartFromChannel(string channel, string message)
        {
            lock (padlock)
            {
                if (channels.Contains(channel))
                    sendQueue.Enqueue("PART " + channel);
            }
                //stream.Write(ASCIIEncoding.ASCII.GetBytes("PART " + channel), 0, ASCIIEncoding.ASCII.GetByteCount("PART " + channel));
        }

        public bool SendMessage(string target, string message)
        {
            lock (padlock)
            {
                sendQueue.Enqueue("PRIVMSG " + target + " :" + message);
                RawMsg rm = new RawMsg(nick + "!you@core PRIVMSG " + target + " :" + message);
                if (OnArchMsg != null)
                    OnArchMsg(rm);
            }
            //stream.Write(ASCIIEncoding.ASCII.GetBytes("PRIVMSG " + target + " :" + message + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("PRIVMSG " + target + " :" + message + "\n"));
            return true;
        }

        public void SendNotice(string target, string message)
        {
            lock (padlock)
            {
                sendQueue.Enqueue("NOTICE " + target + " :" + message);
            }
            //stream.Write(ASCIIEncoding.ASCII.GetBytes("NOTICE " + target + " :" + message + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("NOTICE " + target + " :" + message + "\n"));
        }

        public void SendMe(string target, string message)
        {
            SendCTCPRequest(target, "ACTION", message);
        }

        public void SendCTCPRequest(string target, string command, string message)
        {
            SendMessage(target, "\x01" + command + " " + message + "\x01");
        }

        public void SendCTCPResponse(string target, string command, string message)
        {
            SendNotice(target, "\x01" + command + " " + message + "\x01");
        }

        public void Quit(string message)
        {
            try
            {
                stream.Write(Encoding.UTF8.GetBytes("QUIT :" + message + "\n"), 0, Encoding.UTF8.GetByteCount("QUIT :" + message + "\n"));
                quitOnPurpose = true;
            }
            catch 
            {
                if (OnDisconnect != null)
                    OnDisconnect();
            }
        }

        void ReceiveThread()
        {
            byte[] receiveBuf = new byte[1024];
            string lastOverdraft = "";
            while (true)
            {
                if (!client.Connected)
                {
                    if (quitOnPurpose)
                        return;
                    else
                    {
                        if (OnDisconnect != null)
                            OnDisconnect();
                        return;
                    }
                }
                int c = 0;
                try
                {
                    c = stream.Read(receiveBuf, 0, 1024);
                }
                catch
                {
                    if (quitOnPurpose)
                        return;
                    else
                    {
                        if (OnDisconnect != null)
                            OnDisconnect();
                        return;
                    }
                }
                if (c != 0)
                {
                    string receivedTotal = lastOverdraft + Encoding.UTF8.GetString(receiveBuf, 0, c);
                    List<string> receivedLines = new List<string>();
                    for (int x = 0; x < receivedTotal.Length; x++)
                    {
                        if (receivedTotal[x] != "\n".ToCharArray()[0])
                        {
                            lastOverdraft += receivedTotal[x];
                        }
                        else
                        {
                            receivedLines.Add(lastOverdraft);
                            lastOverdraft = "";
                        }
                    }
                    foreach (string received in receivedLines)
                    {                        
                        try
                        {
                            if (received.StartsWith("PING :"))
                            {
                                stream.Write(ASCIIEncoding.ASCII.GetBytes("PONG :" + received.Substring("PING :".Length) + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("PONG :" + received.Substring("PING :".Length) + "\n"));
                            }
                            else if (received.Contains(":End of /MOTD command.") || received.Contains("MOTD File is missing"))
                            {
                                ready = true;
                            }
                            else if (received.StartsWith(":"))
                            {
                                if (received.Contains("!"))
                                {

                                    RawMsg msg = null;
                                    try
                                    {
                                        msg = new RawMsg(received.Replace("\r", "").Replace("\n", ""));
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }
                                    switch (msg.command)
                                    {
                                        case "PRIVMSG":
                                            {
                                                //PRIVMSG
                                                //string header = received.Substring(1, received.IndexOf(':', 1));
                                                //string message = received.Substring(received.IndexOf(':', 1) + 1).Replace("\r", "");
                                                //string from = header.Substring(0, header.IndexOf('!'));
                                                //string fromUser = header.Substring(header.IndexOf('!') + 1, header.IndexOf('@') - (header.IndexOf('!') + 1));
                                                //string fromHost = header.Substring(header.IndexOf('@') + 1, header.IndexOf(' ') - (header.IndexOf('@') + 1));
                                                //string to = header.Split(' ')[2];                                                
                                                if (msg.payload.StartsWith("\x01"))
                                                {
                                                    if (OnCTCPRecvd != null)
                                                        OnCTCPRecvd(new CTCP(msg));
                                                    if (OnArchMsg != null)
                                                        OnArchMsg(msg);
                                                }
                                                else if (msg.location.Contains("#"))
                                                {
                                                    //Message to Channel
                                                    if (OnMSGRecvd != null)
                                                        OnMSGRecvd(new MSG(msg));
                                                    if (OnArchMsg != null)
                                                        OnArchMsg(msg);
                                                }
                                                else
                                                {
                                                    //Private Message
                                                    if (OnPMRecvd != null)
                                                        OnPMRecvd(new MSG(msg));
                                                    if (OnArchMsg != null)
                                                        OnArchMsg(msg);
                                                }
                                                break;
                                            }
                                        case "NOTICE":
                                            {
                                                //NOTICE
                                                //string header = received.Substring(1, received.IndexOf(':', 1));
                                                //string message = received.Substring(received.IndexOf(':', 1) + 1).Replace("\r", "");
                                                //string from = header.Substring(0, header.IndexOf('!'));
                                                //string fromUser = header.Substring(header.IndexOf('!') + 1, header.IndexOf('@') - (header.IndexOf('!') + 1));
                                                //string fromHost = header.Substring(header.IndexOf('@') + 1, header.IndexOf(' ') - (header.IndexOf('@') + 1));
                                                //string to = header.Split(' ')[2];
                                                if (OnNoticeRecvd != null)
                                                    OnNoticeRecvd(new MSG(msg));
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                        case "JOIN":
                                            {
                                                //Response: :bwall!~crimsonre@kmk-14-694-071-019.rochester.res.rr.com JOIN :#chat
                                                //Add channel to list of channels
                                                if (msg.subject.nick == nick)
                                                    channels.Add(msg.payload);
                                                //string who = received.Substring(1, received.IndexOf('!') - 1);
                                                //string whoUser = received.Substring(received.IndexOf('!') + 1, received.IndexOf('@') - (received.IndexOf('!') + 1));
                                                //string whoHost = received.Substring(received.IndexOf('@') + 1, received.IndexOf(' ') - (received.IndexOf('@') + 1));
                                                //string channel = received.Substring(received.IndexOf(':', 1) + 1).Replace("\r", "");
                                                if (OnJoinRecvd != null)
                                                    OnJoinRecvd(new JoinMsg(msg));
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                        case "NICK":
                                            {
                                                //string who = received.Substring(1, received.IndexOf('!') - 1);
                                                //string whoUser = received.Substring(received.IndexOf('!') + 1, received.IndexOf('@') - (received.IndexOf('!') + 1));
                                                //string whoHost = received.Substring(received.IndexOf('@') + 1, received.IndexOf(' ') - (received.IndexOf('@') + 1));
                                                //string channel = received.Substring(received.IndexOf(':', 1) + 1).Replace("\r", "");
                                                if (OnNickRecvd != null)
                                                    OnNickRecvd(new JoinMsg(msg));
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                        case "QUIT":
                                            {
                                                //string who = received.Substring(1, received.IndexOf('!') - 1);
                                                if (msg.subject.nick == nick)
                                                {
                                                    if (OnDisconnect != null)
                                                        OnDisconnect();
                                                    client.Close();
                                                    return;
                                                }
                                                //string whoUser = received.Substring(received.IndexOf('!') + 1, received.IndexOf('@') - (received.IndexOf('!') + 1));
                                                //string whoHost = received.Substring(received.IndexOf('@') + 1, received.IndexOf(' ') - (received.IndexOf('@') + 1));
                                                //string message = received.Substring(received.IndexOf(':', 1) + 1).Replace("\r", "");
                                                if (OnQuitRecvd != null)
                                                    OnQuitRecvd(new JoinMsg(msg));
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                        case "PART":
                                            {
                                                if (msg.subject.nick == nick)
                                                    channels.Remove(msg.location);
                                                //string header = received.Substring(1);
                                                //string message = received.Substring(received.IndexOf(':', 1) + 1);
                                                //string from = header.Substring(0, header.IndexOf('!'));
                                                //string fromUser = header.Substring(header.IndexOf('!') + 1, header.IndexOf('@') - (header.IndexOf('!') + 1));
                                                //string fromHost = header.Substring(header.IndexOf('@') + 1, header.IndexOf(' ') - (header.IndexOf('@') + 1));
                                                //string to = header.Split(' ')[2];                                                
                                                if (OnPartRecvd != null)
                                                    OnPartRecvd(new MSG(msg));
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                        case "KICK":
                                            {
                                                if (msg.target == nick)
                                                    channels.Remove(msg.location);
                                                //string from = received.Substring(1, received.IndexOf('!') - 1);
                                                //string fromUser = received.Substring(received.IndexOf('!') + 1, received.IndexOf('@') - (received.IndexOf('!') + 1));
                                                //string fromHost = received.Substring(received.IndexOf('@') + 1, received.IndexOf(' ') - (received.IndexOf('@') + 1));
                                                //string message = "";
                                                if (OnKickRecvd != null)
                                                    OnKickRecvd(new KickMsg(msg));
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                        case "MODE":
                                            {
                                                //Do modes later
                                                if (OnArchMsg != null)
                                                    OnArchMsg(msg);
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    switch (received.Split(' ')[1])
                                    {
                                        case "433":
                                            stream.Write(ASCIIEncoding.ASCII.GetBytes("NICK " + server.othernick + "\n"), 0, ASCIIEncoding.ASCII.GetByteCount("NICK " + server.othernick + "\n"));
                                            break;
                                    }
                                }
                            }
                        }
                        catch(Exception e)
                        { 
                            Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Client error: (" + e.Message + ") Errored netcode: " + received); 
                        }
                    }
                }
            }
        }

        public event ArchRecvd OnArchMsg;
        public event JoinRecvd OnNickRecvd;
        public event MSGRecvd OnPMRecvd;
        public event MSGRecvd OnMSGRecvd;
        public event CTCPRecvd OnCTCPRecvd;
        public event JoinRecvd OnJoinRecvd;
        public event MSGRecvd OnPartRecvd;
        public event KickRecvd OnKickRecvd;
        public event MSGRecvd OnNoticeRecvd;
        public event JoinRecvd OnQuitRecvd;
        public event ThreadStart OnDisconnect;
        public delegate void CTCPRecvd(CTCP ctcp);
        public delegate void MSGRecvd(MSG msg);
        public delegate void JoinRecvd(JoinMsg join);
        public delegate void KickRecvd(KickMsg kick);
        public delegate void ArchRecvd(RawMsg amsg);

    }

    public class Subject
    {
        public string nick = null;
        public string user = null;
        public string host = null;

        public Subject()
        {


        }

        public Subject(string input)
        {
            if (input[0] == ':')
                input = input.Substring(1);
            int first = input.IndexOf('!');            
            nick = input.Substring(0, first);
            first++;
            int second = input.IndexOf('@');
            user = input.Substring(first, second - first);
            host = input.Substring(second + 1);
        }
    }

    [Serializable()]
    public class RawMsg
    {
        public Subject subject = new Subject();
        public string command = null;
        public string location = null;
        public string target = null;
        public string payload = null;
        public string header = null;
        public DateTime now = DateTime.Now;
        public string line;

        public RawMsg()
        {

        }

        public RawMsg(string line)
        {
            this.line = line;
            now = DateTime.Now;
            string temp = line;
            header = "";
            do
            {
                if (!temp.Contains(" "))
                {
                    header = line;
                    temp = null;
                    break;
                }
                header += temp.Substring(0, temp.IndexOf(' ') + 1);
                temp = temp.Substring(temp.IndexOf(' ') + 1);
            } while (!temp.StartsWith(":"));
            payload = temp.Substring(1);

            string[] splits = header.Split(' ');
            subject = new Subject(splits[0]);
            command = splits[1];
            if (splits.Length > 2)
            {
                location = splits[2];
                if(splits.Length > 3)
                    target = splits[3];
            }
        }
    }

    public class JoinMsg
    {
        RawMsg raw = null;
        public string who
        {
            get { return raw.subject.nick; }
            set { raw.subject.nick = value; }
        }
        public string whoUser
        {
            get { return raw.subject.user; }
            set { raw.subject.user = value; }
        }
        public string whoHost
        {
            get { return raw.subject.host; }
            set { raw.subject.host = value; }
        }
        public string channel
        {
            get { return raw.payload; }
            set { raw.payload = value; }
        }

        public JoinMsg(RawMsg raw)
        {
            this.raw = raw;
            //who = raw.subject.nick;
            //whoUser = raw.subject.user;
            //whoHost = raw.subject.host;
            //channel = raw.payload;
        }

        public JoinMsg(string who, string whoUser, string whoHost, string channel)
        {
            raw = new RawMsg();
            this.who = who;
            this.whoUser = whoUser;
            this.whoHost = whoHost;
            this.channel = channel;
        }
    }

    public class KickMsg
    {
        public RawMsg raw = null;

        public string from
        {
            get { return raw.subject.nick; }
            set { raw.subject.nick = value; }
        }
        public string fromUser
        {
            get { return raw.subject.user; }
            set { raw.subject.user = value; }
        }
        public string fromHost
        {
            get { return raw.subject.host; }
            set { raw.subject.host = value; }
        }
        public string to
        {
            get { return raw.target; }
            set { raw.target = value; }
        }
        public string channel
        {
            get { return raw.location; }
            set { raw.location = value; }
        }
        public string message
        {
            get { return raw.payload; }
            set { raw.payload = value; }
        }

        public KickMsg(RawMsg raw)
        {
            this.raw = raw;
        }

        public KickMsg(string from, string fromUser, string fromHost, string to, string channel, string message)
        {
            raw = new RawMsg();
            this.from = from;
            this.fromHost = fromHost;
            this.fromUser = fromUser;
            this.to = to;
            this.message = message;
            this.channel = channel;
        }
    }

    public class CTCP
    {
        public RawMsg raw = null;
        public string from
        {
            get { return raw.subject.nick; }
            set { raw.subject.nick = value; }
        }
        public string fromUser
        {
            get { return raw.subject.user; }
            set { raw.subject.user = value; }
        }
        public string fromHost
        {
            get { return raw.subject.host; }
            set { raw.subject.host = value; }
        }
        public string to
        {
            get { return raw.location; }
            set { raw.location = value; }
        }
        public string message
        {
            get
            {
                if (raw.payload.Contains(" "))
                {
                    int space = raw.payload.IndexOf(' ') + 1;
                    return raw.payload.Substring(space, raw.payload.Length - space - 1);
                }
                else
                    return null;
            }
            set { raw.payload = "\x01" + command + " " + value + "\x01"; }
        }
        public string command
        {
            get
            {
                if (raw.payload.Contains(" "))
                {
                    int space = raw.payload.IndexOf(' ') - 1;
                    return raw.payload.Substring(0, space);
                }
                else
                    return raw.payload.Substring(1, raw.payload.Length - 2);
            }
            set
            {
                if (raw.payload.Contains(" "))
                    raw.payload = "\x01" + value + " " + message + "\x01";
                else
                    raw.payload = "\x01" + value + "\x01";
            }
        }
        public DateTime when
        {
            get { return raw.now; }
            set { raw.now = value; }
        }

        public CTCP(RawMsg raw)
        {
            this.raw = raw;
        }

        public CTCP(string line)
        {
            raw = new RawMsg();
            Regex msg = new Regex(@"(\[)(?<date>.*?)(\]\()(?<channel>.*?)(\)<)(?<nick>.*?)(> |)(?<action>.*?)(|)(?<message>.*)");
            Match m = msg.Match(line);
            if (m.Success)
            {
                from = m.Groups["nick"].Value;
                to = m.Groups["channel"].Value;
                message = m.Groups["message"].Value;
                when = DateTime.Parse(m.Groups["date"].Value);
                command = m.Groups["action"].Value;
            }
        }

        public CTCP(string from, string fromUser, string fromHost, string to, string command, string message)
        {
            raw = new RawMsg();
            when = DateTime.Now;
            this.from = from;
            this.fromHost = fromHost;
            this.fromUser = fromUser;
            this.to = to;
            this.message = message;
            this.command = command;
        }

        public override string ToString()
        {
            return "[" + when.ToString() + "](" + to + ")<" + from + "> |" + command + "|" + message;
        }
    }

    public class MSG : IComparable<MSG>
    {
        public RawMsg raw = null;
        public string from
        {
            get { return raw.subject.nick; }
            set { raw.subject.nick = value; }
        }
        public string fromUser
        {
            get { return raw.subject.user; }
            set { raw.subject.user = value; }
        }
        public string fromHost
        {
            get { return raw.subject.host; }
            set { raw.subject.host = value; }
        }
        public string to
        {
            get { return raw.location; }
            set { raw.location = value; }
        }
        public string message
        {
            get { return raw.payload; }
            set { raw.payload = value; }
        }
        public DateTime when
        {
            get { return raw.now; }
            set { raw.now = value; }
        }

        public MSG(RawMsg raw)
        {
            this.raw = raw;
        }

        public MSG(string line)
        {
            raw = new RawMsg();
            Regex msg = new Regex(@"(\[)(?<date>.*?)(\]\()(?<channel>.*?)(\)<)(?<nick>.*?)(> )(?<message>.*)");
            Match m = msg.Match(line);
            if (m.Success)
            {
                from = m.Groups["nick"].Value;
                to = m.Groups["channel"].Value;
                message = m.Groups["message"].Value;
                when = DateTime.Parse(m.Groups["date"].Value);
            }
        }

        public MSG(string from, string fromUser, string fromHost, string to, string message)
        {
            raw = new RawMsg();
            when = DateTime.Now;
            this.from = from;
            this.fromHost = fromHost;
            this.fromUser = fromUser;
            this.to = to;
            this.message = message;
        }

        public override string ToString()
        {
            return "[" + when.ToString() + "](" + to + ")<" + from + "> " + message;
        }

        public int CompareTo(MSG other)
        {
            return when.CompareTo(other.when);
        }
    }
}
