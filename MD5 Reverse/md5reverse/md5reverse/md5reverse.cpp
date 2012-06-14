#include <cctype>
#include <algorithm>
#include <string>
#include "md5string.h"

using namespace std;

int main (int argc, char *argv[])
{
	string hash("433d991c709a37eaf7e6d12f0d17b55d");
	transform(hash.begin(), hash.end(), hash.begin(), (int(*)(int)) tolower);
	string alpha;
	for(int x = 0; x < 255; x++)
	{
		alpha.append(1, (char)x);
	}
	md5string cracker(hash, alpha, "", 0, 8);
	return (0);
}