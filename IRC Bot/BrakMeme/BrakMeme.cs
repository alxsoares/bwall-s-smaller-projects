using System;
using System.Collections.Generic;
using System.Text;
using ModularIrcBot;
using System.Drawing;
using System.Threading;
using System.IO;

namespace BrakMeme
{
    public class BrakMeme : Module
    {
        Dictionary<string, DateTime> lastMeme = new Dictionary<string, DateTime>();
        Random rand = new Random();

        public override void AddBindings()
        {
            irc.OnMSGRecvd += new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        void irc_OnMSGRecvd(MSG msg)
        {
            if (lastMeme.ContainsKey(msg.fromUser + '@' + msg.fromHost))
            {
                if (msg.when.CompareTo(lastMeme[msg.fromUser + '@' + msg.fromHost].AddSeconds(30)) < 0)
                    return;
            }
            if (msg.message.ToLower().StartsWith(".meme "))
            {
                lastMeme[msg.fromUser + '@' + msg.fromHost] = DateTime.Now;
                Thread t = new Thread(new ParameterizedThreadStart(GenerateImage));
                t.Start(msg);
            }
            else if (msg.message.ToLower().StartsWith(".meme"))
            {
                irc.SendMessage(msg.to, "http://crimsonred.co.cc/");
            }
        }


        public static Font AppropriateFont(Graphics g, float minFontSize, float maxFontSize, Size layoutSize, string s, Font f, out SizeF extent)
        {
            if (maxFontSize == minFontSize)
                f = new Font(f.FontFamily, minFontSize, f.Style);

            extent = g.MeasureString(s, f);

            if (maxFontSize <= minFontSize)
                return f;

            float hRatio = layoutSize.Height / extent.Height;
            float wRatio = layoutSize.Width / extent.Width;
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = f.Size * ratio;

            if (newSize < minFontSize)
                newSize = minFontSize;
            else if (newSize > maxFontSize)
                newSize = maxFontSize;

            f = new Font(f.FontFamily, newSize, f.Style);
            extent = g.MeasureString(s, f);

            return f;
        }

        void GenerateImage(object str)
        {
            MSG msg = (MSG)str;
            msg.message = msg.message.Replace(".meme ", "");
            //Bitmap bm = new Bitmap("/var/www/stockImage.jpg");
            DirectoryInfo di = new DirectoryInfo("/var/www/memeStock");
            string file = null;
            if (msg.message.StartsWith("#"))
            {
                FileInfo[] fs = di.GetFiles("*.tags");
                if (fs != null && fs.Length != 0)
                {
                    List<FileInfo> taggedFiles = new List<FileInfo>();
                    foreach (FileInfo ffs in fs)
                    {
                        if (File.ReadAllText(ffs.FullName).ToLower().Contains(msg.message.Split(' ')[0].Replace("#", "").ToLower()))
                        {
                            taggedFiles.Add(ffs);
                        }
                    }
                    if (taggedFiles.Count != 0)
                    {
                        msg.message = msg.message.Substring(msg.message.IndexOf(' ') + 1);
                        file = taggedFiles[rand.Next(taggedFiles.Count)].FullName.Replace(".tags", ".jpg"); ;
                    }
                }
            }
            if (file == null)
            {
                FileInfo[] files = di.GetFiles("*.jpg");
                file = files[rand.Next(files.Length)].FullName;
            }
            
            Bitmap bm = new Bitmap(file);
            Graphics graphics = Graphics.FromImage(bm);
            SizeF stringSize = new SizeF();
            Font arial = new Font("Impact", 12, FontStyle.Bold);
            SizeF o;
            arial = AppropriateFont(graphics, 0, 1000, new Size(bm.Width - 10, 200), msg.message.ToUpper(), arial, out o);
            Point bottom = new Point(0, bm.Height - 100);
            SolidBrush foreground = new SolidBrush(Color.Black);
            SolidBrush w = new SolidBrush(Color.White);
            graphics.DrawString(msg.message.ToUpper(), arial, foreground, new Point(0, bm.Height - 25 - (int)o.Height));
            graphics.DrawString(msg.message.ToUpper(), arial, foreground, new Point(4, bm.Height - 25 - (int)o.Height));
            graphics.DrawString(msg.message.ToUpper(), arial, foreground, new Point(2, bm.Height - 27 - (int)o.Height));
            graphics.DrawString(msg.message.ToUpper(), arial, foreground, new Point(2, bm.Height - 23 - (int)o.Height));
            graphics.DrawString(msg.message.ToUpper(), arial, w, new Point(2, bm.Height - 25 - (int)o.Height));
            foreground.Dispose();
            w.Dispose();
            string name = DateTime.Now.Ticks.ToString() + ".jpg";
            bm.Save("/var/www/" + name);
            File.WriteAllText("/var/www/" + name.Replace(".jpg", ".info"), msg.message.ToUpper());
            //bm.Save(server.Data + Path.DirectorySeparatorChar + name);
            irc.SendMessage(msg.to, "Meme Generated at http://crimsonred.co.cc/index.php?i=" + name.Replace(".jpg", ""));
        }

        public override void RemoveBindings()
        {
            irc.OnMSGRecvd -= new IrcClient.MSGRecvd(irc_OnMSGRecvd);
        }

        public override string GetName()
        {
            return "BrakMeme";
        }

        public override string GetHelp()
        {
            return null;
        }
    }
}
