#include "../include/PassKey.h"

PassKey::PassKey()
{
    //ctor
}

PassKey::PassKey(PassKey * pk)
{
    this->Salt = new unsigned char[32];
    memcpy(this->Salt, pk->Salt, 32);
    this->N = pk->N;
    this->StretchedKey = new unsigned char[32];
    memcpy(this->StretchedKey, pk->StretchedKey, 32);
}

PassKey::~PassKey()
{
    //dtor
}

PassKey::PassKey(unsigned char * Salt, unsigned int N, unsigned char * stretchedKey)
{
    this->Salt = Salt;
    this->N = N;
    this->StretchedKey = stretchedKey;
}

void PassKey::GetPreIterationHash(const Blob * b, unsigned char * output)
{
    SHA256 sha;
    sha.Update((unsigned char*)b->data, b->size);
    sha.Update(Salt, 32);
    sha.Finalize();
    sha.Digest(output);
}
