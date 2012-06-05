using System;
using System.Collections.Generic;
using System.Text;

namespace PHPShell
{
    public class ShellURL
    {
        string url;
        string password;
        string preString;

        public ShellURL(string url, string password, string preString)
        {
            this.url = url;
            this.password = password;
            this.preString = preString;
        }

        public string URL
        {
            get { return url; }
        }

        public string Password
        {
            get { return password; }
        }

        public string PreString
        {
            get { return preString; }
        }
    }
}
