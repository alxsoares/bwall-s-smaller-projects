using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ModularIrcBot
{
    public static class EncFile
    {
        class RC4Encryption
        {
            byte[] S = new byte[256];
            int i = 0, j = 0;
            static SHA256Managed sha = new SHA256Managed();

            void swap(int x, int y)
            {
                byte t = S[x];
                S[x] = S[y];
                S[y] = t;
            }

            public RC4Encryption(string password)
            {
                for (int x = 0; x < 256; x++)
                {
                    password = BitConverter.ToString(sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password))).Replace("-", x.ToString());
                    S[x] = (byte)x;
                }
                for (i = j = 0; i < 256; i++)
                {
                    j = (j + password[i % password.Length] + S[i]) % 256;
                    swap(i, j);
                }
                i = j = 0;
            }

            public byte NextByte()
            {
                i = (i + 1) % 256;
                j = (j + S[i]) % 256;
                swap(i, j);
                return S[(S[i] + S[j]) % 256];
            }
        }

        public static void EncryptFile(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            RC4Encryption enc = new RC4Encryption(Config.I.FilePassword);
            while (stream.Position < stream.Length)
            {
                byte output = (byte)stream.ReadByte();
                stream.Position--;
                stream.WriteByte((byte)(output ^ enc.NextByte()));
            }
            stream.Close();
        }

        public static void WriteAllText(string path, string text)
        {
            if (Config.I.EncryptFiles)
            {
                FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                stream.WriteByte(0);
                RC4Encryption enc = new RC4Encryption(Config.I.FilePassword);
                for (int x = 0; x < text.Length; x++)
                {
                    stream.WriteByte((byte)(text[x] ^ enc.NextByte()));
                }
                stream.Close();
            }
            else
            {
                FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                for (int x = 0; x < text.Length; x++)
                {
                    stream.WriteByte((byte)(text[x]));
                }
                stream.Close();
            }
        }

        public static void WriteAllLines(string path, string[] lines)
        {
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            if (Config.I.EncryptFiles)
            {
                stream.WriteByte(0);
                RC4Encryption enc = new RC4Encryption(Config.I.FilePassword);
                foreach (string line in lines)
                {
                    for (int x = 0; x < line.Length; x++)
                    {
                        stream.WriteByte((byte)(line[x] ^ enc.NextByte()));
                    }
                    stream.WriteByte((byte)("\n".ToCharArray()[0] ^ enc.NextByte()));
                }
            }
            else
            {
                foreach (string line in lines)
                {
                    for (int x = 0; x < line.Length; x++)
                    {
                        stream.WriteByte((byte)(line[x]));
                    }
                    stream.WriteByte((byte)("\n".ToCharArray()[0]));
                }
            }
            stream.Close();
        }

        public static void AppendText(string path, string text)
        {
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (stream.ReadByte() == 0)
            {
                RC4Encryption enc = new RC4Encryption(Config.I.FilePassword);
                for (int x = 0; x < stream.Length; x++)
                    enc.NextByte();
                stream.Seek(0, SeekOrigin.End);
                for (int x = 0; x < text.Length; x++)
                {
                    stream.WriteByte((byte)(text[x] ^ enc.NextByte()));
                }
            }
            else
            {
                stream.Seek(0, SeekOrigin.End);
                for (int x = 0; x < text.Length; x++)
                {
                    stream.WriteByte((byte)(text[x]));
                }
            }
            stream.Close();
        }

        public static string ReadAllText(string path)
        {
            string plain = "";
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.ReadByte() == 0)
            {
                //Encrypted
                RC4Encryption enc = new RC4Encryption(Config.I.FilePassword);
                int read = 0;
                while (read != -1 && stream.Position < stream.Length)
                {
                    read = stream.ReadByte();
                    plain += (char)(read ^ enc.NextByte());
                }
            }
            else
            {
                //Plain                
                int read = 0;
                stream.Seek(0, SeekOrigin.Begin);
                while (read != -1 && stream.Position < stream.Length)
                {
                    read = stream.ReadByte();
                    plain += (char)read;
                }
            }
            stream.Close();
            return plain;
        }

        public static string[] ReadAllLines(string path)
        {
            List<string> plain = new List<string>();
            string plainBuf = "";
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (stream.ReadByte() == 0)
            {
                //Encrypted
                RC4Encryption enc = new RC4Encryption(Config.I.FilePassword);
                int read = 0;
                while (read != -1 && stream.Position < stream.Length)
                {
                    read = stream.ReadByte();
                    char p = (char)(read ^ enc.NextByte());
                    if (p == "\n".ToCharArray()[0])
                    {
                        if (plainBuf.EndsWith("\r"))
                            plainBuf.Remove(plainBuf.LastIndexOf("\r"));
                        if (!string.IsNullOrEmpty(plainBuf))
                            plain.Add(plainBuf);
                        plainBuf = "";
                    }
                    else
                        plainBuf += p;
                }
            }
            else
            {
                //Plain
                stream.Seek(0, SeekOrigin.Begin);
                int read = 0;
                while (read != -1 && stream.Position < stream.Length)
                {
                    read = stream.ReadByte();
                    char p = (char)(read);
                    if (p == "\n".ToCharArray()[0])
                    {
                        if (plainBuf.EndsWith("\r"))
                            plainBuf.Remove(plainBuf.LastIndexOf("\r"));
                        if (!string.IsNullOrEmpty(plainBuf))
                            plain.Add(plainBuf);
                        plainBuf = "";
                    }
                    else
                        plainBuf += p;
                }
            }
            stream.Close();
            if (!string.IsNullOrEmpty(plainBuf))
                plain.Add(plainBuf);
            return plain.ToArray();
        }
    }
}
