using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.IO;

namespace Markov
{
    public class Markov : Module
    {
        Dictionary<string, List<KeyValuePair<int, ushort>>> words = new Dictionary<string, List<KeyValuePair<int, ushort>>>();
        Dictionary<int, KeyValuePair<string, uint>> lines = new Dictionary<int, KeyValuePair<string, uint>>();
        MSG lastMSG = null;
        Random rand = new Random();
        string ReportTo = null;
        int TalkValue = 5;

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(irc_OnMSGRecvd);
            irc.OnPMRecvd += new IrcClient.MSGRecvd(irc_OnPMRecvd);
        }

        public override void  AuthedCommand(MSG msg, int operLevel)
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
                        string s = Reply(lastMSG);
                        if (string.IsNullOrEmpty(s))
                            return;
                        irc.SendMessage(msg.from, "Markov - " + lastMSG.from + " " + lastMSG.message);
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
            string toFile = "";
            lock (words)
            {
                foreach (KeyValuePair<string, List<KeyValuePair<int, ushort>>> word in words)
                {
                    toFile += word.Key;
                    foreach (KeyValuePair<int, ushort> pair in word.Value)
                    {
                        toFile += " " + pair.Key.ToString() + " " + pair.Value.ToString();
                    }
                    toFile += "\n";
                }
            }
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "words.dat", toFile);

            toFile = "";
            lock (lines)
            {
                foreach (KeyValuePair<int, KeyValuePair<string, uint>> line in lines)
                {
                    toFile += line.Value.Value.ToString() + ":" + line.Value.Key + "\n";
                }
            }
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "lines.dat", toFile);
        }

        public override void Load()
        {
            words.Clear();
            lines.Clear();
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "lines.dat") && false)
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "lines.dat"))
                {
                    string str = line.Substring(line.IndexOf(':') + 1);
                    uint num = uint.Parse(line.Substring(0, line.IndexOf(':')));
                    for (uint x = 0; x < num; x++)
                        Learn(str);
                }                
            }
            else
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "logfile.txt"))
                {
                    if (line.Contains("antiroach>") || line.Contains("peterzal>"))
                    {
                        ProcessMessage(new MSG("", "", "", "", line.Substring(line.IndexOf('>') + 2)));
                    }
                }
            }
        }

        void ProcessMessage(object o)
        {
            MSG msg = new MSG(((MSG)o).ToString());
            lastMSG = msg;
            //msg.message = msg.message.ToLower().Replace("\"", "").Replace("\r", "").Replace("\n", "").Replace(";", "").Replace("!", "").Replace("?", "").Replace(".", "").Replace(",", "").Replace("'", "").Replace(":", "");
            msg.message += " ";
            Learn(msg.message);

            if (rand.Next() % 100 < TalkValue)
            {
                string s = Reply(msg);
                if (string.IsNullOrEmpty(s) || s.Contains("lol"))
                    return;
                irc.SendMessage(msg.to, s);
                //irc.SendMessage(ReportTo, "Markov - " + s);
            }
        }

        void Learn(string msg)
        {
            string[] words = msg.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            string str = "";
            /*
             * 	#if one word as more than 13 characters, don't learn
                #		( in french, this represent 12% of the words )
                #and d'ont learn words where there are less than 25% of voyels
                #don't learn the sentence if one word is censored
                #don't learn too if there are digits and char in the word
                #same if learning is off
            * */
            for (int x = 0; x < words.Length; x++)
            {
                if (words[x].Length > 13 || words[x].Contains("-") || words[x].Contains("_"))
                    words[x] = "#nick";
                else if (!string.IsNullOrEmpty(words[x]) && words[x] != " ")
                    str += words[x] + " ";
            }

            for (int x = str.Length - 1; x >= 0; x--)
            {
                if (str[x] == ' ')
                    str.Remove(x, 1);
                else
                    break;
            }

            if (string.IsNullOrEmpty(str))
                return;

            if (lines.ContainsKey(str.GetHashCode()))
                lines[str.GetHashCode()] = new KeyValuePair<string,uint>(lines[str.GetHashCode()].Key, lines[str.GetHashCode()].Value + 1);
            else
            {
                lines[str.GetHashCode()] = new KeyValuePair<string,uint>(str, 1);
                words = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (ushort x = 0; x < words.Length; x++)
                {
                    if (this.words.ContainsKey(words[x]))
                    {
                        this.words[words[x]].Add(new KeyValuePair<int,ushort>(str.GetHashCode(), x));
                    }
                    else
                    {
                        List<KeyValuePair<int, ushort>> t = new List<KeyValuePair<int, ushort>>();
                        t.Add(new KeyValuePair<int, ushort>(str.GetHashCode(), x));
                        this.words[words[x]] = t;
                    }
                }
            }
        }

        string Reply(MSG msg)
        {
            string[] words = msg.message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int known = -1;
            List<string> index = new List<string>();
            for (int x = 0; x < words.Length; x++)
            {
                int wordCount = 0;
                if (this.words.ContainsKey(words[x]))
                    wordCount = this.words[words[x]].Count;
                else
                    continue;

                if ((known == -1 || wordCount < known) && wordCount > 3)
                {
                    index.Clear();
                    index.Add(words[x]);
                    known = wordCount;
                    continue;
                }
                else
                    index.Add(words[x]);
            }

            if (index.Count == 0)
                return null;

            string word = index[rand.Next(index.Count)];
            List<string> sentence = new List<string>();
            sentence.Add(word);
            int done = 0;
            while (done == 0)
            {
                Dictionary<string, uint> prewords = new Dictionary<string, uint>();
                prewords.Add("", 0);
                word = sentence[sentence.Count - 1];
                for (int x = 0; x < this.words[word].Count; x++)
                {
                    int l = this.words[word][x].Key;
                    ushort w = this.words[word][x].Value;

                    string context = this.lines[l].Key;
                    uint num_context = this.lines[l].Value;

                    string[] cwords = context.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (w != 0)
                    {
                        if (sentence.Count > 1 && cwords.Length > w + 1)
                        {
                            if (sentence[sentence.Count - 2] != cwords[w + 1])
                                continue;
                        }
                        if (prewords.ContainsKey(cwords[w - 1]))
                            prewords[cwords[w - 1]] += num_context;
                        else
                            prewords[cwords[w - 1]] = num_context;
                    }
                    else
                        prewords[""] += num_context;
                }

                bool added = false;
                while (prewords.Count != 0)
                {
                    uint highest = 0;
                    string s = "";

                    foreach (KeyValuePair<string, uint> pair in prewords)
                    {
                        if (pair.Value > highest)
                        {
                            highest = pair.Value;
                            s = pair.Key;
                        }
                    }

                    highest = (uint)rand.Next((int)highest);
                    int c = rand.Next(prewords.Count);

                    while (c > 0)
                    {
                        foreach (KeyValuePair<string, uint> p in prewords)
                        {
                            if (p.Value > highest)
                            {
                                c--;
                                s = p.Key;
                            }
                            if (c == 0)
                                break;
                        }
                    }

                    if (sentence.Contains(s))
                    {
                        prewords.Remove(s);
                    }
                    else
                    {
                        if (s == "")
                            done = 1;
                        else
                        {
                            sentence.Add(s);
                            added = true;
                        }
                        break;
                    }
                }
                if (!added)
                    done = 1;
            }

            sentence.Reverse();
            done = 0;
            while (done == 0)
            {
                Dictionary<string, uint> postwords = new Dictionary<string, uint>();
                postwords.Add("", 0);
                word = sentence[sentence.Count - 1];
                for (int x = 0; x < this.words[word].Count; x++)
                {
                    int l = this.words[word][x].Key;
                    ushort w = this.words[word][x].Value;

                    string context = this.lines[l].Key;
                    uint num_context = this.lines[l].Value;

                    string[] cwords = context.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (w < sentence.Count && w != 0 && sentence[sentence.Count - 2] != cwords[w - 1])
                        continue;

                    if (w < cwords.Length - 1)
                    {
                        if (postwords.ContainsKey(cwords[w + 1]))
                            postwords[cwords[w + 1]] += num_context;
                        else
                            postwords[cwords[w + 1]] = num_context;
                    }
                    else
                        postwords[""] += num_context;
                }

                bool added = false;
                while (postwords.Count != 0)
                {
                    uint highest = 0;
                    string s = "";

                    foreach (KeyValuePair<string, uint> pair in postwords)
                    {
                        if (pair.Value > highest)
                        {
                            highest = pair.Value;
                            s = pair.Key;
                        }
                    }

                    highest = (uint)rand.Next((int)highest);
                    int c = rand.Next(postwords.Count);

                    while (c > 0)
                    {
                        foreach (KeyValuePair<string, uint> p in postwords)
                        {
                            if (p.Value > highest)
                            {
                                c--;
                                s = p.Key;
                            }
                            if (c == 0)
                                break;
                        }
                    }

                    if (sentence.Contains(s))
                    {
                        postwords.Remove(s);
                    }
                    else
                    {
                        if (s == "")
                            done = 1;
                        else
                        {
                            sentence.Add(s);
                            added = true;                            
                        }
                        break;
                    }
                }
                if (!added)
                    done = 1;
            }

            string str = "";
            for (int x = 0; x < sentence.Count; x++)
            {
                str += sentence[x];
                if (x != sentence.Count - 1)
                    str += " ";
            }
            return str;
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(irc_OnMSGRecvd);
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(irc_OnPMRecvd);
        }

        public override string GetName()
        {
            return "Markov";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
