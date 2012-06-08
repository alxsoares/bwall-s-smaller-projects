#include <cctype>
#include <algorithm>
#include <string>
#include "md5string.h"

using namespace std;

int main (int argc, char *argv[])
{
	string hash("e1b849f9631ffc1829b2e31402373e3c");
	transform(hash.begin(), hash.end(), hash.begin(), (int(*)(int)) tolower);
	string alpha("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
	md5string cracker(hash, alpha, "", 0, 8);
	return (0);
}