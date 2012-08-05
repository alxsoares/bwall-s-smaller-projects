#ifndef PASSKEY_H
#define PASSKEY_H
#include <string.h>
#include <iostream>
#include <iomanip>
#include "Blob.h"
#include "SHA256.h"

using namespace std;

class PassKey
{
    public:
        PassKey();
        PassKey(unsigned char * Salt, unsigned int N, unsigned char * stretchedKey);
        PassKey(PassKey * pk);
        void GetPreIterationHash(const Blob * b, unsigned char * output);
        unsigned char * StretchedKey;
        virtual ~PassKey();
    protected:
    private:
        unsigned char temp[32];
        unsigned char * Salt;
        unsigned int N;
};

#endif // PASSKEY_H
