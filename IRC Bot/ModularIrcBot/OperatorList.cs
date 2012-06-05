using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace ModularIrcBot
{
    class OperatorList
    {
        Dictionary<string, byte[]> Logins = new Dictionary<string, byte[]>();
        Dictionary<string, int> OperatorLevels = new Dictionary<string, int>();
        Dictionary<string, string> LoggedIn = new Dictionary<string, string>();
        IrcClient irc;
        Server server = null;

        public OperatorList(IrcClient irc, Server server)
        {
            this.server = server;
            Load();
            this.irc = irc;
            irc.OnQuitRecvd += new IrcClient.JoinRecvd(irc_OnQuitRecvd);
            irc.OnNickRecvd += new IrcClient.JoinRecvd(irc_OnNickRecvd);
        }

        public void RemoveOperator(string user)
        {
            try
            {
                if (LoggedIn.ContainsValue(user))
                {
                    //log that oper off
                    string logged = null;
                    foreach (KeyValuePair<string, string> u in LoggedIn)
                    {
                        if (u.Value == user)
                            logged = u.Key;
                    }
                    if(logged != null)
                        LoggedIn.Remove(logged);
                }
                Logins.Remove(user);
            }
            catch { }
            Save();
        }

        public List<string> ListOperators()
        {
            return new List<string>(Logins.Keys);
        }

        public void AddOperator(string user, string pass)
        {
            Logins[user] = HashPassword(user, pass);
            OperatorLevels[user] = 0;
            Save();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public void Save()
        {
            try
            {
                string output = "";
                foreach (KeyValuePair<string, byte[]> login in Logins)
                    output += OperatorLevels[login.Key] + " " + login.Key + " " + ByteArrayToString(login.Value);
                Directory.CreateDirectory(server.Data);
                EncFile.WriteAllText(server.Data + Path.DirectorySeparatorChar + "operatorList.db", output);
            }
            catch { }
        }

        public string ChangePassword(string nick, string user, string host, string password, string newpassword)
        {
            if (!IsLoggedIn(nick, user, host))
                return "You have to login before you can change your password.";
            if (!CompareByteArray(Logins[LoggedIn[nick + "!" + user + "@" + host]], HashPassword(LoggedIn[nick + "!" + user + "@" + host], password)))
                return "You have supplied an incorrect password for this nick.";
            Logins[LoggedIn[nick + "!" + user + "@" + host]] = HashPassword(LoggedIn[nick + "!" + user + "@" + host], newpassword);
            Save();
            return "Password Successfully Changed";
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public void Load()
        {
            try
            {
                if (File.Exists(server.Data + Path.DirectorySeparatorChar + "operatorList.db"))
                {
                    string input = EncFile.ReadAllText(server.Data + Path.DirectorySeparatorChar + "operatorList.db");
                    while (!string.IsNullOrEmpty(input))
                    {
                        int level = int.Parse(input.Split(' ')[0]);
                        input = input.Substring(input.IndexOf(' ') + 1);
                        string user = input.Split(' ')[0];
                        input = input.Substring(user.Length + 1);
                        byte[] hash = StringToByteArray(input.Substring(0, 64));
                        Logins[user] = hash;
                        OperatorLevels[user] = level;
                        input = input.Substring(64);
                    }
                }
            }
            catch { }
        }

        void irc_OnNickRecvd(JoinMsg join)
        {
            if (LoggedIn.ContainsKey(join.who + "!" + join.whoUser + "@" + join.whoHost))
            {
                string nick = LoggedIn[join.who + "!" + join.whoUser + "@" + join.whoHost];
                LoggedIn.Remove(join.who + "!" + join.whoUser + "@" + join.whoHost);
                LoggedIn.Add(join.channel + "!" + join.whoUser + "@" + join.whoHost, nick);
            }
        }

        bool CompareByteArray(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int x = 0; x < a.Length; x++)
            {
                if (a[x] != b[x])
                    return false;
            }
            return true;
        }

        public bool LogIn(string nick, string user, string host, string password)
        {
            if (Logins.ContainsKey(nick))
            {
                if (CompareByteArray(Logins[nick], HashPassword(nick, password)))
                {
                    LoggedIn.Add(nick + "!" + user + "@" + host, nick);
                    return true;
                }
            }
            return false;
        }

        public bool IsLoggedIn(string nick, string user, string host)
        {
            return LoggedIn.ContainsKey(nick + "!" + user + "@" + host);
        }

        void irc_OnQuitRecvd(JoinMsg join)
        {
            LoggedIn.Remove(join.who + "!" + join.whoUser + "@" + join.whoHost);
        }

        byte[] HashPassword(string nick, string password)
        {
            byte[] ret = ASCIIEncoding.ASCII.GetBytes(password);
            for (int x = 0; x < 100; x++)
            {
                byte[] temp = new byte[ret.Length + nick.Length];
                Buffer.BlockCopy(ret, 0, temp, 0, ret.Length);
                Buffer.BlockCopy(ASCIIEncoding.ASCII.GetBytes(nick), 0, temp, ret.Length, temp.Length - ret.Length);
                ret = new SHA256Managed().ComputeHash(temp);
            }
            return ret;
        }

        public int GetOperatorLevel(string nick, string user, string host)
        {
            try
            {
                return OperatorLevels[LoggedIn[nick + "!" + user + "@" + host]];
            }
            catch
            {
                return -1;
            }
        }
    }
}
