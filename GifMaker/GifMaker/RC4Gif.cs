using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;

namespace GifMaker
{
    class RC4Gif
    {
        class ARC4
        {
            public byte[] S = new byte[256];
            public int i, j;

            private void Swap()
            {
                byte temp = S[i];
                S[i] = S[j];
                S[j] = temp;
            }

            public ARC4()
            {
            }
            
            public Image[] Init(byte[] key)
            {
                List<Image> images = new List<Image>();
                for (i = 0; i < 256; i++)
                    S[i] = (byte)i;
                j = 0;
                images.Add(RC4Gif.GenerateImageFromSIJ(S, 0, 0));
                for (i = 0; i < 256; i++)
                {
                    j = (j + S[i] + key[i % key.Length]) & 0xff;
                    Swap();
                    images.Add(RC4Gif.GenerateImageFromSIJ(S, i, j));
                }
                i = j = 0;
                return images.ToArray();
            }

            public byte NextByte()
            {
                i = (i + 1) & 0xff;
                j = (j + S[i]) & 0xff;
                Swap();
                return S[(S[i] + S[j]) & 0xff];
            }

            public byte[] CryptBytes(byte[] input)
            {
                for (int x = 0; x < input.Length; x++)
                    input[x] ^= NextByte();
                return input;
            }
        }

        ARC4 rc4;

        public RC4Gif(string pass)
        {
            rc4 = new ARC4();
            foreach (Image i in rc4.Init(Encoding.ASCII.GetBytes(pass)))
                images.Enqueue(i);
        }

        public static Image GeneratePBKDF2Image(string password, string salt, string interations, string iteration, byte[] hash)
        {
            Bitmap bm = new Bitmap(250, 250);
            Font arial = new Font("Monospace", 8);
            Graphics graphics = Graphics.FromImage(bm);
            SolidBrush background = new SolidBrush(Color.Black);
            graphics.FillRectangle(background, 0, 0, 250, 250);
            background.Dispose();
            SolidBrush foreground = new SolidBrush(Color.White);
            SolidBrush highlight = new SolidBrush(Color.Red);
            SolidBrush key = new SolidBrush(Color.Gold);

            graphics.DrawString("Password: " + password, arial, foreground, new PointF(3, 15f * (0) + 3));
            graphics.DrawString("Salt: " + salt, arial, foreground, new PointF(3, 15f * (1) + 3));
            graphics.DrawString("Iterations: " + interations, arial, foreground, new PointF(3, 15f * (2) + 3));
            graphics.DrawString("Iteration: " + iteration, arial, foreground, new PointF(3, 15f * (3) + 3));
            graphics.DrawString("Hash: ", arial, foreground, new PointF(3, 15f * (13) + 3));
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    string val = Convert.ToString(hash[y + (16 * x)], 16);
                    if (val.Length == 1)
                        val = "0" + val;
                    graphics.DrawString(val.ToUpper(), arial, foreground, new PointF(15f * y + 3, 15f * (14 + x) + 3));
                }
            }
            return bm;
        }

        public static Image[] GenerateImagesForPBKDF2()
        {
            List<Image> images = new List<Image>();            
            byte[] input = Encoding.ASCII.GetBytes("password");
            byte[] Salt = Encoding.ASCII.GetBytes("Salt");
            byte[] derived = new HMACSHA256(Salt).ComputeHash(input);
            byte[] temp = derived;
            images.Add(GeneratePBKDF2Image("password", "Salt", "10000", "0", derived));
            for (int x = 0; x < 10000; x++)
            {
                temp = new HMACSHA256(temp).ComputeHash(input);
                for (int y = 0; y < derived.Length; y++)
                {
                    derived[y] ^= temp[y];
                }
                images.Add(GeneratePBKDF2Image("password", "Salt", "10000", x.ToString(), derived));
            }
            return images.ToArray();
        }

        static Image GenerateImageFromSIJ(byte[] S, int i, int j)
        {
            Bitmap bm = new Bitmap(250, 250);
            Graphics graphics = Graphics.FromImage(bm);
            SolidBrush background = new SolidBrush(Color.Black);
            graphics.FillRectangle(background, 0, 0, 250, 250);
            background.Dispose();
            SolidBrush foreground = new SolidBrush(Color.White);
            SolidBrush highlight = new SolidBrush(Color.Red);
            SolidBrush key = new SolidBrush(Color.Gold);
            Font arial = new Font("Monospace", 8);
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    string val = Convert.ToString(S[x + (16 * y)], 16);
                    if (val.Length == 1)
                        val = "0" + val;
                    if (x + (16 * y) == ((i + j) & 0xff))
                    {
                        graphics.DrawString(val.ToUpper(), arial, key, new PointF(15f * x + 3, 15f * y + 3));
                    }
                    else if (x + (16 * y) == i || x + (16 * y) == j)
                    {
                        graphics.DrawString(val.ToUpper(), arial, highlight, new PointF(15f * x + 3, 15f * y + 3));
                    }
                    else
                    {
                        graphics.DrawString(val.ToUpper(), arial, foreground, new PointF(15f * x + 3, 15f * y + 3));
                    }
                }
            }
            foreground.Dispose();
            highlight.Dispose();
            key.Dispose();
            return bm;
        }

        Queue<Image> images = new Queue<Image>();

        public Image GetNextImage()
        {
            if (images.Count == 0)
            {
                rc4.NextByte();
                return GenerateImageFromSIJ(rc4.S, rc4.i, rc4.j);
            }
            else
            {
                return images.Dequeue();
            }
        }
    }
}
