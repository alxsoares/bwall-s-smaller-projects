using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ModularIrcBot
{
    public abstract class Module
    {
        protected IrcClient irc;
        protected Server server;

        public virtual void StartModule(IrcClient irc, Server server)
        {
            this.server = server;
            this.irc = irc;            
            AddBindings();
            Load();
        }

        public abstract void AddBindings();
        public abstract void RemoveBindings();

        public virtual void WriteStatsToWWW()
        {

        }

        public virtual string GetPrivateValue(string field)
        {
            try
            {
                Type myTypeB = this.GetType();
                FieldInfo myFieldInfo1 = myTypeB.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
                return myFieldInfo1.GetValue(this).ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public virtual void StopModule()
        {
            Console.WriteLine("Saving " + GetName());
            Save();
            Console.WriteLine("Saved " + GetName());
            if (irc != null)
                RemoveBindings();
        }

        public virtual string GetCommands()
        {
            return null;
        }

        public virtual void AuthedCommand(MSG msg, int operLevel)
        {
            msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
            if (msg.message.StartsWith("value "))
            {
                string s = GetPrivateValue(msg.message.Substring(msg.message.IndexOf(' ') + 1));
                if (!string.IsNullOrEmpty(s))
                    irc.SendMessage(msg.from, msg.message.Substring(msg.message.IndexOf(' ') + 1) + " = " + s);
            }
        }

        public virtual string GetStats()
        {
            return null;
        }

        public virtual string GetSpecificStats(string nick)
        {
            return null;
        }

        abstract public string GetName();
        abstract public string GetHelp();

        public virtual void Load()
        {

        }

        public virtual void Save()
        {

        }

        public virtual string GetSpecificHelp(string category)
        {
            return null;
        }
    }
}
