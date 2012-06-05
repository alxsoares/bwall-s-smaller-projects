#include "md5string.h"

inline unsigned int md5string::rotate_left(uint4 x, uint4 n)
{
	return (x << n) | (x >> (32-n))  ;
}

inline unsigned int md5string::F(uint4 x, uint4 y, uint4 z)
{
	return (x & y) | (~x & z);
}

inline unsigned int md5string::G(uint4 x, uint4 y, uint4 z)
{
	return (x & z) | (y & ~z);
}

inline unsigned int md5string::H(uint4 x, uint4 y, uint4 z)
{
	return x ^ y ^ z;
}

inline unsigned int md5string::I(uint4 x, uint4 y, uint4 z)
{
	return y ^ (x | ~z);
}

inline void md5string::FF(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4  s, uint4 ac)
{
 a += F(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

inline void md5string::GG(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac)
{
 a += G(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

inline void md5string::HH(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac)
{
 a += H(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

inline void md5string::II(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac)
{
 a += I(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

void md5string::MD5decode  (unsigned int *output, unsigned char *input, unsigned int len)
{

  unsigned int i, j;

  for (i = 0, j = 0; j < len; i++, j += 4)
    output[i] = ((unsigned int)input[j]) | (((unsigned int)input[j+1]) << 8) |
      (((unsigned int)input[j+2]) << 16) | (((unsigned int)input[j+3]) << 24);
}

md5string::md5string(string str, string alpha, string init, uint4 start, uint4 end)
{
	uint4 hex[104];
	uint1 temp[16];
	uint4 dstate[4];
	uint4 dA, dB, dC, dD;
	
	hex[48] = 0;
	hex[49] = 1;
	hex[50] = 2;
	hex[51] = 3;
	hex[52] = 4;
	hex[53] = 5;
	hex[54] = 6;
	hex[55] = 7;
	hex[56] = 8;
	hex[57] = 9;
	hex[65] = 10;
	hex[66] = 11;
	hex[67] = 12;
	hex[68] = 13;
	hex[69] = 14;
	hex[70] = 15;
	hex[71] = 16;
	hex[97] = 10;
	hex[98] = 11;
	hex[99] = 12;
	hex[100] = 13;
	hex[101] = 14;
	hex[102] = 15;
	hex[103] = 16;
	
	temp[0] = uint1(16*(hex[int(str[0])])+hex[int(str[1])]);
	temp[1] = uint1(16*(hex[int(str[2])])+hex[int(str[3])]);
	temp[2] = uint1(16*(hex[int(str[4])])+hex[int(str[5])]);
	temp[3] = uint1(16*(hex[int(str[6])])+hex[int(str[7])]);
	temp[4] = uint1(16*(hex[int(str[8])])+hex[int(str[9])]);
	temp[5] = uint1(16*(hex[int(str[10])])+hex[int(str[11])]);
	temp[6] = uint1(16*(hex[int(str[12])])+hex[int(str[13])]);
	temp[7] = uint1(16*(hex[int(str[14])])+hex[int(str[15])]);
	temp[8] = uint1(16*(hex[int(str[16])])+hex[int(str[17])]);
	temp[9] = uint1(16*(hex[int(str[18])])+hex[int(str[19])]);
	temp[10] = uint1(16*(hex[int(str[20])])+hex[int(str[21])]);
	temp[11] = uint1(16*(hex[int(str[22])])+hex[int(str[23])]);
	temp[12] = uint1(16*(hex[int(str[24])])+hex[int(str[25])]);
	temp[13] = uint1(16*(hex[int(str[26])])+hex[int(str[27])]);
	temp[14] = uint1(16*(hex[int(str[28])])+hex[int(str[29])]);
	temp[15] = uint1(16*(hex[int(str[30])])+hex[int(str[31])]);
	
	MD5decode(&dstate[0],&temp[0],16);
	
	dA = dstate[0] - 0x67452301;
	dB = dstate[1] - 0xefcdab89;
	dC = dstate[2] - 0x98badcfe;
	dD = dstate[3] - 0x10325476;

	cout << "Alpha:" << alpha << endl;
	
	if(init == string(""))
	{
		for(int x = 0; x < start; x++)
		{
			init = init + alpha[0];
		}
	}
	
	int number = 1;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse1(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse1(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	
	number++;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse2(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse2(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	
	number++;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse3(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse3(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	
	number++;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse4(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse4(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	
	number++;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse5(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse5(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	
	number++;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse6(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse6(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	
	number++;
	if(number >= start && number <= end)
	{
		if(number == start)
			reverse7(dA, dB, dC, dD, alpha, init);
		else{
			string temp("");
			for(int x = 0; x < number; x++)
			{
				temp = temp + alpha[0];
			}
			reverse7(dA, dB, dC, dD, alpha, string(temp));
		}
	}
	cout << "Done" << endl;
}

void inline	 md5string::reverse1	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
	unsigned int a, b, c, d, x[16];
	vector<uint4> revHash = startrev13(8,pA,pB,pC,pD);
	bool done = false;
	clock_t starttime = clock();
	
	while (!done)
	{
		  /* Round 1 */
		a = 0x67452301;
		b = 0xefcdab89;
		c = 0x98badcfe;
		d = 0x10325476;
		
		x[0] = uint4(init[0]);
		x[0] = x[0] + 0x8000;

		FF (a, b, c, d, x[0], S11, 0xd76aa478); 		/* 1 */
		FF (d, a, b, c, 0, S12, 0xe8c7b756); 			/* 2 */
		FF (c, d, a, b, 0, S13, 0x242070db); 			/* 3 */
		FF (b, c, d, a, 0, S14, 0xc1bdceee); 			/* 4 */
		FF (a, b, c, d, 0, S11, 0xf57c0faf); 			/* 5 */
		FF (d, a, b, c, 0, S12, 0x4787c62a); 			/* 6 */
		FF (c, d, a, b, 0, S13, 0xa8304613); 			/* 7 */
		FF (b, c, d, a, 0, S14, 0xfd469501); 			/* 8 */
		FF (a, b, c, d, 0, S11, 0x698098d8); 			/* 9 */
		FF (d, a, b, c, 0, S12, 0x8b44f7af); 			/* 10 */
		FF (c, d, a, b, 0, S13, 0xffff5bb1); 			/* 11 */
		FF (b, c, d, a, 0, S14, 0x895cd7be); 			/* 12 */
		FF (a, b, c, d, 0, S11, 0x6b901122); 			/* 13 */
		FF (d, a, b, c, 0, S12, 0xfd987193); 			/* 14 */
		FF (c, d, a, b, 8, S13, 0xa679438e); 			/* 15 */
		FF (b, c, d, a, 0, S14, 0x49b40821); 			/* 16 */
		
		 /* Round 2 */
		GG (a, b, c, d, 0, S21, 0xf61e2562); 			/* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); 			/* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); 			/* 19 */
		GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); 		/* 20 */
		GG (a, b, c, d, 0, S21, 0xd62f105d); 			/* 21 */
		GG (d, a, b, c, 0, S22,  0x2441453); 			/* 22 */
		GG (c, d, a, b, 0, S23, 0xd8a1e681); 			/* 23 */
		GG (b, c, d, a, 0, S24, 0xe7d3fbc8); 			/* 24 */
		GG (a, b, c, d, 0, S21, 0x21e1cde6); 			/* 25 */
		GG (d, a, b, c, 8, S22, 0xc33707d6); 			/* 26 */
		GG (c, d, a, b, 0, S23, 0xf4d50d87); 			/* 27 */
		GG (b, c, d, a, 0, S24, 0x455a14ed); 			/* 28 */
		GG (a, b, c, d, 0, S21, 0xa9e3e905); 			/* 29 */
		GG (d, a, b, c, 0, S22, 0xfcefa3f8); 			/* 30 */
		GG (c, d, a, b, 0, S23, 0x676f02d9); 			/* 31 */
		GG (b, c, d, a, 0, S24, 0x8d2a4c8a); 			/* 32 */

		/* Round 3 */
		HH (a, b, c, d, 0, S31, 0xfffa3942); 			/* 33 */
		HH (d, a, b, c, 0, S32, 0x8771f681); 			/* 34 */
		HH (c, d, a, b, 0, S33, 0x6d9d6122); 			/* 35 */
		HH (b, c, d, a, 8, S34, 0xfde5380c); 			/* 36 */
		HH (a, b, c, d, 0, S31, 0xa4beea44); 			/* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); 			/* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); 			/* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); 			/* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); 			/* 41 */
		HH (d, a, b, c, x[0], S32, 0xeaa127fa); 		/* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); 			/* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); 			/* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); 			/* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); 			/* 46 */
		if (d == revHash[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); 		/* 47 */
			if (c == revHash[2])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); 	/* 48 */
				if (b == revHash[1])
				{
					clock_t endtime = clock();
					cout<<"Crack found !!!\t\t\""<<init<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
					return;
				}
			}
		}
		if(init[0] == alpha[alpha.size() - 1])
		{
			clock_t endtime = clock();
			cout<<"1 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
			done = true;
		}else{
			init[0] = alpha[alpha.find(init[0]) + 1];
		}
	}
}

void inline	 md5string::reverse2	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
	unsigned int a, b, c, d, x[16];
	vector<uint4> revHash = startrev13(16,pA,pB,pC,pD);
	bool done = false;
	clock_t starttime = clock();

	while (!done)
	{
		a = 0x67452301;
		b = 0xefcdab89;
		c = 0x98badcfe;
		d = 0x10325476;
		
		x[0] = uint4(init[0]) + uint4(init[1])*0x100 + 0x800000;
		
		 /* Round 1 */
		FF (a, b, c, d, x[0], S11, 0xd76aa478); 		/* 1 */
		FF (d, a, b, c, 0, S12, 0xe8c7b756); 			/* 2 */
		FF (c, d, a, b, 0, S13, 0x242070db); 			/* 3 */
		FF (b, c, d, a, 0, S14, 0xc1bdceee); 			/* 4 */
		FF (a, b, c, d, 0, S11, 0xf57c0faf); 			/* 5 */
		FF (d, a, b, c, 0, S12, 0x4787c62a); 			/* 6 */
		FF (c, d, a, b, 0, S13, 0xa8304613); 			/* 7 */
		FF (b, c, d, a, 0, S14, 0xfd469501); 			/* 8 */
		FF (a, b, c, d, 0, S11, 0x698098d8); 			/* 9 */
		FF (d, a, b, c, 0, S12, 0x8b44f7af); 			/* 10 */
		FF (c, d, a, b, 0, S13, 0xffff5bb1); 			/* 11 */
		FF (b, c, d, a, 0, S14, 0x895cd7be); 			/* 12 */
		FF (a, b, c, d, 0, S11, 0x6b901122); 			/* 13 */
		FF (d, a, b, c, 0, S12, 0xfd987193); 			/* 14 */
		FF (c, d, a, b, 16, S13, 0xa679438e); 			/* 15 */
		FF (b, c, d, a, 0, S14, 0x49b40821); 			/* 16 */

		 /* Round 2 */
		GG (a, b, c, d, 0, S21, 0xf61e2562); 			/* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); 			/* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); 			/* 19 */
		GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); 		/* 20 */
		GG (a, b, c, d, 0, S21, 0xd62f105d); 			/* 21 */
		GG (d, a, b, c, 0, S22,  0x2441453); 			/* 22 */
		GG (c, d, a, b, 0, S23, 0xd8a1e681); 			/* 23 */
		GG (b, c, d, a, 0, S24, 0xe7d3fbc8); 			/* 24 */
		GG (a, b, c, d, 0, S21, 0x21e1cde6); 			/* 25 */
		GG (d, a, b, c, 16, S22, 0xc33707d6);	 		/* 26 */
		GG (c, d, a, b, 0, S23, 0xf4d50d87); 			/* 27 */
		GG (b, c, d, a, 0, S24, 0x455a14ed); 			/* 28 */
		GG (a, b, c, d, 0, S21, 0xa9e3e905); 			/* 29 */
		GG (d, a, b, c, 0, S22, 0xfcefa3f8); 			/* 30 */
		GG (c, d, a, b, 0, S23, 0x676f02d9); 			/* 31 */
		GG (b, c, d, a, 0, S24, 0x8d2a4c8a); 			/* 32 */
		
		 /* Round 3 */
		HH (a, b, c, d, 0, S31, 0xfffa3942); 			/* 33 */
		HH (d, a, b, c, 0, S32, 0x8771f681); 			/* 34 */
		HH (c, d, a, b, 0, S33, 0x6d9d6122); 			/* 35 */
		HH (b, c, d, a, 16, S34, 0xfde5380c);	 		/* 36 */
		HH (a, b, c, d, 0, S31, 0xa4beea44); 			/* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); 			/* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); 			/* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); 			/* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); 			/* 41 */
		HH (d, a, b, c, x[0], S32, 0xeaa127fa); 		/* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); 			/* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); 			/* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); 			/* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); 			/* 46 */
		if (d == revHash[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); 		/* 47 */
			if (c == revHash[2])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); 	/* 48 */
				if (b == revHash[1])
				{
					clock_t endtime = clock();
					cout<<"Crack found !!!\t\t\""<<init[0]<<init[1]<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
					return;
				}
			}
		}
		if(init[0] == alpha[alpha.size() - 1])
		{
			if(init[1] == alpha[alpha.size() - 1])
			{	
				clock_t endtime = clock();
				cout<<"2 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
				done = true;
			}else{
				init[0] = alpha[0];
				init[1] = alpha[alpha.find(init[1]) + 1];
			}
		}else{
			init[0] = alpha[alpha.find(init[0]) + 1];
		}
	}	
}

void inline	 md5string::reverse3	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
	unsigned int a, b, c, d, x[16];
	vector<uint4> revHash = startrev13(24,pA,pB,pC,pD);
	bool done = false;
	clock_t starttime = clock();

	while (!done)
	{
		a = 0x67452301;
		b = 0xefcdab89;
		c = 0x98badcfe;
		d = 0x10325476;
		
		x[0] = uint4(init[0]) + uint4(init[1])*0x100 + uint4(init[2])*0x10000 + 0x80000000;
		
		 /* Round 1 */
		FF (a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
		FF (d, a, b, c, 0, S12, 0xe8c7b756); /* 2 */
		FF (c, d, a, b, 0, S13, 0x242070db); /* 3 */
		FF (b, c, d, a, 0, S14, 0xc1bdceee); /* 4 */
		FF (a, b, c, d, 0, S11, 0xf57c0faf); /* 5 */
		FF (d, a, b, c, 0, S12, 0x4787c62a); /* 6 */
		FF (c, d, a, b, 0, S13, 0xa8304613); /* 7 */
		FF (b, c, d, a, 0, S14, 0xfd469501); /* 8 */
		FF (a, b, c, d, 0, S11, 0x698098d8); /* 9 */
		FF (d, a, b, c, 0, S12, 0x8b44f7af); /* 10 */
		FF (c, d, a, b, 0, S13, 0xffff5bb1); /* 11 */
		FF (b, c, d, a, 0, S14, 0x895cd7be); /* 12 */
		FF (a, b, c, d, 0, S11, 0x6b901122); /* 13 */
		FF (d, a, b, c, 0, S12, 0xfd987193); /* 14 */
		FF (c, d, a, b, 24, S13, 0xa679438e); /* 15 */
		FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */

		 /* Round 2 */
		GG (a, b, c, d, 0, S21, 0xf61e2562); /* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
		GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
		GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
		GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
		GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
		GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
		GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
		GG (d, a, b, c, 24, S22, 0xc33707d6); /* 26 */
		GG (c, d, a, b, 0, S23, 0xf4d50d87); /* 27 */
		GG (b, c, d, a, 0, S24, 0x455a14ed); /* 28 */
		GG (a, b, c, d, 0, S21, 0xa9e3e905); /* 29 */
		GG (d, a, b, c, 0, S22, 0xfcefa3f8); /* 30 */
		GG (c, d, a, b, 0, S23, 0x676f02d9); /* 31 */
		GG (b, c, d, a, 0, S24, 0x8d2a4c8a); /* 32 */
		
		/* Round 3 */
		HH (a, b, c, d, 0, S31, 0xfffa3942); /* 33 */
		HH (d, a, b, c, 0, S32, 0x8771f681); /* 34 */
		HH (c, d, a, b, 0, S33, 0x6d9d6122); /* 35 */
		HH (b, c, d, a, 24, S34, 0xfde5380c); /* 36 */
		HH (a, b, c, d, 0, S31, 0xa4beea44); /* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
		HH (d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
		if (d == revHash[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
			if (c == revHash[2])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
				if (b == revHash[1])
				{
					clock_t endtime = clock();
					cout<<"Crack found !!!\t\t\""<<init[0]<<init[1]<<init[2]<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
					return;
				}
			}
		}
		if(init[0] == alpha[alpha.size() - 1])
		{
			if(init[1] == alpha[alpha.size() - 1])
			{	
				if(init[2] == alpha[alpha.size() - 1])
				{	
					clock_t endtime = clock();
					cout<<"3 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
					done = true;
				}else{
					init[0] = alpha[0];
					init[1] = alpha[0];
					init[2] = alpha[alpha.find(init[2]) + 1];
				}
			}else{
				init[0] = alpha[0];
				init[1] = alpha[alpha.find(init[1]) + 1];
			}
		}else{
			init[0] = alpha[alpha.find(init[0]) + 1];
		}
	}	
}

void inline	 md5string::reverse4	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
	unsigned int a, b, c, d, x[16];
	vector<uint4> revHash = startrev4(32,pA,pB,pC,pD);
	bool done = false;
	clock_t starttime = clock();

	while (!done)
	{
		a = 0x67452301;
		b = 0xefcdab89;
		c = 0x98badcfe;
		d = 0x10325476;
		
		x[0] = uint4(init[0]) + uint4(init[1])*0x100 + uint4(init[2])*0x10000 + uint4(init[3])*0x1000000;
		x[1] = 0x80;
		
		 /* Round 1 */
		FF (a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
		FF (d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
		FF (c, d, a, b, 0, S13, 0x242070db); /* 3 */
		FF (b, c, d, a, 0, S14, 0xc1bdceee); /* 4 */
		FF (a, b, c, d, 0, S11, 0xf57c0faf); /* 5 */
		FF (d, a, b, c, 0, S12, 0x4787c62a); /* 6 */
		FF (c, d, a, b, 0, S13, 0xa8304613); /* 7 */
		FF (b, c, d, a, 0, S14, 0xfd469501); /* 8 */
		FF (a, b, c, d, 0, S11, 0x698098d8); /* 9 */
		FF (d, a, b, c, 0, S12, 0x8b44f7af); /* 10 */
		FF (c, d, a, b, 0, S13, 0xffff5bb1); /* 11 */
		FF (b, c, d, a, 0, S14, 0x895cd7be); /* 12 */
		FF (a, b, c, d, 0, S11, 0x6b901122); /* 13 */
		FF (d, a, b, c, 0, S12, 0xfd987193); /* 14 */
		FF (c, d, a, b, 32, S13, 0xa679438e); /* 15 */
		FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */

		 /* Round 2 */
		GG (a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
		GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
		GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
		GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
		GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
		GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
		GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
		GG (d, a, b, c, 32, S22, 0xc33707d6); /* 26 */
		GG (c, d, a, b, 0, S23, 0xf4d50d87); /* 27 */
		GG (b, c, d, a, 0, S24, 0x455a14ed); /* 28 */
		GG (a, b, c, d, 0, S21, 0xa9e3e905); /* 29 */
		GG (d, a, b, c, 0, S22, 0xfcefa3f8); /* 30 */
		GG (c, d, a, b, 0, S23, 0x676f02d9); /* 31 */
		GG (b, c, d, a, 0, S24, 0x8d2a4c8a); /* 32 */
		
		/* Round 3 */
		HH (a, b, c, d, 0, S31, 0xfffa3942); /* 33 */
		HH (d, a, b, c, 0, S32, 0x8771f681); /* 34 */
		HH (c, d, a, b, 0, S33, 0x6d9d6122); /* 35 */
		HH (b, c, d, a, 32, S34, 0xfde5380c); /* 36 */
		HH (a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
		HH (d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
		if (d == revHash[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
			if (c == revHash[2])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
				if (b == revHash[1])
				{
					clock_t endtime = clock();
					cout<<"Crack found !!!\t\t\""<<init[0]<<init[1]<<init[2]<<init[3]<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
					return;
				}
			}
		}
		if(init[0] == alpha[alpha.size() - 1])
		{
			if(init[1] == alpha[alpha.size() - 1])
			{	
				if(init[2] == alpha[alpha.size() - 1])
				{	
					if(init[3] == alpha[alpha.size() - 1])
					{		
						clock_t endtime = clock();
						cout<<"4 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
						done = true;
					}else{
						init[0] = alpha[0];
						init[1] = alpha[0];
						init[2] = alpha[0];
						init[3] = alpha[alpha.find(init[3]) + 1];
					}
				}else{
					init[0] = alpha[0];
					init[1] = alpha[0];
					init[2] = alpha[alpha.find(init[2]) + 1];
				}
			}else{
				init[0] = alpha[0];
				init[1] = alpha[alpha.find(init[1]) + 1];
			}
		}else{
			init[0] = alpha[alpha.find(init[0]) + 1];
		}
	}	
}

void inline	 md5string::reverse5	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
	unsigned int a, b, c, d, x[16];
	vector<uint4> revHash1 = startrev57_1(40,pA,pB,pC,pD);
	vector<uint4> revHash2;
	clock_t starttime = clock();
	bool increment = false;
	bool done1 = false;
	bool done2 = false;

	while (!done1)
	{
		if(increment)
		{
			if(init[4] == alpha[alpha.size() - 1])
			{
				clock_t endtime = clock();
				cout<<"5 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
				done1 = true;
				return;
			}else{
				init[4] = alpha[alpha.find(init[4]) + 1];
				increment = false;
			}
		}
		
		x[1] = uint4(init[4]) + 0x8000;

		revHash2 = startrev57_2(40,revHash1[0],revHash1[1],revHash1[2],revHash1[3], x[1]);
		done2 = false;

		while (!done2)
		{
			x[0] = uint4(init[0]) + uint4(init[1])*0x100 + uint4(init[2])*0x10000 + uint4(init[3])*0x1000000;

			a = 0x67452301;
			b = 0xefcdab89; 
			c = 0x98badcfe; 
			d = 0x10325476;
			
			 /* Round 1 */
			FF (a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
			FF (d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
			FF (c, d, a, b, 0, S13, 0x242070db); /* 3 */
			FF (b, c, d, a, 0, S14, 0xc1bdceee); /* 4 */
			FF (a, b, c, d, 0, S11, 0xf57c0faf); /* 5 */
			FF (d, a, b, c, 0, S12, 0x4787c62a); /* 6 */
			FF (c, d, a, b, 0, S13, 0xa8304613); /* 7 */
			FF (b, c, d, a, 0, S14, 0xfd469501); /* 8 */
			FF (a, b, c, d, 0, S11, 0x698098d8); /* 9 */
			FF (d, a, b, c, 0, S12, 0x8b44f7af); /* 10 */
			FF (c, d, a, b, 0, S13, 0xffff5bb1); /* 11 */
			FF (b, c, d, a, 0, S14, 0x895cd7be); /* 12 */
			FF (a, b, c, d, 0, S11, 0x6b901122); /* 13 */
			FF (d, a, b, c, 0, S12, 0xfd987193); /* 14 */
			FF (c, d, a, b, 40, S13, 0xa679438e); /* 15 */
			FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */

			 /* Round 2 */
			GG (a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
			GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
			GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
			GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
			GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
			GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
			GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
			GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
			GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
			GG (d, a, b, c, 40, S22, 0xc33707d6); /* 26 */
			GG (c, d, a, b, 0, S23, 0xf4d50d87); /* 27 */
			GG (b, c, d, a, 0, S24, 0x455a14ed); /* 28 */
			GG (a, b, c, d, 0, S21, 0xa9e3e905); /* 29 */
			GG (d, a, b, c, 0, S22, 0xfcefa3f8); /* 30 */
			GG (c, d, a, b, 0, S23, 0x676f02d9); /* 31 */
			GG (b, c, d, a, 0, S24, 0x8d2a4c8a); /* 32 */
		
			/* Round 3 */
			HH (a, b, c, d, 0, S31, 0xfffa3942); /* 33 */
			HH (d, a, b, c, 0, S32, 0x8771f681); /* 34 */
			HH (c, d, a, b, 0, S33, 0x6d9d6122); /* 35 */
			HH (b, c, d, a, 40, S34, 0xfde5380c); /* 36 */
			HH (a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
			HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
			HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
			HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
			HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
			HH (d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
			HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
			HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
			HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
			HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
			if (d == revHash2[3])
			{
				HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
				if (c == revHash2[2])
				{
					HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
					if (b == revHash2[1])
					{
						clock_t endtime = clock();
						cout<<"Crack found !!!\t\t\""<<init[0]<<init[1]<<init[2]<<init[3]<<init[4]<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
						return;
					}
				}
			}
			if(init[0] == alpha[alpha.size() - 1])
			{	
				if(init[1] == alpha[alpha.size() - 1])
				{	
					if(init[2] == alpha[alpha.size() - 1])
					{	
						if(init[3] == alpha[alpha.size() - 1])
						{		
							done2 = true;
							increment = true;
							init[0] = alpha[0];
							init[1] = alpha[0];
							init[2] = alpha[0];
							init[3] = alpha[0];
						}else{
							init[0] = alpha[0];
							init[1] = alpha[0];
							init[2] = alpha[0];
							init[3] = alpha[alpha.find(init[3]) + 1];
						}
					}else{
						init[0] = alpha[0];
						init[1] = alpha[0];
						init[2] = alpha[alpha.find(init[2]) + 1];
					}
				}else{
					init[0] = alpha[0];
					init[1] = alpha[alpha.find(init[1]) + 1];
				}
			}else{
				init[0] = alpha[alpha.find(init[0]) + 1];
			}
		}
	}	
}

void inline	 md5string::reverse6	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
	unsigned int a, b, c, d, x[16];
	vector<uint4> revHash1 = startrev57_1(48,pA,pB,pC,pD);
	vector<uint4> revHash2;
	clock_t starttime = clock();
	bool increment = false;
	bool done1 = false;
	bool done2 = false;

	while (!done1)
	{
		if(increment)
		{
			if(init[4] == alpha[alpha.size() - 1])
			{
				if(init[5] == alpha[alpha.size() - 1])
				{
					clock_t endtime = clock();
					cout<<"6 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
					done1 = true;
					return;
				}else{
					init[4] = alpha[0];
					init[5] = alpha[alpha.find(init[5]) + 1];
					increment = false;
				}
			}else{
				init[4] = alpha[alpha.find(init[4]) + 1];
				increment = false;
			}
		}
		
		x[1] = uint4(init[4]) + uint4(init[5])*0x100 + 0x800000;

		revHash2 = startrev57_2(48,revHash1[0],revHash1[1],revHash1[2],revHash1[3], x[1]);
		done2 = false;

		while (!done2)
		{
			x[0] = uint4(init[0]) + uint4(init[1])*0x100 + uint4(init[2])*0x10000 + uint4(init[3])*0x1000000;

			a = 0x67452301;
			b = 0xefcdab89; 
			c = 0x98badcfe; 
			d = 0x10325476;
			
			 /* Round 1 */
			FF (a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
			FF (d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
			FF (c, d, a, b, 0, S13, 0x242070db); /* 3 */
			FF (b, c, d, a, 0, S14, 0xc1bdceee); /* 4 */
			FF (a, b, c, d, 0, S11, 0xf57c0faf); /* 5 */
			FF (d, a, b, c, 0, S12, 0x4787c62a); /* 6 */
			FF (c, d, a, b, 0, S13, 0xa8304613); /* 7 */
			FF (b, c, d, a, 0, S14, 0xfd469501); /* 8 */
			FF (a, b, c, d, 0, S11, 0x698098d8); /* 9 */
			FF (d, a, b, c, 0, S12, 0x8b44f7af); /* 10 */
			FF (c, d, a, b, 0, S13, 0xffff5bb1); /* 11 */
			FF (b, c, d, a, 0, S14, 0x895cd7be); /* 12 */
			FF (a, b, c, d, 0, S11, 0x6b901122); /* 13 */
			FF (d, a, b, c, 0, S12, 0xfd987193); /* 14 */
			FF (c, d, a, b, 48, S13, 0xa679438e); /* 15 */
			FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */

			 /* Round 2 */
			GG (a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
			GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
			GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
			GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
			GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
			GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
			GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
			GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
			GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
			GG (d, a, b, c, 48, S22, 0xc33707d6); /* 26 */
			GG (c, d, a, b, 0, S23, 0xf4d50d87); /* 27 */
			GG (b, c, d, a, 0, S24, 0x455a14ed); /* 28 */
			GG (a, b, c, d, 0, S21, 0xa9e3e905); /* 29 */
			GG (d, a, b, c, 0, S22, 0xfcefa3f8); /* 30 */
			GG (c, d, a, b, 0, S23, 0x676f02d9); /* 31 */
			GG (b, c, d, a, 0, S24, 0x8d2a4c8a); /* 32 */
		
			/* Round 3 */
			HH (a, b, c, d, 0, S31, 0xfffa3942); /* 33 */
			HH (d, a, b, c, 0, S32, 0x8771f681); /* 34 */
			HH (c, d, a, b, 0, S33, 0x6d9d6122); /* 35 */
			HH (b, c, d, a, 48, S34, 0xfde5380c); /* 36 */
			HH (a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
			HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
			HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
			HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
			HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
			HH (d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
			HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
			HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
			HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
			HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
			if (d == revHash2[3])
			{
				HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
				if (c == revHash2[2])
				{
					HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
					if (b == revHash2[1])
					{
						clock_t endtime = clock();
						cout<<"Crack found !!!\t\t\""<<init[0]<<init[1]<<init[2]<<init[3]<<init[4]<<init[5]<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
						return;
					}
				}
			}
			if(init[0] == alpha[alpha.size() - 1])
			{	
				if(init[1] == alpha[alpha.size() - 1])
				{	
					if(init[2] == alpha[alpha.size() - 1])
					{	
						if(init[3] == alpha[alpha.size() - 1])
						{
							done2 = true;		
							increment = true;
							init[0] = alpha[0];
							init[1] = alpha[0];
							init[2] = alpha[0];
							init[3] = alpha[0];
						}else{
							init[0] = alpha[0];
							init[1] = alpha[0];
							init[2] = alpha[0];
							init[3] = alpha[alpha.find(init[3]) + 1];
						}
					}else{
						init[0] = alpha[0];
						init[1] = alpha[0];
						init[2] = alpha[alpha.find(init[2]) + 1];
					}
				}else{
					init[0] = alpha[0];
					init[1] = alpha[alpha.find(init[1]) + 1];
				}
			}else{
				init[0] = alpha[alpha.find(init[0]) + 1];
			}
		}
	}	
}

void inline	 md5string::reverse7	(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init)
{
unsigned int a, b, c, d, x[16];
	vector<uint4> revHash1 = startrev57_1(56,pA,pB,pC,pD);
	vector<uint4> revHash2;
	clock_t starttime = clock();
	bool increment = false;
	bool done1 = false;
	bool done2 = false;

	while (!done1)
	{
		if(increment)
		{
			if(init[4] == alpha[alpha.size() - 1])
			{
				if(init[5] == alpha[alpha.size() - 1])
				{
					if(init[6] == alpha[alpha.size() - 1])
					{
						clock_t endtime = clock();
						cout<<"7 char done in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
						done1 = true;
						return;
					}else{
						init[4] = alpha[0];
						init[5] = alpha[0];
						init[6] = alpha[alpha.find(init[6]) + 1];
						increment = false;
					}
				}else{
					init[4] = alpha[0];
					init[5] = alpha[alpha.find(init[5]) + 1];
					increment = false;
				}
			}else{
				init[4] = alpha[alpha.find(init[4]) + 1];
				increment = false;
			}
		}
		
		x[1] = uint4(init[4]) + uint4(init[5])*0x100 + uint4(init[6])*0x10000 + 0x80000000;

		revHash2 = startrev57_2(56,revHash1[0],revHash1[1],revHash1[2],revHash1[3], x[1]);
		done2 = false;

		while (!done2)
		{
			x[0] = uint4(init[0]) + uint4(init[1])*0x100 + uint4(init[2])*0x10000 + uint4(init[3])*0x1000000;

			a = 0x67452301;
			b = 0xefcdab89; 
			c = 0x98badcfe; 
			d = 0x10325476;
			
			 /* Round 1 */
			FF (a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
			FF (d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
			FF (c, d, a, b, 0, S13, 0x242070db); /* 3 */
			FF (b, c, d, a, 0, S14, 0xc1bdceee); /* 4 */
			FF (a, b, c, d, 0, S11, 0xf57c0faf); /* 5 */
			FF (d, a, b, c, 0, S12, 0x4787c62a); /* 6 */
			FF (c, d, a, b, 0, S13, 0xa8304613); /* 7 */
			FF (b, c, d, a, 0, S14, 0xfd469501); /* 8 */
			FF (a, b, c, d, 0, S11, 0x698098d8); /* 9 */
			FF (d, a, b, c, 0, S12, 0x8b44f7af); /* 10 */
			FF (c, d, a, b, 0, S13, 0xffff5bb1); /* 11 */
			FF (b, c, d, a, 0, S14, 0x895cd7be); /* 12 */
			FF (a, b, c, d, 0, S11, 0x6b901122); /* 13 */
			FF (d, a, b, c, 0, S12, 0xfd987193); /* 14 */
			FF (c, d, a, b, 56, S13, 0xa679438e); /* 15 */
			FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */

			 /* Round 2 */
			GG (a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
			GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
			GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
			GG (b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
			GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
			GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
			GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
			GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
			GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
			GG (d, a, b, c, 56, S22, 0xc33707d6); /* 26 */
			GG (c, d, a, b, 0, S23, 0xf4d50d87); /* 27 */
			GG (b, c, d, a, 0, S24, 0x455a14ed); /* 28 */
			GG (a, b, c, d, 0, S21, 0xa9e3e905); /* 29 */
			GG (d, a, b, c, 0, S22, 0xfcefa3f8); /* 30 */
			GG (c, d, a, b, 0, S23, 0x676f02d9); /* 31 */
			GG (b, c, d, a, 0, S24, 0x8d2a4c8a); /* 32 */
		
			/* Round 3 */
			HH (a, b, c, d, 0, S31, 0xfffa3942); /* 33 */
			HH (d, a, b, c, 0, S32, 0x8771f681); /* 34 */
			HH (c, d, a, b, 0, S33, 0x6d9d6122); /* 35 */
			HH (b, c, d, a, 56, S34, 0xfde5380c); /* 36 */
			HH (a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
			HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
			HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
			HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
			HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
			HH (d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
			HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
			HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
			HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
			HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
			if (d == revHash2[3])
			{
				HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
				if (c == revHash2[2])
				{
					HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
					if (b == revHash2[1])
					{
						clock_t endtime = clock();
						cout<<"Crack found !!!\t\t\""<<init[0]<<init[1]<<init[2]<<init[3]<<init[4]<<init[5]<<init[6]<<"\" in " << ((double)( endtime - starttime )) / CLOCKS_PER_SEC << " seconds.\n";
						return;
					}
				}
			}
			if(init[0] == alpha[alpha.size() - 1])
			{	
				if(init[1] == alpha[alpha.size() - 1])
				{	
					if(init[2] == alpha[alpha.size() - 1])
					{	
						if(init[3] == alpha[alpha.size() - 1])
						{
							done2 = true;		
							increment = true;
							init[0] = alpha[0];
							init[1] = alpha[0];
							init[2] = alpha[0];
							init[3] = alpha[0];
						}else{
							init[0] = alpha[0];
							init[1] = alpha[0];
							init[2] = alpha[0];
							init[3] = alpha[alpha.find(init[3]) + 1];
						}
					}else{
						init[0] = alpha[0];
						init[1] = alpha[0];
						init[2] = alpha[alpha.find(init[2]) + 1];
					}
				}else{
					init[0] = alpha[0];
					init[1] = alpha[alpha.find(init[1]) + 1];
				}
			}else{
				init[0] = alpha[alpha.find(init[0]) + 1];
			}
		}
	}		
}

inline vector<uint4>	md5string::startrev13	(int size, uint4 pA, uint4 pB, uint4 pC, uint4 pD)
{
	uint4 apA[4];
	uint4 apB[4];
	uint4 apC[4];
	uint4 apD[4];
	
	apB[0] = rotate_left(pB-pC, 0-S44) - 0xeb86d391 - I(pC,pD,pA);										/* 64 */
	apC[0] = rotate_left(pC-pD, 0-S43) - 0x2ad7d2bb - I(pD,pA,apB[0]);									/* 63 */
	apD[0] = rotate_left(pD-pA, 0-S42) - 0xbd3af235 - I(pA,apB[0],apC[0]);								/* 62 */
	apA[0] = rotate_left(pA-apB[0], 0-S41) - 0xf7537e82 - I(apB[0],apC[0],apD[0]);						/* 61 */

	apB[1] = rotate_left(apB[0]-apC[0], 0-S44) - 0x4e0811a1 - I(apC[0],apD[0],apA[0]);					/* 60 */
	apC[1] = rotate_left(apC[0]-apD[0], 0-S43) - 0xa3014314 - I(apD[0],apA[0],apB[1]);					/* 59 */
	apD[1] = rotate_left(apD[0]-apA[0], 0-S42) - 0xfe2ce6e0 - I(apA[0],apB[1],apC[1]);					/* 58 */
	apA[1] = rotate_left(apA[0]-apB[1], 0-S41) - 0x6fa87e4f - I(apB[1],apC[1],apD[1]);					/* 57 */
	
	apB[2] = rotate_left(apB[1]-apC[1], 0-S44) - 0x85845dd1 - I(apC[1],apD[1],apA[1]);					/* 56 */
	apC[2] = rotate_left(apC[1]-apD[1], 0-S43) - 0xffeff47d - I(apD[1],apA[1],apB[2]);					/* 55 */
	apD[2] = rotate_left(apD[1]-apA[1], 0-S42) - 0x8f0ccc92 - I(apA[1],apB[2],apC[2]);					/* 54 */
	apA[2] = rotate_left(apA[1]-apB[2], 0-S41) - 0x655b59c3 - I(apB[2],apC[2],apD[2]);					/* 53 */


	apB[3] = rotate_left(apB[2]-apC[2], 0-S44) - 0xfc93a039 - I(apC[2],apD[2],apA[2]);					/* 52 */
	apC[3] = rotate_left(apC[2]-apD[2], 0-S43) - 0xab9423a7 - I(apD[2],apA[2],apB[3]) - size;			/* 51 */
	apD[3] = rotate_left(apD[2]-apA[2], 0-S42) - 0x432aff97 - I(apA[2],apB[3],apC[3]);					/* 50 */
	
	vector<uint4> revHash;
	revHash.push_back(apA[3]);
	revHash.push_back(apB[3]);
	revHash.push_back(apC[3]);
	revHash.push_back(apD[3]);
	return revHash;
}

inline vector<uint4>	md5string::startrev4	(int size, uint4 pA, uint4 pB, uint4 pC, uint4 pD)
{
	uint4 apA[4];
	uint4 apB[4];
	uint4 apC[4];
	uint4 apD[4];
	
	apB[0] = rotate_left(pB-pC, 0-S44) - 0xeb86d391 - I(pC,pD,pA);								/* 64 */
	apC[0] = rotate_left(pC-pD, 0-S43) - 0x2ad7d2bb - I(pD,pA,apB[0]);							/* 63 */
	apD[0] = rotate_left(pD-pA, 0-S42) - 0xbd3af235 - I(pA,apB[0],apC[0]);						/* 62 */
	apA[0] = rotate_left(pA-apB[0], 0-S41) - 0xf7537e82 - I(apB[0],apC[0],apD[0]);				/* 61 */

	apB[1] = rotate_left(apB[0]-apC[0], 0-S44) - 0x4e0811a1 - I(apC[0],apD[0],apA[0]);			/* 60 */
	apC[1] = rotate_left(apC[0]-apD[0], 0-S43) - 0xa3014314 - I(apD[0],apA[0],apB[1]);			/* 59 */
	apD[1] = rotate_left(apD[0]-apA[0], 0-S42) - 0xfe2ce6e0 - I(apA[0],apB[1],apC[1]);			/* 58 */
	apA[1] = rotate_left(apA[0]-apB[1], 0-S41) - 0x6fa87e4f - I(apB[1],apC[1],apD[1]);			/* 57 */
	
	apB[2] = rotate_left(apB[1]-apC[1], 0-S44) - 0x85845dd1 - I(apC[1],apD[1],apA[1]) - 0x80;	/* 56 */
	apC[2] = rotate_left(apC[1]-apD[1], 0-S43) - 0xffeff47d - I(apD[1],apA[1],apB[2]);			/* 55 */
	apD[2] = rotate_left(apD[1]-apA[1], 0-S42) - 0x8f0ccc92 - I(apA[1],apB[2],apC[2]);			/* 54 */
	apA[2] = rotate_left(apA[1]-apB[2], 0-S41) - 0x655b59c3 - I(apB[2],apC[2],apD[2]);			/* 53 */


	apB[3] = rotate_left(apB[2]-apC[2], 0-S44) - 0xfc93a039 - I(apC[2],apD[2],apA[2]);			/* 52 */
	apC[3] = rotate_left(apC[2]-apD[2], 0-S43) - 0xab9423a7 - I(apD[2],apA[2],apB[3]) - size;	/* 51 */
	apD[3] = rotate_left(apD[2]-apA[2], 0-S42) - 0x432aff97 - I(apA[2],apB[3],apC[3]);			/* 50 */
	
	vector<uint4> revHash;
	revHash.push_back(apA[3]);
	revHash.push_back(apB[3]);
	revHash.push_back(apC[3]);
	revHash.push_back(apD[3]);
	return revHash;
}

inline vector<uint4>   	md5string::startrev57_1	(int size, uint4 pA, uint4 pB, uint4 pC, uint4 pD)
{
	uint4 apA[4];
	uint4 apB[4];
	uint4 apC[4];
	uint4 apD[4];
	
	apB[0] = rotate_left(pB-pC, 0-S44) - 0xeb86d391 - I(pC,pD,pA);										/* 64 */
	apC[0] = rotate_left(pC-pD, 0-S43) - 0x2ad7d2bb - I(pD,pA,apB[0]);									/* 63 */
	apD[0] = rotate_left(pD-pA, 0-S42) - 0xbd3af235 - I(pA,apB[0],apC[0]);								/* 62 */
	apA[0] = rotate_left(pA-apB[0], 0-S41) - 0xf7537e82 - I(apB[0],apC[0],apD[0]);						/* 61 */

	apB[1] = rotate_left(apB[0]-apC[0], 0-S44) - 0x4e0811a1 - I(apC[0],apD[0],apA[0]);					/* 60 */
	apC[1] = rotate_left(apC[0]-apD[0], 0-S43) - 0xa3014314 - I(apD[0],apA[0],apB[1]);					/* 59 */
	apD[1] = rotate_left(apD[0]-apA[0], 0-S42) - 0xfe2ce6e0 - I(apA[0],apB[1],apC[1]);					/* 58 */
	apA[1] = rotate_left(apA[0]-apB[1], 0-S41) - 0x6fa87e4f - I(apB[1],apC[1],apD[1]);					/* 57 */
	
	vector<uint4> revHash;
	revHash.push_back(apA[1]);
	revHash.push_back(apB[1]);
	revHash.push_back(apC[1]);
	revHash.push_back(apD[1]);
	return revHash;
}

inline vector<uint4>   	md5string::startrev57_2	(int size, uint4 apA, uint4 apB, uint4 apC, uint4 apD, uint4 c2)
{
	uint4 bpA[4];
	uint4 bpB[4];
	uint4 bpC[4];
	uint4 bpD[4];
	
	bpB[2] = rotate_left(apB-apC, 0-S44) - 0x85845dd1 - I(apC,apD,apA) - c2;		/* 56 */
	bpC[2] = rotate_left(apC-apD, 0-S43) - 0xffeff47d - I(apD,apA,bpB[2]);		/* 55 */
	bpD[2] = rotate_left(apD-apA, 0-S42) - 0x8f0ccc92 - I(apA,bpB[2],bpC[2]);		/* 54 */
	bpA[2] = rotate_left(apA-bpB[2], 0-S41) - 0x655b59c3 - I(bpB[2],bpC[2],bpD[2]);		/* 53 */


	bpB[3] = rotate_left(bpB[2]-bpC[2], 0-S44) - 0xfc93a039 - I(bpC[2],bpD[2],bpA[2]);		/* 52 */
	bpC[3] = rotate_left(bpC[2]-bpD[2], 0-S43) - 0xab9423a7 - I(bpD[2],bpA[2],bpB[3]) - size;	/* 51 */
	bpD[3] = rotate_left(bpD[2]-bpA[2], 0-S42) - 0x432aff97 - I(bpA[2],bpB[3],bpC[3]);		/* 50 */
	
	vector<uint4> revHash;
	revHash.push_back(bpA[3]);
	revHash.push_back(bpB[3]);
	revHash.push_back(bpC[3]);
	revHash.push_back(bpD[3]);
	return revHash;
}
