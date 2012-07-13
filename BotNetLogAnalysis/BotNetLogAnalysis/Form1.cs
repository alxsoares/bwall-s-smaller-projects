using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BotNetLogAnalysis
{
    public partial class Form1 : Form
    {
        Dictionary<string, Dictionary<string, int>> AttackerToPayload = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, Dictionary<string, int>> PayloadToAttacker = new Dictionary<string, Dictionary<string, int>>();
        class NodeSorter : IComparer
        {

            public int Compare(object x, object y)
            {
                return ((TreeNode)y).Text.CompareTo(((TreeNode)x).Text);
            }
        }

        public Form1()
        {
            InitializeComponent();
            treeViewBotToNet.TreeViewNodeSorter = new NodeSorter();
            if (File.Exists("archive.txt"))
            {
                foreach (string line in File.ReadAllLines("archive.txt"))
                    ProcessLine(line);
            }
            GenAttackerToPayload();
        }

        private void GenAttackerToPayload()
        {
            treeViewBotToNet.Nodes.Clear();
            TreeNode root = new TreeNode("Attacker to Payload [" + AttackerToPayload.Count + "]");
            foreach (KeyValuePair<string, Dictionary<string, int>> pair in AttackerToPayload)
            {
                TreeNode attacker = new TreeNode("[" + pair.Value.Count.ToString("D8") + "] " + pair.Key);
                foreach (KeyValuePair<string, int> payload in pair.Value)
                    attacker.Nodes.Add("[" + payload.Value.ToString("D8") + "] " + payload.Key);
                root.Nodes.Add(attacker);
            }
            treeViewBotToNet.Nodes.Add(root);
            GenPayloadToAttacker();
            treeViewBotToNet.Sort();
        }

        private void GenPayloadToAttacker()
        {
            TreeNode root = new TreeNode("Payload to Attacker [" + PayloadToAttacker.Count + "]");
            foreach (KeyValuePair<string, Dictionary<string, int>> pair in PayloadToAttacker)
            {
                TreeNode attacker = new TreeNode("[" + pair.Value.Count.ToString("D8") + "] " + pair.Key);
                foreach (KeyValuePair<string, int> payload in pair.Value)
                    attacker.Nodes.Add("[" + payload.Value.ToString("D8") + "] " + payload.Key);
                root.Nodes.Add(attacker);
            }
            
            treeViewBotToNet.Nodes.Add(root);
        }

        private void ProcessLine(string line)
        {
            line = Uri.UnescapeDataString(line);
            Regex regex = new Regex(@"(?<attacker>[\S]+).*=(?<payload>https?://[^/]+)");
            if (regex.IsMatch(line))
            {
                Match m = regex.Match(line);
                if (!AttackerToPayload.ContainsKey(m.Groups["attacker"].Value))
                {
                    AttackerToPayload[m.Groups["attacker"].Value] = new Dictionary<string, int>();
                }
                if (!AttackerToPayload[m.Groups["attacker"].Value].ContainsKey(m.Groups["payload"].Value))
                    AttackerToPayload[m.Groups["attacker"].Value].Add(m.Groups["payload"].Value, 0);
                AttackerToPayload[m.Groups["attacker"].Value][m.Groups["payload"].Value]++;

                if (!PayloadToAttacker.ContainsKey(m.Groups["payload"].Value))
                {
                    PayloadToAttacker[m.Groups["payload"].Value] = new Dictionary<string, int>();
                }
                if (!PayloadToAttacker[m.Groups["payload"].Value].ContainsKey(m.Groups["attacker"].Value))
                    PayloadToAttacker[m.Groups["payload"].Value].Add(m.Groups["attacker"].Value, 0);
                PayloadToAttacker[m.Groups["payload"].Value][m.Groups["attacker"].Value]++;
            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            foreach (string line in textBoxInput.Text.Split("\n".ToCharArray()))
            {
                ProcessLine(line);                
            }
            File.AppendAllLines("archive.txt", textBoxInput.Text.Split("\n".ToCharArray()));
            textBoxInput.Text = "";
            GenAttackerToPayload();
        }
    }
}
