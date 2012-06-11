using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NET_Source_Protection
{
    public class RC4a
    {
        byte[] s = new byte[256];
        int i, j, k;

        void swap(int a, int b)
        {
            byte temp = s[a];
            s[a] = s[b];
            s[b] = temp;
        }

        public RC4a(byte[] key)
        {
            for (int x = 0; x < 256; x++)
                s[x] = (byte)x;

            for (i = j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) % 256;
                swap(i, j);
            }
            i = j = k = 0;
        }

        byte Output()
        {
            i = (i + 1) % 256;
            j = (j + s[i]) % 256;
            k = (k + s[j]) % 256;

            swap(i, j);
            swap(i, k);

            return s[(s[(s[i] + s[j]) % 256] + s[k]) % 256];
        }

        public string EncryptAndEncode(byte[] input)
        {
            for (int x = 0; x < input.Length; x++)
            {
                input[x] = (byte)(input[x] ^ Output());
            }
            return Convert.ToBase64String(input);
        }

        public byte[] DecodeAndDecrypt(string input)
        {
            byte[] temp = Convert.FromBase64String(input);
            for (int x = 0; x < temp.Length; x++)
            {
                temp[x] = (byte)(temp[x] ^ Output());
            }
            return temp;
        }
    }

    public class Encryption
    {
        public static byte[] Decrypt(string encoded, byte[] key)
        {
            RC4a rc4a = new RC4a(key);
            return rc4a.DecodeAndDecrypt(encoded);
        }

        public static string Encrypt(byte[] input, byte[] key)
        {
            RC4a rc4a = new RC4a(key);
            return rc4a.EncryptAndEncode(input);
        }
    }
}
