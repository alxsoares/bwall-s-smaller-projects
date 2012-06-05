using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PHPShell
{
    class Configuration
    {
        static Configuration instance = null;
        static readonly object padlock = new object();

        public string DataFolder = "Data";
        public string UrlList = "Urls.xml";

        public static Configuration Instance
        {
            get
            {
				lock(padlock)
				{
	                if (instance == null)
	                    instance = new Configuration();
	                return instance;
				}
            }
        }

        Configuration()
        {
            LoadConfiguration("configuration.xml");
        }

        private void LoadConfiguration(string p)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(p);
                reader.Read();
            }
            catch
            {
                if (reader != null)
                    reader.Close();
                SaveConfiguration(p);
                reader = new XmlTextReader(p);
                reader.Read();
            }
            while (!(reader.Name == "Configuration" && reader.NodeType == XmlNodeType.EndElement))
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "DataFolder":
                            reader.Read();
                            DataFolder = reader.Value;
                            break;
                        case "UrlList":
                            reader.Read();
                            UrlList = reader.Value;
                            break;
                    }
                }
            }
            reader.Close();
        }

        public void SaveConfiguration(string p)
        {
            XmlTextWriter writer = new XmlTextWriter(p, Encoding.ASCII);
            writer.WriteStartElement("Configuration");
            
            writer.WriteStartElement("DataFolder");
            writer.WriteValue(DataFolder);
            writer.WriteEndElement();

            writer.WriteStartElement("UrlList");
            writer.WriteValue(UrlList);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Close();
        }
    }
}
