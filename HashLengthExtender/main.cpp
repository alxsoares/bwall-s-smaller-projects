
#include <openssl/sha.h>
#include "SHA1.h"

using namespace std;

vector<unsigned char> StringToVector(unsigned char * str)
{
	vector<unsigned char> ret;
	for(int x = 0; x < strlen((char*)str); x++)
	{
		ret.push_back(str[x]);
	}
	return ret;
}

void DigestToRaw(string hash, unsigned char * raw)
{
	transform(hash.begin(), hash.end(), hash.begin(), ::tolower);
	string alpha("0123456789abcdef");
	for(unsigned int x = 0; x < (hash.length() / 2); x++)
	{
		raw[x] = (unsigned char)((alpha.find(hash.at((x * 2))) << 4));
		raw[x] |= (unsigned char)(alpha.find(hash.at((x * 2) + 1)));
	}
}

int main(int argc, char ** argv)
{
	cout << "Input Signature: ";
	string sig;
	cin >> sig;
	cout << "Input Data: ";
	string data;
	cin >> data;
	int keylength;
	cout << "Input Key Length: ";
	cin >> keylength;
	string datatoadd;
	cout << "Input Data to Add: ";
	cin >> datatoadd;

	vector<unsigned char> vmessage = StringToVector((unsigned char*)data.c_str());
	vector<unsigned char> vtoadd = StringToVector((unsigned char*)datatoadd.c_str());

	SHA1ex sex;
	unsigned char firstSig[20];
	DigestToRaw(sig, firstSig);
	unsigned char * secondSig;
	vector<unsigned char> secondMessage = sex.GenerateStretchedData(vmessage, keylength, firstSig, vtoadd, &secondSig);
	for(int x = 0; x < 20; x++)
	{
		printf("%02x", secondSig[x]);
	}
	cout << endl;
	for(int x = 0; x < secondMessage.size(); x++)
	{
		unsigned char c = secondMessage.at(x);
		if(c >= 32 && c <= 126)
		{
			cout << c;
		}
		else
		{
			printf("\\x%02x", c);
		}
	}
}
