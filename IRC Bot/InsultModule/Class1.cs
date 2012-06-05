using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.IO;

namespace InsultModule
{
    public class InsultModule : ModularIrcBot.Module
    {
        List<string> bodyParts = new List<string>();
        List<string> animalCocks = new List<string>();
        List<string> wipeMunchers = new List<string>();
        Random rand = new Random();

        public override void Load()
        {
            if (File.Exists(server.Data + Path.DirectorySeparatorChar + "insult.db"))
            {
                foreach (string line in EncFile.ReadAllLines(server.Data + Path.DirectorySeparatorChar + "insult.db"))
                {
                    switch (line.Split(':')[0])
                    {
                        case "body":
                            bodyParts.Add(line.Split(':')[1]);
                            break;
                        case "animal":
                            animalCocks.Add(line.Split(':')[1]);
                            break;
                        case "wipe":
                            wipeMunchers.Add(line.Split(':')[1]);
                            break;
                    }
                }
            }
            else
            {
                File.Create(server.Data + Path.DirectorySeparatorChar + "insult.db").Close();
                bodyParts.Add("face");
                bodyParts.Add("ass");
                bodyParts.Add("dick");
                bodyParts.Add("twat");
                bodyParts.Add("mouth");

                animalCocks.Add("baby dick");
                animalCocks.Add("mosquito cock");
                animalCocks.Add("weasel's ass");
                animalCocks.Add("badger's vagina");
                animalCocks.Add("rhino ass");
                animalCocks.Add("90 year old man's boobs");

                wipeMunchers.Add("butt faced pile of dog shit");
                wipeMunchers.Add("baby rapist");
                wipeMunchers.Add("shit eating cock bag");
                wipeMunchers.Add("cock sucking fucktard");
                wipeMunchers.Add("steaming pile of nigger shit");
            }            
        }

        public override void Save()
        {
            string o = "";
            foreach (string s in bodyParts)
                o += "body:" + s + "\n";
            foreach (string s in animalCocks)
                o += "animal:" + s + "\n";
            foreach (string s in wipeMunchers)
                o += "wipe:" + s + "\n";
            EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "insult.db", o);
        }

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new ModularIrcBot.IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        void irc_OnMSGRecvd(ModularIrcBot.MSG msg)
        {
            if (msg.message.StartsWith(".insult"))
            {
                string bodyPart = bodyParts[rand.Next(bodyParts.Count)];
                string animalGenitalia = animalCocks[rand.Next(animalCocks.Count)];
                string wipeMuncher = wipeMunchers[rand.Next(wipeMunchers.Count)];

                string insult = "Your " + bodyPart + " looks like a " + animalGenitalia + ", you " + wipeMuncher;
                irc.SendMessage(msg.to, insult);
            }
            else if (msg.message.StartsWith(".addbody "))
                bodyParts.Add(msg.message.Substring(".addbody ".Length));
            else if (msg.message.StartsWith(".addanimal "))
                animalCocks.Add(msg.message.Substring(".addanimal ".Length));
            else if (msg.message.StartsWith(".addwipe "))
                wipeMunchers.Add(msg.message.Substring(".addwipe ".Length));
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new ModularIrcBot.IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        public override string GetName()
        {
            return "Insult";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
