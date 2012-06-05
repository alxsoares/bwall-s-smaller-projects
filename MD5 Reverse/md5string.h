#ifndef MD5STRING
#define MD5STRING

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <iostream>
#include <string>
#include <vector>

using namespace std;

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

typedef unsigned       int uint4;
typedef unsigned short int uint2;
typedef unsigned      char uint1;

class md5string
{
public:
	md5string(string str, string alpha, string init, uint4 start, uint4 end);
private:
	void				 			MD5decode		(uint4 *output, uint1 *input, uint4 len);
	static inline uint4  			rotate_left		(uint4 x, uint4 n);
	static inline uint4  			F				(uint4 x, uint4 y, uint4 z);
	static inline uint4  			G				(uint4 x, uint4 y, uint4 z);
	static inline uint4  			H				(uint4 x, uint4 y, uint4 z);
	static inline uint4  			I				(uint4 x, uint4 y, uint4 z);
	static inline void   			FF				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
	static inline void   			GG				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
	static inline void   			HH				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
	static inline void   			II				(uint4& a, uint4 b, uint4 c, uint4 d, uint4 x, uint4 s, uint4 ac);
	void inline	 					reverse1		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	void inline	 					reverse2		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	void inline	 					reverse3		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	void inline	 					reverse4		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	void inline	 					reverse5		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	void inline	 					reverse6		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	void inline	 					reverse7		(uint4 pA, uint4 pB, uint4 pC, uint4 pD, string alpha, string init);
	static inline vector<uint4>   	startrev13		(int size, uint4 pA, uint4 pB, uint4 pC, uint4 pD);
	static inline vector<uint4>   	startrev4		(int size, uint4 pA, uint4 pB, uint4 pC, uint4 pD);
	static inline vector<uint4>   	startrev57_1	(int size, uint4 pA, uint4 pB, uint4 pC, uint4 pD);
	static inline vector<uint4>   	startrev57_2	(int size, uint4 apA, uint4 apB, uint4 apC, uint4 apD, uint4 c2);
};

#endif
