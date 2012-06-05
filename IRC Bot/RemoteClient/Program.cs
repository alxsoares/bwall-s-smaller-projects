using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RemoteClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MessageCenter mc = new MessageCenter();

            ConnectToCore ctc = new ConnectToCore();
            ctc.ShowDialog();
            mc.server = ctc.server;
            mc.user = ctc.user;
            mc.password = ctc.password;
            Form1 f1 = new Form1(mc);
            f1.ShowDialog();
        }
    }
}
