using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkovEx
{
    class ZZLink<T, F> : IComparable<ZZLink<T, F>>, IEquatable<ZZLink<T, F>>
    {
        int dimension = 0;
        T to;
        F from;
        int strength = 0;

        public ZZLink(F from, T to, int dimension)
        {
            this.from = from;
            this.to = to;
            this.dimension = dimension;
            this.strength = 1;
        }

        public T GetTo()
        {
            return to;
        }

        public F GetFrom()
        {
            return from;
        }

        public int CompareTo(ZZLink<T, F> other)
        {
            return strength.CompareTo(other.strength);
        }

        public bool Equals(ZZLink<T, F> other)
        {
            return to.Equals(other.to) && from.Equals(other.from);
        }

        public int Strength
        {
            get { return strength; }
            set { strength = value; }
        }

        public override string ToString()
        {
            return from.ToString() + " -> " + to.ToString();
        }
    }

    class ZZNode<T> : IEquatable<ZZNode<T>>
    {
        List<ZZLink<ZZNode<T>, ZZNode<T>>> backLinks = new List<ZZLink<ZZNode<T>, ZZNode<T>>>();
        List<ZZLink<ZZNode<T>, ZZNode<T>>> nextLinks = new List<ZZLink<ZZNode<T>, ZZNode<T>>>();
        List<ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>>> sideLinks = new List<ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>>>();
        T data;

        public ZZNode(T data)
        {
            this.data = data;
        }

        public bool Equals(ZZNode<T> other)
        {
            return data.Equals(other.data);
        }

        public T GetData()
        {
            return data;
        }

        public void AddBackLink(ZZLink<ZZNode<T>, ZZNode<T>> backLink)
        {
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> l in backLinks)
            {
                if (l.Equals(backLink))
                {
                    l.Strength++;
                    return;
                }
            }
            backLinks.Add(backLink);
        }

        public void AddNextLink(ZZLink<ZZNode<T>, ZZNode<T>> nextLink)
        {
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> l in nextLinks)
            {
                if (l.Equals(nextLink))
                {
                    l.Strength++;
                    return;
                }
            }
            nextLinks.Add(nextLink);
        }

        public void AddSideLink(ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> sideLink)
        {
            foreach (ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> l in sideLinks)
            {
                if (l.Equals(sideLink))
                {
                    l.Strength++;
                    return;
                }
            }
            sideLinks.Add(sideLink);
        }

        public ZZLink<ZZNode<T>, ZZNode<T>> GetStrongestBackLink()
        {
            ZZLink<ZZNode<T>, ZZNode<T>> ret = null;
            int strongest = int.MinValue;
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> link in backLinks)
            {
                if (link.Strength > strongest)
                {
                    ret = link;
                    strongest = link.Strength;
                }
            }
            return ret;
        }

        public ZZLink<ZZNode<T>, ZZNode<T>> GetRandomBackLink()
        {
            int total = 0;
            ZZLink<ZZNode<T>, ZZNode<T>> ret = null;
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> link in backLinks)
            {
                total += link.Strength;
            }
            int position = 0;
            int rand = new Random((int)DateTime.Now.Ticks).Next(total);
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> link in backLinks)
            {
                position += link.Strength;
                if (position > rand)
                {
                    ret = link;
                    break;
                }
            }
            return ret;
        }

        public ZZLink<ZZNode<T>, ZZNode<T>> GetRandomNextLink()
        {
            int total = 0;
            ZZLink<ZZNode<T>, ZZNode<T>> ret = null;
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> link in nextLinks)
            {
                total += link.Strength;
            }
            int position = 0;
            int rand = new Random((int)DateTime.Now.Ticks).Next(total);
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> link in nextLinks)
            {
                position += link.Strength;
                if (position > rand)
                {
                    ret = link;
                    break;
                }
            }
            return ret;
        }

        public ZZLink<ZZNode<T>, ZZNode<T>> GetStrongestNextLink()
        {
            ZZLink<ZZNode<T>, ZZNode<T>> ret = null;
            int strongest = int.MinValue;
            foreach (ZZLink<ZZNode<T>, ZZNode<T>> link in nextLinks)
            {
                if (link.Strength > strongest)
                {
                    ret = link;
                    strongest = link.Strength;
                }
            }
            return ret;
        }

        public int GetSideLinkStrength()
        {
            int strongest = 0;
            foreach (ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> link in sideLinks)
            {
                strongest += link.Strength;
            }
            return strongest;
        }

        public ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> GetRandomSideLink()
        {
            int total = 0;
            ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> ret = null;
            foreach (ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> link in sideLinks)
            {
                total += link.Strength;
            }
            int position = 0;
            int rand = new Random((int)DateTime.Now.Ticks).Next(total);
            foreach (ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> link in sideLinks)
            {
                position += link.Strength;
                if (position > rand)
                {
                    ret = link;
                    break;
                }
            }
            return ret;
        }

        public ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> GetStrongestSideLink()
        {
            ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> ret = null;
            int strongest = int.MinValue;
            foreach (ZZLink<ZZNode<T>, ZZNode<List<ZZNode<T>>>> link in sideLinks)
            {
                if (link.Strength > strongest)
                {
                    ret = link;
                    strongest = link.Strength;
                }
            }
            return ret;
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }

    public class ZZStructure
    {
        List<ZZNode<List<ZZNode<string>>>> lines = new List<ZZNode<List<ZZNode<string>>>>();
        List<ZZNode<string>> wordList = new List<ZZNode<string>>();
        ZZNode<List<ZZNode<string>>> lastSentence;
        ZZNode<List<ZZNode<string>>> sentenceNode;
        ZZNode<string> wordNode;
        Regex regex;

        public ZZStructure()
        {
            //words.Add(string.Empty, new ZZNode<string>(string.Empty));
            wordList.Add(new ZZNode<string>(string.Empty));
            lastSentence = new ZZNode<List<ZZNode<string>>>(new List<ZZNode<string>>());
            lines.Add(lastSentence);
            regex = new Regex("\\A[a-z0-9]+\\Z");
        }

        public string GenerateResponse(string input)
        {
            input = input.ToLower();
            ZZNode<string> popWord = null;
            int strongest = int.MinValue;
            foreach (string s in input.Split(' '))
            {
                wordNode = new ZZNode<string>(s);
                if (this.wordList.Exists(WordEquals))
                {

                    if (wordNode.GetSideLinkStrength() > strongest && wordNode.GetStrongestSideLink() != null)
                    {
                        popWord = wordNode;
                        strongest = wordNode.GetSideLinkStrength();
                    }
                }
            }

            if (popWord == null)
                return null;

            ZZNode<List<ZZNode<string>>> sent = null;
            while (sent == null)
            {
                sent = popWord.GetRandomSideLink().GetFrom();
            }
            if (sent.GetStrongestNextLink() != null)
                sent = sent.GetStrongestNextLink().GetTo();

            popWord = sent.GetData()[new Random().Next(sent.GetData().Count)];

            Stack<string> backWords = new Stack<string>();
            Queue<string> nextWords = new Queue<string>();
            ZZLink<ZZNode<string>, ZZNode<string>> tempLink = popWord.GetRandomBackLink();
            ZZNode<string> tempWord = popWord;
            while (tempLink != null && tempLink.GetTo() != null && !string.IsNullOrEmpty(tempLink.GetTo().GetData()) && !backWords.Contains(tempLink.GetTo().GetData()) && popWord.GetData() != tempLink.GetTo().GetData())
            {
                tempWord = tempLink.GetTo();
                backWords.Push(tempWord.GetData());
                tempLink = tempWord.GetRandomBackLink();
            }
            tempLink = popWord.GetRandomNextLink();
            tempWord = popWord;
            while (tempLink != null && tempLink.GetTo() != null && !string.IsNullOrEmpty(tempLink.GetTo().GetData()) && !nextWords.Contains(tempLink.GetTo().GetData()) && popWord.GetData() != tempLink.GetTo().GetData())
            {
                tempWord = tempLink.GetTo();
                nextWords.Enqueue(tempWord.GetData());
                tempLink = tempWord.GetRandomNextLink();
            }
            string ret = "";
            while (backWords.Count != 0)
            {
                ret += backWords.Pop() + " ";
            }
            ret += popWord.GetData();
            if (nextWords.Count != 0)
                ret += " ";
            while (nextWords.Count != 0)
            {
                ret += nextWords.Dequeue();
                if (nextWords.Count != 0)
                    ret += " ";
            }
            return ret;

        }

        bool WordEquals(ZZNode<string> other)
        {
            if (wordNode.GetData() == other.GetData())
            {
                wordNode = other;
                return true;
            }
            return false;
        }

        bool ListEquals(ZZNode<List<ZZNode<string>>> other)
        {
            if (sentenceNode.GetData().Count != other.GetData().Count)
                return false;
            for (int x = 0; x < sentenceNode.GetData().Count; x++)
            {
                if (sentenceNode.GetData()[x] != other.GetData()[x])
                    return false;
            }
            sentenceNode = other;
            return true;
        }

        public void AddSentence(string sentence)
        {
            sentence = sentence.ToLower();
            sentenceNode = null;

            List<ZZNode<string>> sentenceList = new List<ZZNode<string>>();
            string[] words = sentence.Split(' ');
            foreach (string word in words)
            {
                if (regex.Match(word).Success)
                {
                    wordNode = new ZZNode<string>(word);
                    if (!this.wordList.Exists(WordEquals))
                    {
                        wordNode = new ZZNode<string>(word);
                        this.wordList.Add(wordNode);
                    }
                    sentenceList.Add(wordNode);
                }
            }
            if (sentenceList.Count == 0)
                return;
            sentenceNode = new ZZNode<List<ZZNode<string>>>(sentenceList);
            if (!lines.Exists(ListEquals))
            {
                lines.Add(sentenceNode);
            }

            for (int x = 0; x < sentenceList.Count; x++)
            {
                ZZNode<string> w = sentenceList[x];
                w.AddSideLink(new ZZLink<ZZNode<string>, ZZNode<List<ZZNode<string>>>>(sentenceNode, w, 2));
            }

            if (!lastSentence.Equals(sentenceNode))
            {
                lastSentence.AddNextLink(new ZZLink<ZZNode<List<ZZNode<string>>>, ZZNode<List<ZZNode<string>>>>(lastSentence, sentenceNode, 3));
                sentenceNode.AddBackLink(new ZZLink<ZZNode<List<ZZNode<string>>>, ZZNode<List<ZZNode<string>>>>(sentenceNode, lastSentence, 3));
            }
            lastSentence = sentenceNode;

            for (int x = 0; x < sentenceList.Count; x++)
            {
                ZZNode<string> word = sentenceList[x];

                if (x == 0 && x == words.Length - 1)
                {
                    //add terminator to last and next
                    wordNode = new ZZNode<string>(string.Empty);
                    wordList.Exists(WordEquals);
                    ZZLink<ZZNode<string>, ZZNode<string>> backNode = new ZZLink<ZZNode<string>, ZZNode<string>>(wordNode, word, 1);
                    ZZLink<ZZNode<string>, ZZNode<string>> nextNode = new ZZLink<ZZNode<string>, ZZNode<string>>(word, wordNode, 1);
                    wordNode.AddNextLink(backNode);
                    wordNode.AddBackLink(backNode);
                    word.AddBackLink(nextNode);
                    word.AddNextLink(nextNode);
                }
                else if (x == 0)
                {
                    //add terminator to last
                    wordNode = new ZZNode<string>(string.Empty);
                    wordList.Exists(WordEquals);
                    wordNode.AddNextLink(new ZZLink<ZZNode<string>, ZZNode<string>>(wordNode, word, 1));
                    word.AddBackLink(new ZZLink<ZZNode<string>, ZZNode<string>>(word, wordNode, 1));
                    wordNode = new ZZNode<string>(words[x + 1]);
                    if (wordList.Exists(WordEquals))
                    {
                        word.AddNextLink(new ZZLink<ZZNode<string>, ZZNode<string>>(word, wordNode, 1));
                        wordNode.AddBackLink(new ZZLink<ZZNode<string>, ZZNode<string>>(wordNode, word, 1));
                    }
                }
                else if (x == words.Length - 1)
                {
                    //add terminator to next
                    wordNode = new ZZNode<string>(string.Empty);
                    wordList.Exists(WordEquals);
                    wordNode.AddBackLink(new ZZLink<ZZNode<string>, ZZNode<string>>(wordNode, word, 1));
                    word.AddNextLink(new ZZLink<ZZNode<string>, ZZNode<string>>(word, wordNode, 1));
                }
                else
                {
                    //treat it normally
                    wordNode = new ZZNode<string>(words[x + 1]);
                    if (wordList.Exists(WordEquals) && wordNode != word)
                    {
                        word.AddNextLink(new ZZLink<ZZNode<string>, ZZNode<string>>(word, wordNode, 1));
                        wordNode.AddBackLink(new ZZLink<ZZNode<string>, ZZNode<string>>(wordNode, word, 1));
                    }
                }
            }
        }
    }


    public class MarkovEx : Module
    {
        ZZStructure zzn;

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
                        string s = "";// zzn.GenerateResponse(lastMSG.message);
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
        }

        public override void Load()
        {
            if (!File.Exists(server.Data + Path.DirectorySeparatorChar + "logs.dat"))
            {
                zzn = new ZZStructure();
                return;
            }
            zzn = new ZZStructure();
            foreach (string line in File.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "logs.dat"))
            {
                if (line.Contains(">"))
                {
                    string msg = line.Substring(line.IndexOf('>') + 1);
                    zzn.AddSentence(msg);
                }
            }
        }

        void ProcessMessage(object o)
        {
            try
            {
                MSG msg = new MSG(((MSG)o).ToString());
                lastMSG = msg;
                msg.message = msg.message.ToLower().Replace("\"", "").Replace("\r", "").Replace("\n", "").Replace(";", "").Replace("!", "").Replace("?", "").Replace(".", "").Replace(",", "").Replace("'", "").Replace(":", "");
                File.AppendAllText(server.Data + Path.DirectorySeparatorChar + "logs.dat", "\n>" + msg.message);
                zzn.AddSentence(msg.message);

                if (rand.Next(100) < TalkValue)
                {
                    string s = zzn.GenerateResponse(msg.message);
                    if (string.IsNullOrEmpty(s))
                        return;
                    irc.SendMessage(msg.to, s);
                }
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Oops in markovex");
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
