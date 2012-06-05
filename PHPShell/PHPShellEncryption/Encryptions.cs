using System;
using System.Text;

namespace PHPShellEncryption
{
	public class RC4
	{
		byte[] s = new byte[256];
		int i, j, k;
		
		void swap(int a, int b)
		{
			byte temp = s[a];
			s[a] = s[b];
			s[b] = temp;
		}
		
		public RC4(byte[] key)
		{
			for(int x = 0; x < 256; x++)
				s[x] = (byte)x;
			
			for(i = j = 0; i < 256; i++)
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
			for(int x = 0; x < input.Length; x++)
			{
				input[x] = (byte)(input[x] ^ Output());	
			}
			return Convert.ToBase64String(input);
		}
		
		public byte[] DecodeAndDecrypt(byte[] input)
		{
			byte[] temp = Convert.FromBase64String(ASCIIEncoding.ASCII.GetString(input));
			for(int x = 0; x < temp.Length; x++)
			{
				temp[x] = (byte)(temp[x] ^ Output());	
			}
			return temp;
		}
	}
}
