using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GifMaker
{
    class nameGif
    {
        public static Image[] GenerateStreamingImagesFromS(string s)
        {
            List<Image> images = new List<Image>();
            Bitmap b = new Bitmap(250, 250);
            Font a = new Font("Monospace", 200, FontStyle.Bold);
            int width = 250 + (int)Graphics.FromImage(b).MeasureString(s, a).Width;
            for (int x = 0; x < width; x += 50)
            {
                Bitmap bm = new Bitmap("eye.jpg");
                Graphics graphics = Graphics.FromImage(bm);
                SolidBrush foreground = new SolidBrush(Color.FromArgb(200, Color.Blue));
                Pen linePen = new Pen(new SolidBrush(Color.Firebrick), 20);
                Pen oPen = new Pen(new SolidBrush(Color.DarkRed), 20);
                List<Point> points = new List<Point>();
                List<Point> opoints = new List<Point>();
                for (int y = 0; y < width + 250; y += 500)
                {
                    points.Add(new Point(y - x, 0));
                    opoints.Add(new Point(y - x, 250));
                    points.Add(new Point(y - x + 250, 250));
                    opoints.Add(new Point(y - x + 250, 0));
                }
                graphics.DrawCurve(linePen, points.ToArray());
                graphics.DrawCurve(oPen, opoints.ToArray());
                linePen.Dispose();
                Font arial = new Font("Monospace", 200, FontStyle.Bold);
                graphics.DrawString(s, arial, foreground, new PointF(250 - x, 0));
                foreground.Dispose();
                images.Add(bm);
            }
            return images.ToArray();
        }

        public static Image GenerateImageFromS(string s)
        {
            Bitmap bm = new Bitmap(250, 250);
            Graphics graphics = Graphics.FromImage(bm);
            SolidBrush background = new SolidBrush(Color.Transparent);
            graphics.FillRectangle(background, 0, 0, 250, 250);
            background.Dispose();
            SolidBrush foreground = new SolidBrush(Color.Red);
            Font arial = new Font("Monospace", 200, FontStyle.Bold);
            graphics.DrawString(s, arial, foreground, new PointF(0, 0));
            foreground.Dispose();
            return bm;
        }
    }
}
