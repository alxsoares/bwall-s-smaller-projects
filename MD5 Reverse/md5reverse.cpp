#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <iostream>
#include <windows.h>

using namespace std;

// Length of test block, number of test blocks.

#define TEST_BLOCK_LEN 1000
#define TEST_BLOCK_COUNT 1000
#define S11 7
#define S12 12
#define S13 17
#define S14 22
#define S21 5
#define S22 9
#define S23 14
#define S24 20
#define S31 4
#define S32 11
#define S33 16
#define S34 23
#define S41 6
#define S42 10
#define S43 15
#define S44 21


typedef unsigned       int uint4; // assumes integer is 4 words long
typedef unsigned short int uint2; // assumes short integer is 2 words long
typedef unsigned      char uint1; // assumes char is 1 word long

static void			 MD5_string		(unsigned char *string);
void				 MD5decode		(unsigned int *output, unsigned char *input, unsigned int len);
static char inline	 reverse1		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static char inline	 reverse2		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static char inline	 reverse3		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static char inline	 reverse4		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static char inline	 reverse5		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static char inline	 reverse6		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static char inline	 reverse7		(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static inline uint4  rotate_left	(uint4 x, uint4 n);
static inline uint4  F				(uint4 x, uint4 y, uint4 z);
static inline uint4  G				(uint4 x, uint4 y, uint4 z);
static inline uint4  H				(uint4 x, uint4 y, uint4 z);
static inline uint4  I				(uint4 x, uint4 y, uint4 z);
static inline void   FF				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
static inline void   GG				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
static inline void   HH				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
static inline void   II				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
static inline void   startrev		(int size, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static inline void   startrev2		(int size, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static inline void   startrev51		(int size, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD);
static inline void   startrev52		(unsigned int c2, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD, int size);

unsigned int	dstate[4];
char			tempstring[16];
uint4			apA[16];
uint4			apB[16];
uint4			apC[16];
uint4			apD[16];
bool			done;
unsigned int	charset1[16][256];
unsigned int	hex1[256];
DWORD			tstart, tend, tnow;
time_t			starttime, now, endtime;
int				tablelen;
int				tablelens[16];
int				table;


// Main driver.

int main (int argc, char *argv[])
{
	hex1[48] = 0;
	hex1[49] = 1;
	hex1[50] = 2;
	hex1[51] = 3;
	hex1[52] = 4;
	hex1[53] = 5;
	hex1[54] = 6;
	hex1[55] = 7;
	hex1[56] = 8;
	hex1[57] = 9;
	hex1[65] = 10;
	hex1[66] = 11;
	hex1[67] = 12;
	hex1[68] = 13;
	hex1[69] = 14;
	hex1[70] = 15;
	hex1[71] = 16;
	hex1[97] = 10;
	hex1[98] = 11;
	hex1[99] = 12;
	hex1[100] = 13;
	hex1[101] = 14;
	hex1[102] = 15;
	hex1[103] = 16;
	
	table = int(argv[2][0]) - 48;
	MD5_string((unsigned char *) argv[1]);
	return (0);
}

void MD5_string (unsigned char *string)
{
	unsigned char blahblahtemp[16];
	unsigned char ft[33];
	unsigned int dA, dB, dC, dD;

	memcpy(ft, string, 33);
	blahblahtemp[0] = unsigned char(16*(hex1[int(ft[0])])+hex1[int(ft[1])]);
	blahblahtemp[1] = unsigned char(16*(hex1[int(ft[2])])+hex1[int(ft[3])]);
	blahblahtemp[2] = unsigned char(16*(hex1[int(ft[4])])+hex1[int(ft[5])]);
	blahblahtemp[3] = unsigned char(16*(hex1[int(ft[6])])+hex1[int(ft[7])]);
	blahblahtemp[4] = unsigned char(16*(hex1[int(ft[8])])+hex1[int(ft[9])]);
	blahblahtemp[5] = unsigned char(16*(hex1[int(ft[10])])+hex1[int(ft[11])]);
	blahblahtemp[6] = unsigned char(16*(hex1[int(ft[12])])+hex1[int(ft[13])]);
	blahblahtemp[7] = unsigned char(16*(hex1[int(ft[14])])+hex1[int(ft[15])]);
	blahblahtemp[8] = unsigned char(16*(hex1[int(ft[16])])+hex1[int(ft[17])]);
	blahblahtemp[9] = unsigned char(16*(hex1[int(ft[18])])+hex1[int(ft[19])]);
	blahblahtemp[10] = unsigned char(16*(hex1[int(ft[20])])+hex1[int(ft[21])]);
	blahblahtemp[11] = unsigned char(16*(hex1[int(ft[22])])+hex1[int(ft[23])]);
	blahblahtemp[12] = unsigned char(16*(hex1[int(ft[24])])+hex1[int(ft[25])]);
	blahblahtemp[13] = unsigned char(16*(hex1[int(ft[26])])+hex1[int(ft[27])]);
	blahblahtemp[14] = unsigned char(16*(hex1[int(ft[28])])+hex1[int(ft[29])]);
	blahblahtemp[15] = unsigned char(16*(hex1[int(ft[30])])+hex1[int(ft[31])]);
	MD5decode(&dstate[0],&blahblahtemp[0],16);


	//States have been reversed

	dA = dstate[0] - 0x67452301;
	dB = dstate[1] - 0xefcdab89;
	dC = dstate[2] - 0x98badcfe;
	dD = dstate[3] - 0x10325476;

	//The final a,b,c,d have been found
	//The painfully difficult reversing has begun...

	int j = 0;
	for(int i = 48; i <= 57; i++)
	{
		charset1[0][j] = i;
		charset1[1][j] = i;
		charset1[4][j] = i;
		charset1[5][j] = i;
		j++;
	}
	for(int i = 65; i <= 90; i++)
	{
		charset1[0][j] = i;
		charset1[2][j-10] = i;
		charset1[4][j] = i;
		j++;
	}
	for(int i = 97; i <= 122; i++)
	{
		charset1[0][j] = i;
		charset1[3][j-10-26] = i;
		charset1[5][j - 26] = i;
		j++;
	}
	tablelens[0] = 62;//Upper + Lower Letters + Numbers
	tablelens[1] = 10;//Numbers
	tablelens[2] = 26;//Upper Case Letters
	tablelens[3] = 26;//Lower Case Letters
	tablelens[4] = 36;//Upper Case Letters + Numbers
	tablelens[5] = 36;//Lower Case Letters + Numbers

	tablelen = tablelens[table];

	tstart = GetTickCount();

	reverse1(dA,dB,dC,dD);
	if(done != true)
		reverse2(dA,dB,dC,dD);
	if(done != true)
		reverse3(dA,dB,dC,dD);
	if(done != true)
		reverse4(dA,dB,dC,dD);
	if(done != true)
		reverse5(dA,dB,dC,dD);
	if(done != true)
		reverse6(dA,dB,dC,dD);
	if(done != true)
		reverse7(dA,dB,dC,dD);
cout<<"\nDone.\n";
}

void MD5decode  (unsigned int *output, unsigned char *input, unsigned int len){

  unsigned int i, j;

  for (i = 0, j = 0; j < len; i++, j += 4)
    output[i] = ((unsigned int)input[j]) | (((unsigned int)input[j+1]) << 8) |
      (((unsigned int)input[j+2]) << 16) | (((unsigned int)input[j+3]) << 24);
}

char reverse1	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart = 0;
	unsigned int Cend = tablelen;
	unsigned int a, b, c, d, x[16];
	uint4 pX[16];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev(8,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	x[14]= 8;
	x[0] = Cstart;
	
	while (x[0] <= Cend)
	{
		  /* Round 1 */
		a = 0x67452301;
		b = 0xefcdab89;
		c = 0x98badcfe;
		d = 0x10325476;
		
		Cstart = charset1[table][x[0]] + 0x8000;

		FF (a, b, c, d, Cstart, S11, 0xd76aa478); /* 1 */
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
		FF (c, d, a, b, 8, S13, 0xa679438e); /* 15 */
		FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */
		
		 /* Round 2 */
		GG (a, b, c, d, 0, S21, 0xf61e2562); /* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
		GG (b, c, d, a, Cstart, S24, 0xe9b6c7aa); /* 20 */
		GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
		GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
		GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
		GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
		GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
		GG (d, a, b, c, 8, S22, 0xc33707d6); /* 26 */
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
		HH (b, c, d, a, 8, S34, 0xfde5380c); /* 36 */
		HH (a, b, c, d, 0, S31, 0xa4beea44); /* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
		HH (d, a, b, c, Cstart, S32, 0xeaa127fa); /* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
		if (d == apD[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
			if (c == apC[3])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
				if (b == apB[3])
				{
					tend = GetTickCount();
					cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
					cout<<"Crack found !!!\t\t\""<<char(charset1[table][x[0]])<<"\""<<endl;
					done = true;
					return 0;
				}
			}
		}
		x[0]++;
	}
	tnow = GetTickCount();
	cout<<"1 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";
	return pX[0];
}
char reverse2	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart = 0;
	unsigned int Cend = tablelen * tablelen;
	unsigned int temp[2];
	uint4 pX[16];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev(16,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	unsigned int a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476, x[16];
	x[0] = Cstart;
	while (x[0] <= Cend)
	{
		  /* Round 1 */
		a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;
		
		temp[0] = x[0] / tablelen;
		temp[1] = x[0]-(temp[0]*tablelen);
		Cstart = 0x800000 + charset1[table][temp[0]]*0x100 + charset1[table][temp[1]];

		FF (a, b, c, d, Cstart, S11, 0xd76aa478); /* 1 */
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
		FF (c, d, a, b, 16, S13, 0xa679438e); /* 15 */
		FF (b, c, d, a, 0, S14, 0x49b40821); /* 16 */

		 /* Round 2 */
		GG (a, b, c, d, 0, S21, 0xf61e2562); /* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
		GG (b, c, d, a, Cstart, S24, 0xe9b6c7aa); /* 20 */
		GG (a, b, c, d, 0, S21, 0xd62f105d); /* 21 */
		GG (d, a, b, c, 0, S22,  0x2441453); /* 22 */
		GG (c, d, a, b, 0, S23, 0xd8a1e681); /* 23 */
		GG (b, c, d, a, 0, S24, 0xe7d3fbc8); /* 24 */
		GG (a, b, c, d, 0, S21, 0x21e1cde6); /* 25 */
		GG (d, a, b, c, 16, S22, 0xc33707d6); /* 26 */
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
		HH (b, c, d, a, 16, S34, 0xfde5380c); /* 36 */
		HH (a, b, c, d, 0, S31, 0xa4beea44); /* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
		HH (d, a, b, c, Cstart, S32, 0xeaa127fa); /* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
		if (d == apD[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
			if (c == apC[3])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
				if (b == apB[3])
				{
					tend = GetTickCount();
					cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
					cout<<"Crack found !!!\t\t\""<<char(charset1[table][temp[1]])<<char(charset1[table][temp[0]])<<"\""<<endl;
					done = true;
					return 0;
				}
			}
		}
		x[0]++;
	}
	tnow = GetTickCount();
	cout<<"2 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";
	return pX[0];
}


char reverse3	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart =	0;
	unsigned int Cend	=	tablelen * tablelen * tablelen;
	unsigned int temp[3];
	uint4 pX[16];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev(24,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	unsigned int a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476, x[16];
	x[0] = Cstart;
	while (x[0] <= Cend)
	{
		  /* Round 1 */
		a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;

		temp[0] = x[0] / (tablelen * tablelen);
		Cstart = x[0] - (temp[0] * tablelen * tablelen);
		temp[1] = Cstart / tablelen;
		Cstart = Cstart - (temp[1] * tablelen);
		temp[2] = Cstart;
		Cstart = 0x80000000 + charset1[table][temp[0]]*0x10000 + charset1[table][temp[1]]*0x100 + charset1[table][Cstart];

		FF (a, b, c, d, Cstart, S11, 0xd76aa478); /* 1 */
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
		GG (b, c, d, a, Cstart, S24, 0xe9b6c7aa); /* 20 */
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
		HH (d, a, b, c, Cstart, S32, 0xeaa127fa); /* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
		if (d == apD[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
			if (c == apC[3])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
				if (b == apB[3])
				{
					tend = GetTickCount();
					cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
					cout<<"Crack found !!!\t\t\""<<char(charset1[table][temp[2]])<<char(charset1[table][temp[1]])<<char(charset1[table][temp[0]])<<"\""<<endl;
					done = true;
					return 0;
				}
			}
		}
		x[0]++;
	}
	tnow = GetTickCount();
	cout<<"3 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";
	return pX[0];
}
char reverse4	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart =	0;
	unsigned int Cend	=	tablelen * tablelen * tablelen * tablelen;
	uint4 pX[1];
	unsigned int temp[4];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev2(32,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	unsigned int a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476, x[16];
	x[0] = Cstart;
	while (x[0] < Cend)
	{
		  /* Round 1 */
		a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;

		temp[0] = x[0] / (tablelen * tablelen * tablelen);
		Cstart = x[0] - (temp[0] * tablelen * tablelen * tablelen);
		temp[1] = Cstart / (tablelen * tablelen);
		Cstart = Cstart - (temp[1] * tablelen * tablelen);
		temp[2] = Cstart / (tablelen);
		Cstart = Cstart - (temp[2] * tablelen);
		temp[3] = Cstart;
		Cstart = charset1[table][temp[0]]*0x1000000 + charset1[table][temp[1]]*0x10000 + charset1[table][temp[2]]*0x100 + charset1[table][Cstart];

		FF (a, b, c, d, Cstart, S11, 0xd76aa478); /* 1 */
		FF (d, a, b, c, 0x80, S12, 0xe8c7b756); /* 2 */
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
		GG (a, b, c, d, 0x80, S21, 0xf61e2562); /* 17 */
		GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
		GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
		GG (b, c, d, a, Cstart, S24, 0xe9b6c7aa); /* 20 */
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
		HH (a, b, c, d, 0x80, S31, 0xa4beea44); /* 37 */
		HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
		HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
		HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
		HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
		HH (d, a, b, c, Cstart, S32, 0xeaa127fa); /* 42 */
		HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
		HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
		HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
		HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
		if (d == apD[3])
		{
			HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
			if (c == apC[3])
			{
				HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
				if (b == apB[3])
				{
					tend = GetTickCount();
					cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
					cout<<"Crack found !!!\t\t\""<<char(charset1[table][temp[3]])<<char(charset1[table][temp[2]])<<char(charset1[table][temp[1]])<<char(charset1[table][temp[0]])<<"\"\t\tAnd total # of keys/sec = "<<float(x[0] + (tablelen*tablelen*tablelen) + (tablelen*tablelen) + (tablelen))/(float(tend - tstart)/float(1000))<<endl;
					done = true;
					return 0;
				}
			}
		}
		x[0]++;
	}
	tnow = GetTickCount();
	cout<<"4 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";
	return pX[0];
}
char reverse5	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart1 =	0;
	unsigned int Cend1	 =	tablelen * tablelen * tablelen * tablelen;
	unsigned int Cstart2 =  0;
	unsigned int Cend2	 =  tablelen;
	unsigned int temp[16];
	uint4 pX[1];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev51(40,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	unsigned int a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476, x[16];
	x[1] = Cstart2;
	while (x[1] <= Cend2)
	{
		Cstart2 = charset1[table][x[1]] + 0x8000;

		startrev52(Cstart2,pA,pB,pC,pD,40);

		x[0] = Cstart1;
		while (x[0] < Cend1)
		{
				/* Round 1 */
			temp[0] = x[0] / (tablelen * tablelen * tablelen);
			Cstart1 = x[0] - (temp[0] * tablelen * tablelen *tablelen);
			temp[1] = Cstart1 / (tablelen * tablelen);
			Cstart1 = Cstart1 - (temp[1] * tablelen * tablelen);
			temp[2] = Cstart1 / (tablelen);
			Cstart1 = Cstart1 - (temp[2] * tablelen);
			temp[3] = Cstart1;
			Cstart1 = charset1[table][temp[0]]*0x1000000 + charset1[table][temp[1]]*0x10000 + charset1[table][temp[2]]*0x100 + charset1[table][Cstart1];



			a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;
			FF (a, b, c, d, Cstart1, S11, 0xd76aa478); /* 1 */
			FF (d, a, b, c, Cstart2, S12, 0xe8c7b756); /* 2 */
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
			GG (a, b, c, d, Cstart2, S21, 0xf61e2562); /* 17 */
			GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
			GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
			GG (b, c, d, a, Cstart1, S24, 0xe9b6c7aa); /* 20 */
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
			HH (a, b, c, d, Cstart2, S31, 0xa4beea44); /* 37 */
			HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
			HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
			HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
			HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
			HH (d, a, b, c, Cstart1, S32, 0xeaa127fa); /* 42 */
			HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
			HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
			HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
			HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
			if (d == apD[3])
			{
				HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
				if (c == apC[3])
				{
					HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
					if (b == apB[3])
					{
						tend = GetTickCount();
						cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
						cout<<"Crack found !!!\t\t\""<<char(charset1[table][temp[3]])<<char(charset1[table][temp[2]])<<char(charset1[table][temp[1]])<<char(charset1[table][temp[0]])<<char(charset1[table][x[1]])<<"\"\t\tAnd total # of keys/sec = "<<float(x[0] + (tablelen*tablelen*tablelen*tablelen) + (tablelen*tablelen*tablelen) + (tablelen*tablelen) + (tablelen) +(x[1] * tablelen*tablelen*tablelen*tablelen))/(float(tend - tstart)/float(1000))<<endl;
						done = true;
						return 0;
					}
				}
			}
			x[0]++;
		}
		x[0] = 0;
		Cstart1 = 0;
		x[1]++;
	}
	tnow = GetTickCount();
	cout<<"5 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";

	return pX[0];
}
char reverse6	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart1 =	0;
	unsigned int Cend1	 =	tablelen*tablelen*tablelen*tablelen;
	unsigned int Cstart2 =  0;
	unsigned int Cend2	 =  tablelen*tablelen;
	unsigned int temp[16];
	uint4 pX[1];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev51(48,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	unsigned int a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476, x[16];
	x[1] = Cstart2;
	while (x[1] <= Cend2)
	{
		temp[3] = x[1] / tablelen;
		Cstart2 = 0x800000 + charset1[table][temp[3]]*0x100 + charset1[table][x[1]-(temp[3] * tablelen)];

		startrev52(Cstart2,pA,pB,pC,pD,48);

		x[0] = Cstart1;
		while (x[0] < Cend1)
		{
				/* Round 1 */
			temp[0] = x[0] / (tablelen * tablelen * tablelen);
			Cstart1 = x[0] - (temp[0] * tablelen * tablelen *tablelen);
			temp[1] = Cstart1 / (tablelen * tablelen);
			Cstart1 = Cstart1 - (temp[1] * tablelen * tablelen);
			temp[2] = Cstart1 / (tablelen);
			Cstart1 = Cstart1 - (temp[2] * tablelen);
			temp[4] = Cstart1;
			Cstart1 = charset1[table][temp[0]]*0x1000000 + charset1[table][temp[1]]*0x10000 + charset1[table][temp[2]]*0x100 + charset1[table][temp[4]];

			

			a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;
			FF (a, b, c, d, Cstart1, S11, 0xd76aa478); /* 1 */
			FF (d, a, b, c, Cstart2, S12, 0xe8c7b756); /* 2 */
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
			GG (a, b, c, d, Cstart2, S21, 0xf61e2562); /* 17 */
			GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
			GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
			GG (b, c, d, a, Cstart1, S24, 0xe9b6c7aa); /* 20 */
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
			HH (a, b, c, d, Cstart2, S31, 0xa4beea44); /* 37 */
			HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
			HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
			HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
			HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
			HH (d, a, b, c, Cstart1, S32, 0xeaa127fa); /* 42 */
			HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
			HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
			HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
			HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
			if (d == apD[3])
			{
				HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
				if (c == apC[3])
				{
					HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
					if (b == apB[3])
					{
						tend = GetTickCount();
						cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
						cout<<"Crack found !!!\t\t\""<<char(charset1[table][temp[4]])<<char(charset1[table][temp[2]])<<char(charset1[table][temp[1]])<<char(charset1[table][temp[0]])<<char(charset1[table][x[1]-(temp[3] * tablelen)])<<char(charset1[table][temp[3]])<<"\"\t\tAnd total # of keys/sec = "<<float(x[0] + (x[1] * tablelen * tablelen * tablelen * tablelen) + (tablelen * tablelen * tablelen * tablelen * tablelen) + (tablelen*tablelen*tablelen*tablelen) + (tablelen*tablelen*tablelen) + (tablelen*tablelen) + tablelen)/(float(tend - tstart)/float(1000))<<endl;
						done = true;
						return 0;
					}
				}
			}
			x[0]++;
		}
		x[0] = 0;
		Cstart1 = 0;
		x[1]++;
	}
	tnow = GetTickCount();
	cout<<"6 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";

	return pX[0];
}
char reverse7	(unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	unsigned int Cstart1 =	0;
	unsigned int Cend1	 =	tablelen * tablelen * tablelen * tablelen;
	unsigned int Cstart2 =  0;
	unsigned int Cend2	 =  tablelen * tablelen * tablelen;
	unsigned int temp[16];
	uint4 pX[1];
	pX[0] = 0;//What needs to be found for 1-3 chars//

	startrev51(56,pA,pB,pC,pD);

	//All possible reversing done//
	//Time to crack it!!! >>-P//

	unsigned int a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476, x[16];
	x[1] = Cstart2;
	while (x[1] <= Cend2)
	{
		
		temp[4] = x[1] / (tablelen * tablelen);
		Cstart2 = x[1] - (temp[4] * tablelen * tablelen);
		temp[5] = Cstart2 / tablelen;
		Cstart2 = Cstart2 - (temp[5] * tablelen);
		temp[6] = Cstart2;
		Cstart2 = 0x80000000 + charset1[table][temp[4]]*0x10000 + charset1[table][temp[5]]*0x100 + charset1[table][Cstart2];
		
		startrev52(Cstart2,pA,pB,pC,pD,56);

		x[0] = Cstart1;
		while (x[0] < Cend1)
		{
				/* Round 1 */
			temp[0] = x[0] / (tablelen * tablelen * tablelen);
			Cstart1 = x[0] - (temp[0] * tablelen * tablelen * tablelen);
			temp[1] = Cstart1 / (tablelen * tablelen);
			Cstart1 = Cstart1 - (temp[1] * tablelen * tablelen);
			temp[2] = Cstart1 / (tablelen);
			Cstart1 = Cstart1 - (temp[2] * tablelen);
			temp[3] = Cstart1;
			Cstart1 = charset1[table][temp[0]]*0x1000000 + charset1[table][temp[1]]*0x10000 + charset1[table][temp[2]]*0x100 + charset1[table][temp[3]];

			a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;
			FF (a, b, c, d, Cstart1, S11, 0xd76aa478); /* 1 */
			FF (d, a, b, c, Cstart2, S12, 0xe8c7b756); /* 2 */
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
			GG (a, b, c, d, Cstart2, S21, 0xf61e2562); /* 17 */
			GG (d, a, b, c, 0, S22, 0xc040b340); /* 18 */
			GG (c, d, a, b, 0, S23, 0x265e5a51); /* 19 */
			GG (b, c, d, a, Cstart1, S24, 0xe9b6c7aa); /* 20 */
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
			HH (a, b, c, d, Cstart2, S31, 0xa4beea44); /* 37 */
			HH (d, a, b, c, 0, S32, 0x4bdecfa9); /* 38 */
			HH (c, d, a, b, 0, S33, 0xf6bb4b60); /* 39 */
			HH (b, c, d, a, 0, S34, 0xbebfbc70); /* 40 */
			HH (a, b, c, d, 0, S31, 0x289b7ec6); /* 41 */
			HH (d, a, b, c, Cstart1, S32, 0xeaa127fa); /* 42 */
			HH (c, d, a, b, 0, S33, 0xd4ef3085); /* 43 */
			HH (b, c, d, a, 0, S34,  0x4881d05); /* 44 */
			HH (a, b, c, d, 0, S31, 0xd9d4d039); /* 45 */
			HH (d, a, b, c, 0, S32, 0xe6db99e5); /* 46 */
			if (d == apD[3])
			{
				HH (c, d, a, b, 0, S33, 0x1fa27cf8); /* 47 */
				if (c == apC[3])
				{
					HH (b, c, d, a, 0, S34, 0xc4ac5665); /* 48 */
					if (b == apB[3])
					{
						tend = GetTickCount();
						cout<<"Time = "<<float(tend - tstart)/float(1000)<<endl;
						cout<<"Crack found !!!\t\t\""<<char(charset1[table][temp[3]])<<char(charset1[table][temp[2]])<<char(charset1[table][temp[1]])<<char(charset1[table][temp[0]])<<char(charset1[table][temp[6]])<<char(charset1[table][temp[5]])<<char(charset1[table][temp[4]])<<"\"\t\tAnd total # of keys/sec = "<<float(x[0] + (x[1] * tablelen * tablelen * tablelen * tablelen) + (tablelen * tablelen * tablelen * tablelen * tablelen) + (tablelen*tablelen*tablelen*tablelen) + (tablelen*tablelen*tablelen) + (tablelen*tablelen) + tablelen)/(float(tend - tstart)/float(1000))<<endl;
						done = true;
						return 0;
					}
				}
			}
			x[0]++;
		}
		x[0] = 0;
		Cstart1 = 0;
		x[1]++;
	}
	tnow = GetTickCount();
	cout<<"7 char done at "<<float(tnow - tstart)/float(1000)<<" second.\n";

	return pX[0];
}
inline unsigned int rotate_left  (uint4 x, uint4 n)
{
	return (x << n) | (x >> (32-n))  ;
}

inline unsigned int F			(uint4 x, uint4 y, uint4 z)
{
	return (x & y) | (~x & z);
}

inline unsigned int G            (uint4 x, uint4 y, uint4 z)
{
	return (x & z) | (y & ~z);
}

inline unsigned int H            (uint4 x, uint4 y, uint4 z)
{
	return x ^ y ^ z;
}

inline unsigned int I            (uint4 x, uint4 y, uint4 z)
{
	return y ^ (x | ~z);
}

inline void FF(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4  s, uint4 ac)
{
 a += F(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

inline void GG(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac)
{
 a += G(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

inline void HH(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac)
{
 a += H(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}

inline void II(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac)
{
 a += I(b, c, d) + x + ac;
 a = rotate_left (a, s) +b;
}


#pragma optimize("", off)
static inline void   startrev(int size, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	apB[0] = rotate_left(pB-pC, 0-S44) - 0xeb86d391 - I(pC,pD,pA);										/* 64 */
	apC[0] = rotate_left(pC-pD, 0-S43) - 0x2ad7d2bb - I(pD,pA,apB[0]);									/* 63 */
	apD[0] = rotate_left(pD-pA, 0-S42) - 0xbd3af235 - I(pA,apB[0],apC[0]);								/* 62 */
	apA[0] = rotate_left(pA-apB[0], 0-S41) - 0xf7537e82 - I(apB[0],apC[0],apD[0]);						/* 61 */

	apB[1] = rotate_left(apB[0]-apC[0], 0-S44) - 0x4e0811a1 - I(apC[0],apD[0],apA[0]);					/* 60 */
	apC[1] = rotate_left(apC[0]-apD[0], 0-S43) - 0xa3014314 - I(apD[0],apA[0],apB[1]);					/* 59 */
	apD[1] = rotate_left(apD[0]-apA[0], 0-S42) - 0xfe2ce6e0 - I(apA[0],apB[1],apC[1]);					/* 58 */
	apA[1] = rotate_left(apA[0]-apB[1], 0-S41) - 0x6fa87e4f - I(apB[1],apC[1],apD[1]);					/* 57 */
	
	apB[2] = rotate_left(apB[1]-apC[1], 0-S44) - 0x85845dd1 - I(apC[1],apD[1],apA[1]);		/* 56 */
	apC[2] = rotate_left(apC[1]-apD[1], 0-S43) - 0xffeff47d - I(apD[1],apA[1],apB[2]);		/* 55 */
	apD[2] = rotate_left(apD[1]-apA[1], 0-S42) - 0x8f0ccc92 - I(apA[1],apB[2],apC[2]);		/* 54 */
	apA[2] = rotate_left(apA[1]-apB[2], 0-S41) - 0x655b59c3 - I(apB[2],apC[2],apD[2]);		/* 53 */


	apB[3] = rotate_left(apB[2]-apC[2], 0-S44) - 0xfc93a039 - I(apC[2],apD[2],apA[2]);		/* 52 */
	apC[3] = rotate_left(apC[2]-apD[2], 0-S43) - 0xab9423a7 - I(apD[2],apA[2],apB[3]) - size;	/* 51 */
	apD[3] = rotate_left(apD[2]-apA[2], 0-S42) - 0x432aff97 - I(apA[2],apB[3],apC[3]);		/* 50 */

}

static inline void   startrev2(int size, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	apB[0] = rotate_left(pB-pC, 0-S44) - 0xeb86d391 - I(pC,pD,pA);										/* 64 */
	apC[0] = rotate_left(pC-pD, 0-S43) - 0x2ad7d2bb - I(pD,pA,apB[0]);									/* 63 */
	apD[0] = rotate_left(pD-pA, 0-S42) - 0xbd3af235 - I(pA,apB[0],apC[0]);								/* 62 */
	apA[0] = rotate_left(pA-apB[0], 0-S41) - 0xf7537e82 - I(apB[0],apC[0],apD[0]);						/* 61 */

	apB[1] = rotate_left(apB[0]-apC[0], 0-S44) - 0x4e0811a1 - I(apC[0],apD[0],apA[0]);					/* 60 */
	apC[1] = rotate_left(apC[0]-apD[0], 0-S43) - 0xa3014314 - I(apD[0],apA[0],apB[1]);					/* 59 */
	apD[1] = rotate_left(apD[0]-apA[0], 0-S42) - 0xfe2ce6e0 - I(apA[0],apB[1],apC[1]);					/* 58 */
	apA[1] = rotate_left(apA[0]-apB[1], 0-S41) - 0x6fa87e4f - I(apB[1],apC[1],apD[1]);					/* 57 */
	
	apB[2] = rotate_left(apB[1]-apC[1], 0-S44) - 0x85845dd1 - I(apC[1],apD[1],apA[1]) - 0x80;		/* 56 */
	apC[2] = rotate_left(apC[1]-apD[1], 0-S43) - 0xffeff47d - I(apD[1],apA[1],apB[2]);		/* 55 */
	apD[2] = rotate_left(apD[1]-apA[1], 0-S42) - 0x8f0ccc92 - I(apA[1],apB[2],apC[2]);		/* 54 */
	apA[2] = rotate_left(apA[1]-apB[2], 0-S41) - 0x655b59c3 - I(apB[2],apC[2],apD[2]);		/* 53 */


	apB[3] = rotate_left(apB[2]-apC[2], 0-S44) - 0xfc93a039 - I(apC[2],apD[2],apA[2]);		/* 52 */
	apC[3] = rotate_left(apC[2]-apD[2], 0-S43) - 0xab9423a7 - I(apD[2],apA[2],apB[3]) - size;	/* 51 */
	apD[3] = rotate_left(apD[2]-apA[2], 0-S42) - 0x432aff97 - I(apA[2],apB[3],apC[3]);		/* 50 */

}
static inline void   startrev51(int size, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD)
{
	apB[0] = rotate_left(pB-pC, 0-S44) - 0xeb86d391 - I(pC,pD,pA);										/* 64 */
	apC[0] = rotate_left(pC-pD, 0-S43) - 0x2ad7d2bb - I(pD,pA,apB[0]);									/* 63 */
	apD[0] = rotate_left(pD-pA, 0-S42) - 0xbd3af235 - I(pA,apB[0],apC[0]);								/* 62 */
	apA[0] = rotate_left(pA-apB[0], 0-S41) - 0xf7537e82 - I(apB[0],apC[0],apD[0]);						/* 61 */

	apB[1] = rotate_left(apB[0]-apC[0], 0-S44) - 0x4e0811a1 - I(apC[0],apD[0],apA[0]);					/* 60 */
	apC[1] = rotate_left(apC[0]-apD[0], 0-S43) - 0xa3014314 - I(apD[0],apA[0],apB[1]);					/* 59 */
	apD[1] = rotate_left(apD[0]-apA[0], 0-S42) - 0xfe2ce6e0 - I(apA[0],apB[1],apC[1]);					/* 58 */
	apA[1] = rotate_left(apA[0]-apB[1], 0-S41) - 0x6fa87e4f - I(apB[1],apC[1],apD[1]);					/* 57 */
}
static inline void   startrev52(unsigned int c2, unsigned int pA, unsigned int pB, unsigned int pC, unsigned int pD, int size)
{
	apB[2] = rotate_left(apB[1]-apC[1], 0-S44) - 0x85845dd1 - I(apC[1],apD[1],apA[1]) - c2;		/* 56 */
	apC[2] = rotate_left(apC[1]-apD[1], 0-S43) - 0xffeff47d - I(apD[1],apA[1],apB[2]);		/* 55 */
	apD[2] = rotate_left(apD[1]-apA[1], 0-S42) - 0x8f0ccc92 - I(apA[1],apB[2],apC[2]);		/* 54 */
	apA[2] = rotate_left(apA[1]-apB[2], 0-S41) - 0x655b59c3 - I(apB[2],apC[2],apD[2]);		/* 53 */


	apB[3] = rotate_left(apB[2]-apC[2], 0-S44) - 0xfc93a039 - I(apC[2],apD[2],apA[2]);		/* 52 */
	apC[3] = rotate_left(apC[2]-apD[2], 0-S43) - 0xab9423a7 - I(apD[2],apA[2],apB[3]) - size;	/* 51 */
	apD[3] = rotate_left(apD[2]-apA[2], 0-S42) - 0x432aff97 - I(apA[2],apB[3],apC[3]);		/* 50 */
}

#pragma optimize("", on)
