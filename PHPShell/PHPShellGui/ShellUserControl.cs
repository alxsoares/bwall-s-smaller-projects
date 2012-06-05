using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PHPShell;

namespace PHPShellGui
{
    public partial class ShellUserControl : UserControl
    {
        ShellURL shellURL;
        PHPShell.PHPShell shell;

        public ShellUserControl(ShellURL url)
        {
            shellURL = url;
            shell = new PHPShell.PHPShell(url);
            shell.NewStatusToReport += new StatusEvent(shell_NewStatusToReport);
            InitializeComponent();
        }

        delegate void StringDelegate(string str);

        void shell_NewStatusToReport(string status)
        {
            if (textBoxStatus.InvokeRequired)
            {
                StringDelegate sd = new StringDelegate(shell_NewStatusToReport);
                textBoxStatus.Invoke(sd, new object[1] { status });
            }
            else
            {
                bool scrollToBottom = true;
                if (textBoxStatus.SelectionStart != textBoxStatus.Text.Length)
                    scrollToBottom = false;
                textBoxStatus.Text += status + "\r\n";
                if (scrollToBottom)
                {
                    textBoxStatus.SelectionStart = textBoxStatus.Text.Length;
                    textBoxStatus.ScrollToCaret();
                }
            }
        }

        private void ShellUserControl_Load(object sender, EventArgs e)
        {
            //Possibly do some diagnostics here
            //Load previously read shit
            shell.LoadPreviousCommands();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxCommand.Text != "")
            {
                bool scrollToBottom = true;
                if (textBoxOutput.SelectionStart != textBoxOutput.Text.Length)
                    scrollToBottom = false;
                textBoxOutput.Text += shell.ProcessCommand(textBoxCommand.Text) + "\r\n";
                if (scrollToBottom)
                {
                    textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
                    textBoxOutput.ScrollToCaret();
                }
                textBoxCommand.Text = "";
            }
        }

        private void textBoxCommand_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Tab:
                    string[] possiblities = shell.TabComplete(textBoxCommand.Text);
                    if (possiblities.Length == 1)
                        textBoxCommand.Text = possiblities[0];
                    else if (possiblities.Length > 1)
                    {
                        foreach (string p in possiblities)
                            textBoxOutput.Text += p + " ";
                        textBoxOutput.Text += "\r\n";
                    }
                    e.Handled = true;
                    break;
                case Keys.Up:
                    shell.AddCommand(textBoxCommand.Text);
                    textBoxCommand.Text = shell.GetLastCommand();
                    e.Handled = true;
                    break;
                case Keys.Down:
                    shell.AddCommand(textBoxCommand.Text);
                    textBoxCommand.Text = shell.GetNextCommand();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    buttonSend_Click(null, null);
                    e.Handled = true;
                    break;
            }
        }

        private void textBoxCommand_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
