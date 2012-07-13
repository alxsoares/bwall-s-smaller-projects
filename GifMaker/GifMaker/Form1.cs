using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace GifMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        delegate void UpdateImage(Image i);

        void Up(Image I)
        {
            if (pictureBox1.InvokeRequired)
            {
                UpdateImage ui = new UpdateImage(Up);
                pictureBox1.Invoke(ui, I);
            }
            else
            {
                pictureBox1.Image = I;
            }
        }

        void SName()
        {            
            if (Directory.Exists("pbk"))
                Directory.Delete("pbk", true);
            if (!Directory.Exists("pbk"))
                Directory.CreateDirectory("pbk");
            byte[] input = Encoding.ASCII.GetBytes("password");
            byte[] Salt = Encoding.ASCII.GetBytes("Salt");
            byte[] derived = new HMACSHA256(Salt).ComputeHash(input);
            byte[] temp = derived;
            RC4Gif.GeneratePBKDF2Image("password", "Salt", "1000", "0", derived).Save("pbk" + Path.DirectorySeparatorChar + (0).ToString("D8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            for (int x = 0; x < 1000; x++)
            {
                temp = new HMACSHA256(temp).ComputeHash(input);
                for (int y = 0; y < derived.Length; y++)
                {
                    derived[y] ^= temp[y];
                }
                RC4Gif.GeneratePBKDF2Image("password", "Salt", "1000", x.ToString(), derived).Save("pbk" + Path.DirectorySeparatorChar + (x + 1).ToString("D8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        void GName()
        {
            if (Directory.Exists("name"))
                Directory.Delete("name", true);
            if (!Directory.Exists("name"))
                Directory.CreateDirectory("name");
            Image temp = nameGif.GenerateImageFromS(" ");
            string name = "bwallHatesTwits";
            temp.Save("name" + Path.DirectorySeparatorChar + "000.png", System.Drawing.Imaging.ImageFormat.Png);
            Up(temp);
            for (int x = 0; x < name.Length; x++)
            {
                Thread.Sleep(100);
                temp = nameGif.GenerateImageFromS(name[x] + "");
                temp.Save("name" + Path.DirectorySeparatorChar + (1 + x).ToString("D3") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                Up(temp);
            }
        }

        void RC()
        {
            if (Directory.Exists("RC"))
                Directory.Delete("RC", true);
            if (!Directory.Exists("RC"))
                Directory.CreateDirectory("RC");
            RC4Gif gif = new RC4Gif("bwallHatesTwits");
            Image temp = gif.GetNextImage();
            temp.Save("RC" + Path.DirectorySeparatorChar + "000.png", System.Drawing.Imaging.ImageFormat.Png);
            Up(temp);
            for (int x = 1; x < 257; x++)
            {
                Thread.Sleep(10);
                temp = gif.GetNextImage();
                temp.Save("RC" + Path.DirectorySeparatorChar + x.ToString("D3") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                Up(temp);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Text = "Running";
            Thread t = new Thread(SName);
            t.Start();
            t.Join();
            this.Text = "Done";
        }
    }
}
