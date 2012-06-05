using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RemoteClient
{
    public partial class ConnectToCore : Form
    {
        public string server;
        public string user;
        public string password;

        public ConnectToCore()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            server = textBoxServer.Text;
            user = textBoxUser.Text;
            password = textBoxPassword.Text;
            this.Close();
        }
    }
}
