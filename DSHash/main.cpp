#include <openssl/dsa.h>
#include <openssl/ecdsa.h>
#include <openssl/engine.h>
#include <openssl/sha.h>
#include <openssl/objects.h>
#include <openssl/bn.h>
#include <iostream>
#include <sys/time.h>
#include <string.h>

using namespace std;

int attempts = 82 * 82 * 82;

void PrintHex(unsigned char * data, int length)
{
	for(int x = 0; x < length; x++)
	{
		printf("%X", data[x]);
	}
}

//Makes a DSA hash of the given bit size and benchmarks cracking speed
void Test(int bits)
{
	cout << "Generating " << bits << " bit DSA key pair" << endl;
	struct timeval startTime;
	gettimeofday(&startTime, NULL);
	DSA * dsa = DSA_generate_parameters(bits, NULL, 0, NULL, NULL, NULL, NULL);
	DSA_generate_key(dsa);

	struct timeval now;
	gettimeofday(&now, NULL);
	double seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to generate key pair" << endl;

	gettimeofday(&startTime, NULL);
	unsigned char * password = (unsigned char *)"password";
	cout << "Hashing and signing \"" << password << "\"" << endl;
	unsigned char hash[20];
	SHA1(password, strlen((char *)password), hash);
	unsigned char * sig = (unsigned char *)malloc(DSA_size(dsa));
	unsigned int siglen;
	DSA_sign(NID_sha1, hash, 20, sig, &siglen, dsa);
	gettimeofday(&now, NULL);
	seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to hash and sign" << endl;

	cout << "Signature: ";
	PrintHex(sig, siglen);
	cout << endl;

	gettimeofday(&startTime, NULL);
	cout << "Starting to do " << attempts << " verification requests" << endl;
	for(int x = 0; x < attempts; x++)
	{
		SHA1(hash, 20, hash);
		if(DSA_verify(NID_sha1, hash, 20, sig, siglen, dsa))
		{
			cout << "False positive" << endl;
			return;
		}
	}
	gettimeofday(&now, NULL);
	seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to verify " << attempts << " times" << endl;

	free(sig);

	cout << endl << endl;
}

//Makes a ECDSA hash of the given bit size and benchmarks cracking speed
void TestEC(int bits)
{
	if(bits == 714)
		cout << "Generating " << 256 << " bit ECDSA key pair" << endl;
	if(bits == 715)
		cout << "Generating " << 384 << " bit ECDSA key pair" << endl;
	if(bits == 716)
		cout << "Generating " << 521 << " bit ECDSA key pair" << endl;
	struct timeval startTime;
	gettimeofday(&startTime, NULL);

	ECDSA_SIG * sig;
	EC_KEY *eckey = EC_KEY_new();
	EC_KEY_set_group(eckey, EC_GROUP_new_by_curve_name(bits));
	EC_KEY_generate_key(eckey);
	struct timeval now;
	gettimeofday(&now, NULL);
	double seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to generate key pair" << endl;

	gettimeofday(&startTime, NULL);
	unsigned char * password = (unsigned char *)"password";
	cout << "Hashing and signing \"" << password << "\"" << endl;
	unsigned char hash[20];
	SHA1(password, strlen((char *)password), hash);

	sig = ECDSA_do_sign(hash, 20, eckey);
	gettimeofday(&now, NULL);
	seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to hash and sign" << endl;

	cout << "Signature: " << BN_bn2hex(sig->r) << BN_bn2hex(sig->s) << endl;

	gettimeofday(&startTime, NULL);
	cout << "Starting to do " << attempts << " verification requests" << endl;
	for(int x = 0; x < attempts; x++)
	{
		SHA1(hash, 20, hash);
		if(ECDSA_do_verify(hash, 20, sig, eckey) == 1)
		{
			cout << "False positive" << endl;
			return;
		}
	}
	gettimeofday(&now, NULL);
	seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to verify " << attempts << " times" << endl;

	free(sig);

	cout << endl << endl;
}

void ControlTest()
{
	cout << "Conducting control of " << attempts << " SHA1 hashes" << endl;
	struct timeval startTime, now;
	gettimeofday(&startTime, NULL);
	unsigned char hash[20];
	for(int x = 0; x < attempts; x++)
	{
		SHA1(hash, 20, hash);
		//Done to properly benchmark verifying if the password is right
		memcmp(hash, hash, 20);
	}
	gettimeofday(&now, NULL);
	double seconds = (now.tv_sec + ((double)now.tv_usec / 1000000)) - (startTime.tv_sec + ((double)startTime.tv_usec / 1000000));
	cout << "Took " << seconds << " seconds to hash " << attempts << " times" << endl;
	cout << endl << endl;
}

int main()
{
	ControlTest();

	TestEC(NID_secp256k1);
	TestEC(NID_secp384r1);
	TestEC(NID_secp521r1);

	Test(1024);
	Test(2048);
	Test(4096);
}
