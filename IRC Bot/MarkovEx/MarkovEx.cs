using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ZZStructure;

namespace MarkovEx
{
    public class MarkovEx : Module
    {
        ZZStructure.ZZStructure zzn;

        Dictionary<string, List<KeyValuePair<int, ushort>>> words = new Dictionary<string, List<KeyValuePair<int, ushort>>>();
        Dictionary<int, KeyValuePair<string, KeyValuePair<uint, List<int>>>> lines = new Dictionary<int, KeyValuePair<string, KeyValuePair<uint, List<int>>>>();
        int lastLine = 0;
        MSG lastMSG = null;
        Random rand = new Random();
        string ReportTo = null;
        int TalkValue = 100;

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(irc_OnMSGRecvd);
            irc.OnPMRecvd += new IrcClient.MSGRecvd(irc_OnPMRecvd);
        }

        public override void AuthedCommand(MSG msg, int operLevel)
        {
            ReportTo = msg.from;
            msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
            if (msg.message.StartsWith("TalkValue "))
            {
                TalkValue = int.Parse(msg.message.Substring(msg.message.IndexOf(' ') + 1));
            }
            if (msg.message.StartsWith("value "))
            {
                string s = GetPrivateValue(msg.message.Substring(msg.message.IndexOf(' ') + 1));
                if (!string.IsNullOrEmpty(s))
                    irc.SendMessage(msg.from, msg.message.Substring(msg.message.IndexOf(' ') + 1) + " = " + s);
            }
        }

        void irc_OnPMRecvd(MSG msg)
        {
            try
            {
                if (msg.message == "MarkovReply")
                {
                    if (lastMSG != null)
                    {
                        string s = zzn.GenerateResponse(lastMSG.message);
                        if (string.IsNullOrEmpty(s))
                            return;
                        irc.SendMessage(msg.from, "MarkovEx - " + lastMSG.from + " " + lastMSG.message);
                        irc.SendMessage(msg.from, s);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        void irc_OnMSGRecvd(MSG msg)
        {
            Thread t = new Thread(new ParameterizedThreadStart(ProcessMessage));
            t.Start(msg);
        }

        public override void Save()
        {
            FileStream stream = File.Open(server.Data + Path.DirectorySeparatorChar + "zzmarkov.dat", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, zzn);
            stream.Close();
        }

        public override void Load()
        {
            if (!File.Exists(server.Data + Path.DirectorySeparatorChar + "zzmarkov.dat"))
            {
                zzn = new ZZStructure.ZZStructure();
                return;
            }
            FileStream stream = File.Open(server.Data + Path.DirectorySeparatorChar + "zzmarkov.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryFormatter bFormatter = new BinaryFormatter();
            zzn = (ZZStructure.ZZStructure)bFormatter.Deserialize(stream);
            stream.Close();
        }

        void ProcessMessage(object o)
        {
            MSG msg = new MSG(((MSG)o).ToString());
            lastMSG = msg;
            msg.message = msg.message.ToLower().Replace("\"", "").Replace("\r", "").Replace("\n", "").Replace(";", "").Replace("!", "").Replace("?", "").Replace(".", "").Replace(",", "").Replace("'", "").Replace(":", "");
            zzn.AddSentence(msg.message);

            if (rand.Next(100) < TalkValue)
            {
                string s = zzn.GenerateResponse(msg.message);
                if (string.IsNullOrEmpty(s))
                    return;
                irc.SendMessage(msg.to, s);
            }
        }        

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(irc_OnMSGRecvd);
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(irc_OnPMRecvd);
        }

        public override string GetName()
        {
            return "MarkovEx";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
