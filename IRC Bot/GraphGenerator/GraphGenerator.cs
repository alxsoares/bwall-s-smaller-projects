using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GraphGenerator
{
    public static class GraphGenerator
    {
        public static void GenerateLineGraph(int width, int height, uint maxValue, uint[] values, string saveLocation)
        {
            int[] v = new int[values.Length];
            for (int x = 0; x < values.Length; x++)
                v[x] = (int)values[x];
            GenerateLineGraph(width, height, (int)maxValue, v, saveLocation, 30, 20, 10);
        }

        public static void GenerateLineGraph(int width, int height, int maxValue, int[] values, string saveLocation, int leftBuffer, int bottomBuffer, int low)
        {
            Bitmap bm = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bm);
            SolidBrush brush = new SolidBrush(Color.Aquamarine);
            graphics.FillRectangle(brush, leftBuffer, 0, width, height - bottomBuffer);
            brush.Dispose();

            SolidBrush foreground = new SolidBrush(Color.DarkCyan);
            Pen p = new Pen(foreground);
            Point lastp = new Point(leftBuffer, (height - bottomBuffer) - (int)(((float)values[0] / maxValue) * (height - bottomBuffer)));
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(leftBuffer, height - bottomBuffer), lastp);
            Point[] curve = new Point[values.Length];
            curve[0] = lastp;
            for (int x = 0; x < values.Length; x++)
            {
                Point top = new Point(leftBuffer + (int)(((float)(width - leftBuffer) / (float)(values.Length)) * x), 0);
                Point bottom = new Point(leftBuffer + (int)(((float)(width - leftBuffer) / (float)(values.Length)) * x), height);
                graphics.DrawLine(p, top, bottom);
                Point pnt = new Point(leftBuffer + (int)(((float)(width - leftBuffer) / (float)(values.Length)) * x), (height - bottomBuffer) - (int)(((float)values[x] / maxValue) * (height - bottomBuffer)));
                curve[x] = pnt;
                lastp = pnt;
            }
            curve[values.Length - 1] = new Point(width, (height - bottomBuffer) - (int)(((float)values[values.Length - 1] / maxValue) * (height - bottomBuffer)));
            gp.AddCurve(curve);
            gp.AddLine(curve[values.Length - 1], new Point(width, (height - bottomBuffer)));
            gp.AddLine(new Point(leftBuffer, height), new Point(width, (height - bottomBuffer)));
            LinearGradientBrush lgb = new LinearGradientBrush(new Point(width / 2, (height - bottomBuffer)), new Point(width / 2, 0), Color.DarkBlue, Color.Cyan);
            graphics.FillRegion(lgb, new Region(gp));
            for (int x = 0; x < values.Length; x++)
            {
                SizeF stringSize = new SizeF();
                Font arial = new Font("Monospace", 12);
                stringSize = graphics.MeasureString(x.ToString(), arial);
                int shift = (int)(((float)(width - leftBuffer) / (float)(values.Length)) / 2) - (int)(stringSize.Width / 2);
                Point bottom = new Point(leftBuffer + (int)(((float)(width - leftBuffer) / (float)(values.Length)) * x) + shift, height - bottomBuffer);                    
                graphics.DrawString(x.ToString(), arial, foreground, bottom);
            }
            p.Dispose();
            foreground.Dispose();
            bm.Save(saveLocation, ImageFormat.Bmp);
        }
    }
}
