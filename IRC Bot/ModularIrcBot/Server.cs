using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModularIrcBot
{
    public class Server
    {
        public List<string> channels = new List<string>();
        public string domain = null;
        public string name = null;
        public int port = -1;
        public bool useSSL = false;
        string dataFolder = null;
        string wwwFolder = null;
        string Nick = null;
        string otherNick = null;
        string User = null;
        string Real = null;
        string Host = null;
        string file = null;
        string version = null;

        public string Version
        {
            get
            {
                if (version != null)
                    return version;
                return Config.I.vars["Version"];
            }
        }

        public string SSLFolder
        {
            get
            {
                return Config.I.vars["ServerFolder"] + Path.DirectorySeparatorChar + "SSLCerts" + Path.DirectorySeparatorChar + name;
            }
        }

        public string host
        {
            get
            {
                if (Host == null)
                    return Config.I.vars["Host"];
                return Host;
            }
        }

        public string real
        {
            get
            {
                if (Real == null)
                    return Config.I.vars["Real"];
                return Real;
            }
        }

        public string user
        {
            get
            {
                if (User == null)
                    return Config.I.vars["User"];
                return User;
            }
        }

        public string othernick
        {
            get
            {
                if (otherNick == null)
                    return Config.I.vars["otherNick"];
                return otherNick;
            }
        }

        public string nick
        {
            get
            {
                if (Nick == null)
                    return Config.I.vars["Nick"];
                return Nick;
            }
        }

        public string Data
        {
            get
            {
                if (dataFolder == null)
                    return Config.I.vars["Data"] + Path.DirectorySeparatorChar + name;
                return dataFolder;
            }
        }

        public string WWW
        {
            get
            {
                if (wwwFolder == null)
                    return Config.I.vars["WWW"] + Path.DirectorySeparatorChar + name;
                return wwwFolder;
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(file))
                Save(file);
            else
                Save(Config.I.vars["ServerFolder"] + Path.DirectorySeparatorChar + name);
        }

        public void Save(string file)
        {
            this.file = file;
            string outfile = "";
            if (Nick != null)
                outfile += "Nick " + Nick + "\n";
            if (otherNick != null)
                outfile += "otherNick " + otherNick + "\n";
            if (User != null)
                outfile += "User " + User + "\n";
            if (Real != null)
                outfile += "Real " + Real + "\n";
            if (Host != null)
                outfile += "Host " + Host + "\n";
            if (domain != null)
                outfile += "Domain " + domain + "\n";
            if (port != -1)
                outfile += "Port " + port.ToString() + "\n";
            outfile += "useSSL " + useSSL.ToString() + "\n";
            if (name != null)
                outfile += "Name " + name + "\n";
            if (wwwFolder != null)
                outfile += "WWWFolder " + wwwFolder + "\n";
            if (dataFolder != null)
                outfile += "DataFolder " + dataFolder + "\n";
            if (version != null)
                outfile += "Version " + version + "\n";
            foreach (string chan in channels)
                outfile += "Channel " + chan + "\n";
            EncFile.WriteAllText(file, outfile);
        }

        public Server()
        {
            Console.Write("Name of server: ");
            name = Console.ReadLine();
            Console.Write("Domain of server: ");
            domain = Console.ReadLine();
            Console.Write("Port for IRC: ");
            port = int.Parse(Console.ReadLine());
            Console.Write("Use ssl?[true/false]: ");
            useSSL = bool.Parse(Console.ReadLine());
            Console.WriteLine("Enter channels to join, pressing enter after each one.  When complete, enter \".\"");
            string chan = null;
            while (chan != ".")
            {
                chan = Console.ReadLine();
                if (chan == ".")
                    break;
                channels.Add(chan);
            }
            Save();
        }

        public Server(string file)
        {
            this.file = file;
            if (File.Exists(file))
            {
                foreach (string line in EncFile.ReadAllLines(file))
                {
                    string[] split = line.Split(' ');
                    if (split.Length >= 2)
                    {
                        switch (split[0])
                        {
                            case "Nick":
                                Nick = split[1];
                                break;
                            case "otherNick":
                                otherNick = split[1];
                                break;
                            case "User":
                                User = split[1];
                                break;
                            case "Real":
                                Real = split[1];
                                break;
                            case "Host":
                                Host = split[1];
                                break;
                            case "Domain":
                                domain = split[1];
                                break;
                            case "Port":
                                port = int.Parse(split[1]);
                                break;
                            case "Channel":
                                channels.Add(split[1]);
                                break;
                            case "useSSL":
                                useSSL = bool.Parse(split[1]);
                                break;
                            case "Name":
                                name = line.Substring(split[0].Length + 1);
                                break;
                            case "DataFolder":
                                dataFolder = line.Substring(split[0].Length + 1);
                                break;
                            case "WWWFolder":
                                wwwFolder = line.Substring(split[0].Length + 1);
                                break;
                            case "Version":
                                version = line.Substring(split[0].Length + 1);
                                break;
                        }
                    }
                }
            }
        }
    }
}
