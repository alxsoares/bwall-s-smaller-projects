using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Encryption
{
    public class Encryption
    {
        int[] S = new int[256];
        int i = 0;
        int j = 0;

        void Swap()
        {
            int tempi = S[i];
            S[i] = S[j];
            S[j] = tempi;
        }

        public void SetKey(string key)
        {
            SetKey(Encoding.UTF8.GetBytes(key));
        }

        byte[] GenKey(byte[] key)
        {
            for (int x = 0; x < 10000; x++)
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                sha1.TransformBlock(key, 0, key.Length, key, 0);
                key = sha1.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes("fuckoff"), 0, 7);
                sha1.Clear();
            }
            return key;
        }

        public void SetKey(byte[] key)
        {
            key = GenKey(key);
            i = 0;
            j = 0;
            for (i = 0; i < 256; i++)
            {
                S[i] = i;
            }
            for (i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                Swap();
            }
            i = 0;
            j = 0;
        }

        public byte[] CryptData(byte[] data)
        {
            return CryptData(data, data.Length);
        }

        public byte[] CryptData(byte[] data, int length)
        {
            for (int x = 0; x < length; x++)
            {
                i = (i + 1) % 256;
                j = (j + S[i]) % 256;
                Swap();
                data[x] = (byte)(data[x] ^ (byte)S[(S[i] + S[j]) % 256]);
            }
            return data;
        }
    }
}
