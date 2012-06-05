using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PHPShell;

namespace PHPShellGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        delegate void StringParam(ShellURL param);

        BindingSource URLs = new BindingSource();

        void AddURL(ShellURL url)
        {
            if (dataGridView1.InvokeRequired)
            {
                StringParam sp = new StringParam(AddURL);
                dataGridView1.Invoke(sp, new object[1] { url });
            }
            else
            {
                URLs.Add(url);
            }
        }

        private void buttonAddURL_Click(object sender, EventArgs e)
        {
            AddURLForm auf = new AddURLForm();
            if (auf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ShellURL newURL = new ShellURL(auf.newURL, auf.Password, auf.PreString);
                AddURL(newURL);
                ShellList.Instance.ShellURLs.Add(newURL);
                ShellList.Instance.SaveURLList();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = URLs;
            ShellList.Instance.LoadURLList();
            foreach (ShellURL shell in ShellList.Instance.ShellURLs)
            {
                AddURL(shell);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.RowIndex < URLs.Count)
            {
				foreach(TabPage tpg in tabControl1.TabPages)
				{
					if(tpg.Text == ((ShellURL)URLs[e.RowIndex]).URL)
						return;
				}
                ShellUserControl suc = new ShellUserControl((ShellURL)URLs[e.RowIndex]);
                suc.Dock = DockStyle.Fill;
                TabPage tp = new TabPage(((ShellURL)URLs[e.RowIndex]).URL);
                tp.Controls.Add(suc);
                tabControl1.TabPages.Add(tp);
                tabControl1.SelectedTab = tp;
            }
        }
    }
}
