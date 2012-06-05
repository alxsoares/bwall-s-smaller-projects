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
    public partial class Form1 : Form
    {
        MessageCenter mc;
        string currentServer;
        string currentChan;
        readonly object updatelock = new object();

        public Form1(MessageCenter m)
        {
            mc = m;
            mc.nMessage += new MessageCenter.NewMessage(mc_nMessage);
            mc.Connect();
            InitializeComponent();
        }

        void mc_nMessage(string server, string channel)
        {
            lock (updatelock)
            {
                UpdateTreeView();
                if (currentServer == server && currentChan == channel)
                {
                    UpdateCurrentView();
                }
            }
        }

        private void UpdateCurrentView()
        {
            if (textBoxMessages.InvokeRequired)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(UpdateCurrentView);
                textBoxMessages.Invoke(ts);
            }
            else
            {
                textBoxMessages.Text = mc.GetChanText(currentServer, currentChan);
                textBoxMessages.SelectionStart = textBoxMessages.Text.Length - 1;
                textBoxMessages.ScrollToCaret();
            }
        }

        private void UpdateTreeView()
        {
            if (treeViewServers.InvokeRequired)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(UpdateTreeView);
                treeViewServers.Invoke(ts);
            }
            else
            {
                treeViewServers.Nodes.Clear();
                TreeNode root = mc.GetServerList();
                foreach (TreeNode tn in root.Nodes)
                {
                    treeViewServers.Nodes.Add(tn);
                }
                treeViewServers.ExpandAll();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void treeViewServers_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                currentServer = e.Node.Parent.Text;
                currentChan = e.Node.Text;
                UpdateCurrentView();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mc.Disconnect();
        }

        private void textBoxToSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && textBoxToSend.Text != "")
            {
                mc.SendCommand(currentServer, currentChan, textBoxToSend.Text);
                textBoxToSend.Text = "";
                e.Handled = true;
            }
        }

    }
}
