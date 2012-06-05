using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PHPShell
{
    public class ShellList
    {
        static ShellList instance = null;
        static readonly object padlock = new object();
        public List<ShellURL> ShellURLs = new List<ShellURL>();

        public static ShellList Instance
        {
            get
            {
				lock(padlock)
				{
	                if (instance == null)
	                    instance = new ShellList();
	                return instance;
				}
            }
        }

        public void LoadURLList()
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(Configuration.Instance.UrlList);
                reader.Read();
            }
            catch
            {
                if (reader != null)
                    reader.Close();
                SaveURLList();
                reader = new XmlTextReader(Configuration.Instance.UrlList);
                reader.Read();
            }
			if(reader.Name == "URLList" && !reader.IsEmptyElement)
			{
	            while (!(reader.Name == "URLList" && reader.NodeType == XmlNodeType.EndElement))
	            {
	                reader.Read();
	                if (reader.Name == "ShellURL" && reader.IsStartElement())
	                {
	                    string url = "";
                        string password = "";
                        string prestring = "";
	                    while (!(reader.Name == "ShellURL" && reader.NodeType == XmlNodeType.EndElement))
	                    {
	                        reader.Read();
	                        if (reader.IsStartElement())
	                        {
	                            switch (reader.Name)
	                            {
	                                case "URL":
	                                    reader.Read();
	                                    url = reader.Value;
	                                    break;
                                    case "Password":
                                        reader.Read();
                                        password = reader.Value;
                                        break;
                                    case "PreString":
                                        reader.Read();
                                        prestring = reader.Value;
                                        break;
	                            }
	                        }
	                    }
	                    ShellURLs.Add(new ShellURL(url, password, prestring));
	                }
	            }
			}
            reader.Close();
        }

        public void SaveURLList()
        {
            XmlTextWriter writer = new XmlTextWriter(Configuration.Instance.UrlList, Encoding.ASCII);
            writer.WriteStartElement("URLList");
            foreach (ShellURL shell in ShellURLs)
            {
                writer.WriteStartElement("ShellURL");
                writer.WriteStartElement("URL");
                writer.WriteString(shell.URL);
                writer.WriteEndElement();
                writer.WriteStartElement("Password");
                writer.WriteString(shell.Password);
                writer.WriteEndElement();
                writer.WriteStartElement("PreString");
                writer.WriteString(shell.PreString);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Close();
        }
    }
}
