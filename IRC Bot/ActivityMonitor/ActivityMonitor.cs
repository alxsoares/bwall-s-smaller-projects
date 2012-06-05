using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using GraphGenerator;
using System.IO;

namespace ActivityMonitor
{
    public class ActivityMonitor : Module
    {
        Dictionary<string, List<uint>> channelOverall = new Dictionary<string, List<uint>>();
        Dictionary<string, List<uint>> channelToday = new Dictionary<string, List<uint>>();
        Dictionary<string, Dictionary<string, List<uint>>> chanNickOverall = new Dictionary<string, Dictionary<string, List<uint>>>();
        Dictionary<string, Dictionary<string, List<uint>>> chanNickToday = new Dictionary<string, Dictionary<string, List<uint>>>();
        DateTime lastMsgRecvd = DateTime.Now;

        void ProcessMessage(MSG msg)
        {
            if (msg.when.Hour != lastMsgRecvd.Hour)
            {
                foreach (string key in channelToday.Keys)
                {
                    channelToday[key][msg.when.Hour] = 0;
                }
                foreach (string key in chanNickToday.Keys)
                {
                    foreach (string value in chanNickToday[key].Keys)
                    {
                        chanNickToday[key][value][msg.when.Hour] = 0;
                    }
                }
            }
            lastMsgRecvd = msg.when;
            if(!channelOverall.ContainsKey(msg.to))
            {
                List<uint> list = new List<uint>();
                for(int x = 0; x < 24; x++)
                    list.Add(0);
                channelOverall.Add(msg.to, list);
            }
            if (!channelToday.ContainsKey(msg.to))
            {
                List<uint> list = new List<uint>();
                for (int x = 0; x < 24; x++)
                    list.Add(0);
                channelToday.Add(msg.to, list);
            }

            if (!chanNickOverall.ContainsKey(msg.to))
            {
                chanNickOverall.Add(msg.to, new Dictionary<string, List<uint>>());
            }
            if (!chanNickOverall[msg.to].ContainsKey(msg.from))
            {
                List<uint> list = new List<uint>();
                for (int x = 0; x < 24; x++)
                    list.Add(0);
                chanNickOverall[msg.to].Add(msg.from, list);
            }
            if (!chanNickToday.ContainsKey(msg.to))
            {
                chanNickToday.Add(msg.to, new Dictionary<string, List<uint>>());
            }
            if (!chanNickToday[msg.to].ContainsKey(msg.from))
            {
                List<uint> list = new List<uint>();
                for (int x = 0; x < 24; x++)
                    list.Add(0);
                chanNickToday[msg.to].Add(msg.from, list);
            }
            channelOverall[msg.to][msg.when.Hour]++;
            channelToday[msg.to][msg.when.Hour]++;
            chanNickOverall[msg.to][msg.from][msg.when.Hour]++;
            chanNickToday[msg.to][msg.from][msg.when.Hour]++;
        }

        public ActivityMonitor()
        {
        }

        public override void WriteStatsToWWW()
        {
            WriteStatsToWWW(server.WWW);
        }

        public void WriteStatsToWWW(string folder)
        {
            foreach (string chan in channelToday.Keys)
            {
                //ChannelToday Graphic
                uint max = 0;
                foreach (uint u in channelToday[chan])
                {
                    if(u > max)
                        max = u;
                }
                Directory.CreateDirectory(folder);
                GraphGenerator.GraphGenerator.GenerateLineGraph(800, 200, max, channelToday[chan].ToArray(), folder + Path.DirectorySeparatorChar + "t_" + chan + ".bmp");
            }
            foreach (string chan in channelOverall.Keys)
            {
                //ChannelOverall Graphic
                uint max = 0;
                foreach (uint u in channelOverall[chan])
                {
                    if (u > max)
                        max = u;
                }
                Directory.CreateDirectory(folder);
                GraphGenerator.GraphGenerator.GenerateLineGraph(800, 200, max, channelOverall[chan].ToArray(), folder + Path.DirectorySeparatorChar + "o_" + chan + ".bmp");
            }
        }

        public override void Save()
        {
            string fileout = "";
            foreach (KeyValuePair<string, List<uint>> times in channelOverall)
            {
                fileout += times.Key;
                foreach (ulong time in times.Value)
                {
                    fileout += " " + time.ToString();
                }
                fileout += "\n";
            }
            foreach (KeyValuePair<string, Dictionary<string, List<uint>>> timess in chanNickOverall)
            {
                foreach (KeyValuePair<string, List<uint>> times in timess.Value)
                {
                    fileout += timess.Key + " " + times.Key;
                    foreach (uint time in times.Value)
                    {
                        fileout += " " + time.ToString();
                    }
                    fileout += "\n";
                }
            }
            Directory.CreateDirectory(server.Data);
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "ActivityMonitor.db", fileout);
        }

        public override void Load()
        {
            chanNickOverall.Clear();
            channelOverall.Clear();
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "ActivityMonitor.db"))
            {
                foreach(string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "ActivityMonitor.db"))
                {
                    string[] s = line.Split(' ');
                    if (s.Length == 25)
                    {
                        List<uint> list = new List<uint>();
                        for (int x = 1; x < 25; x++)
                            list.Add(uint.Parse(s[x]));
                        channelOverall[s[0]] = list;
                    }
                    else
                    {
                        if (!chanNickOverall.ContainsKey(s[0]))
                            chanNickOverall[s[0]] = new Dictionary<string, List<uint>>();
                        List<uint> list = new List<uint>();
                        for (int x = 2; x < 26; x++)
                            list.Add(uint.Parse(s[x]));
                        chanNickOverall[s[0]][s[1]] = list;
                    }
                }
            }
        }

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(ProcessMessage);
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(ProcessMessage);
        }

        public override string GetName()
        {
            return "ActivityMonitor";
        }

        public override string GetHelp()
        {
            return "Takes statistics as to when channels/users are most active.";
        }
    }
}
