using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PHPShellGui
{
    public partial class AddURLForm : Form
    {
        public string newURL;
        public string Password = "DefaultPassword";
        public string PreString = "PreString";

        public AddURLForm()
        {
            InitializeComponent();
        }

        private void AddURLForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            newURL = textBoxNewURL.Text;
            if (textBoxPassword.Text != "")
                Password = textBoxPassword.Text;
            if (textBoxPreString.Text != "")
                PreString = textBoxPreString.Text;
            this.Close();
        }
    }
}
