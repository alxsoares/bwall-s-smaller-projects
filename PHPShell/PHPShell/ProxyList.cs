using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace PHPShell
{
	class ProxyList
	{
        static ProxyList instance = null;
        static readonly object padlock = new object();

        public static ProxyList Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ProxyList();
                    return instance;
                }
            }
        }

        List<Proxy> proxies = new List<Proxy>();

        ProxyList()
        {

        }
        
	}

    class Credentials : ICredentials
    {

        #region ICredentials Members

        string user;
        string pass;

        public Credentials(string user, string pass)
        {
            this.user = user;
            this.pass = pass;
        }

        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            return new NetworkCredential(user, pass);
        }

        #endregion
    }

    class Proxy : IWebProxy
    {
        Credentials creds = null;
        Uri uri;

        public Proxy(Uri uri)
        {
            this.uri = uri;
        }

        #region IWebProxy Members

        public ICredentials Credentials
        {
            get
            {
                return creds;
            }
            set
            {
                creds = (Credentials)value;
            }
        }

        public Uri GetProxy(Uri destination)
        {
            return uri;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public Uri URI
        {
            get
            {
                return uri;
            }
        }

        #endregion
    }
}
