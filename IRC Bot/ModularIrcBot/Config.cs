using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModularIrcBot
{
    public class ConfigNode
    {
        public ConfigNode(string text, int id, ConfigNode parent)
        {
            Text = text;
            ID = id;
            Parent = parent;
            Children = new Dictionary<string, ConfigNode>();
        }

        public ConfigNode(string text, int id, ConfigNode parent, string data)
        {
            Text = text;
            ID = id;
            Parent = parent;
            Data = data;
            Children = new Dictionary<string, ConfigNode>();
        }

        public ConfigNode(string fileLine)
        {
            string[] split = fileLine.Split(' ');
            Parent = Config.I.GetNode(int.Parse(split[0]));
            ID = int.Parse(split[1]);
            Text = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(split[2]));
            Data = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(split[3]));
        }

        public override string ToString()
        {
            //ParentID ID base64(Text) base64(Data)
            string ret = "";
            if(Parent != null)
                ret = Parent.ID.ToString() + " " + ID.ToString() + " " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Text)) + " " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Data));
            else
                ret = (0).ToString() + " " + ID.ToString() + " " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Text)) + " " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Data));
            foreach (ConfigNode cn in Children.Values)
                ret += "\n" + cn.ToString();
            return ret;
        }

        public string Text = null;
        public string Data = null;
        public ConfigNode Parent = null;
        public Dictionary<string, ConfigNode> Children = null;
        public int ID = -1;
    }

    public sealed class Config
    {
        static Config instance = null;
        static readonly object padlock = new object();

        public bool EncryptFiles = false;
        public string password = null;
        public string FilePassword
        {
            get
            {
                return password;
            }
        }
        public List<Server> servers = new List<Server>();
        public Dictionary<string, string> vars = new Dictionary<string, string>();
        public ConfigNode nodes = new ConfigNode("root", 0, null);
        public Dictionary<int, ConfigNode> nodeList = new Dictionary<int, ConfigNode>();

        public ConfigNode GetNode(int ID)
        {
            if (nodeList.ContainsKey(ID))
                return nodeList[ID];
            return null;
        }

        public ConfigNode GetNode(string path)
        {
            string[] splits = path.Split('/');
            ConfigNode current = nodes;

            try
            {
                foreach (string s in splits)
                {
                    current = current.Children[s];
                }
            }
            catch { current = null; }

            return current;
        }

        Config()
        {
            nodes.Children.Add("Folders", new ConfigNode("Folders", 1, null));
            nodes.Children.Add("ClientInfo", new ConfigNode("ClientInfo", 2, null));

            nodes.Children["Folders"].Children.Add("Server", new ConfigNode("Servers", 3, nodes.Children["Folders"], "Servers"));
            nodes.Children["Folders"].Children.Add("Modules", new ConfigNode("Modules", 4, nodes.Children["Folders"], "Modules"));
            nodes.Children["Folders"].Children.Add("WWW", new ConfigNode("WWW", 5, nodes.Children["Folders"], "www"));
            nodes.Children["Folders"].Children.Add("Data", new ConfigNode("Data", 6, nodes.Children["Folders"], "data"));

            nodes.Children["ClientInfo"].Children.Add("Nick", new ConfigNode("Nick", 7, nodes.Children["ClientInfo"], "buckey11"));
            nodes.Children["ClientInfo"].Children.Add("User", new ConfigNode("User", 8, nodes.Children["ClientInfo"], "buckey11"));
            nodes.Children["ClientInfo"].Children.Add("Real", new ConfigNode("Real", 9, nodes.Children["ClientInfo"], "buckey11"));
            nodes.Children["ClientInfo"].Children.Add("Host", new ConfigNode("Host", 10, nodes.Children["ClientInfo"], "buckey11"));
            nodes.Children["ClientInfo"].Children.Add("Version", new ConfigNode("Version", 10, nodes.Children["ClientInfo"], "xchat 2.8.8 Ubuntu"));

            vars["ServerFolder"] = "Servers";
            vars["ModuleLocation"] = "Modules";
            vars["Nick"] = "buckey11";
            vars["User"] = "buckey11";
            vars["Real"] = "buckey11";
            vars["Host"] = "buckey11";
            vars["WWW"] = "www";
            vars["Data"] = "data";
            vars["Version"] = "xchat 2.8.8 Ubuntu";
            Load();
            Directory.CreateDirectory(GetNode("Folders/WWW").Data);
            Directory.CreateDirectory(GetNode("Folders/Data").Data);
            Directory.CreateDirectory(GetNode("Folders/Server").Data);
            Directory.CreateDirectory(GetNode("Folders/Modules").Data);
        }

        public static Config I
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }
                    return instance;
                }
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists("config"))
                {
                    foreach (string line in File.ReadAllLines("config"))
                    {
                        ConfigNode node = new ConfigNode(line);
                        nodeList.Add(node.ID, node);
                    }
                }
            }
            catch { }
        }

        public void LoadServers()
        {
            try
            {
                if (Directory.Exists(vars["ServerFolder"]))
                {
                    foreach (string file in Directory.GetFiles(vars["ServerFolder"]))
                        servers.Add(new Server(file));
                }
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                string outfile = "";
                foreach (ConfigNode cn in nodes.Children.Values)
                {
                    outfile += cn.ToString() + "\n";
                }
                //outfile += "EncFiles " + EncryptFiles.ToString() + "\n";
                //foreach (KeyValuePair<string, string> k in vars)
                //    outfile += k.Key + " " + k.Value + "\n";
                File.WriteAllText("config", outfile);
                foreach (Server s in servers)
                    s.Save();
            }
            catch { }
        }
    }
}
