#include <iostream>
#include <vector>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <string>
#include <string.h>

using namespace std;

class BigNumber
{
public:
	BigNumber()
	{
		number = new unsigned char [1];
		*(number) = 0x00;
		length = 0;
	}

	BigNumber(unsigned char * num, unsigned int len)
	{
		number = new unsigned char[len];
		memcpy(number, num, len);
		length = len - 1;
	}

	void SetValue(unsigned char * num, unsigned int len)
	{
		delete [] number;
		number = new unsigned char[len + 1];
		memcpy(number, num, len + 1);
		length = len;
	}

	bool operator==(const BigNumber &other)
	{
		if(length != other.length)
			return false;
		for(unsigned int x = 0; x <= length; x++)
		{
			if(*(number + x) != *(other.number + x))
				return false;
		}
		return true;
	}

	BigNumber& operator=(const BigNumber &other)
	{
		SetValue(other.number, other.length);
		return *this;
	}

	BigNumber& operator+=(const BigNumber &other)
	{
		for(int x = 0; x <= other.length; x++)
		{
			AddUCharToPosition(*(other.number + x), x);
		}
		return *this;
	}

	void Add(const BigNumber &other)
	{
		for(int x = 0; x <= other.length; x++)
		{
			AddUCharToPosition(*(other.number + x), x);
		}
	}

	BigNumber operator+(const BigNumber &other)
	{
		BigNumber * temp = new BigNumber(number, length + 1);
		temp->Add(other);
		return *temp;
	}

	void operator++()
	{
		AddUCharToPosition(0x01, 0);
	}

	unsigned char FirstByte()
	{
		return *(number);
	}

	bool IsZero()
	{
		for(unsigned int x = 0; x <= length; x++)
		{
			if(*(number + x) != 0x00)
				return false;
		}
		return true;
	}

	BigNumber& operator*=(const BigNumber &other)
	{
		BigNumber zero;
		BigNumber c;
		BigNumber b;
		b.SetValue(other.number, other.length);
		BigNumber a;
		a.SetValue(number, length);
		while(!b.IsZero())
		{
			if(b.FirstByte() & 0x01 == 0x01)
				c.Add(a);
			a <<= 1;
			b >>= 1;
		}
		SetValue(c.number, c.length);
		return *this;
	}

	BigNumber operator*(const BigNumber &other)
	{
		BigNumber * temp = new BigNumber(number, length + 1);
		return (*temp *= other);
	}

	BigNumber& operator>>=(unsigned int other)
	{
		if(other < 8)
		{
			unsigned char lastbyte = 0x00;
			unsigned char temp = 0x00;
			for(long long x = length; x >= 0; x--)
			{
				temp = *(number + x);
				*(number + x) >>= other;
				*(number + x) |= (lastbyte << (8 - other));
				lastbyte = temp;
			}
		}
		/*else if(other == 8)
		{
			if(length == 0)
			{
				*number = 0x00;
				return *this;
			}
			unsigned char * num = new unsigned char[length];
			memcpy(num, number + 1, length);
			length--;
			delete [] number;
			number = num;
		}*/
		else
		{
			while(other > 0)
			{
				if(other >= 7)
				{
					*this >>= 7;
					other -= 7;
				}
				else
				{
					*this >>= other;
					other = 0;
				}
			}
		}
		for(unsigned int x = length; x != 0; x--)
		{
			if(*(number + x) == 0x00)
				length--;
			else
				break;
		}
		return *this;
	}

	BigNumber& operator<<=(unsigned int other)
	{
		if(other < 8)
		{
			unsigned char lastbyte = 0x00;
			unsigned char temp = 0x00;
			for(unsigned int x = 0; x <= length; x++)
			{
				temp = *(number + x);
				*(number + x) <<= other;
				*(number + x) |= (lastbyte >> (8 - other));
				lastbyte = temp;
			}
			if((lastbyte >> (8 - other)) > 0)
			{
				AddUCharToPosition(lastbyte >> (8 - other), length + 1);
			}
		}
		/*else if(other == 8)
		{
			length++;
			unsigned char * num = new unsigned char[length + 1];
			memcpy(num + 1, number, length);
			delete [] number;
			number = num;
			*number = 0x00;
		}*/
		else
		{
			while(other > 0)
			{
				if(other >= 7)
				{
					*this <<= 7;
					other -= 7;
				}
				else
				{
					*this <<= other;
					other = 0;
				}
			}
		}
		return *this;
	}

	BigNumber operator<<(const unsigned int other)
	{
		BigNumber * temp = new BigNumber(number, length + 1);
		return (*temp <<= other);
	}

	BigNumber operator>>(const unsigned int other)
	{
		BigNumber * temp = new BigNumber(number, length + 1);
		return (*temp >>= other);
	}

	//Returns true if the resulting number is negative
	bool Subtract(const BigNumber &other)
	{
		if(other.length > length || (other.length == length && *(other.number + other.length) > *(number + length)))
		{
			delete [] number;
			length = 0;
			number = new unsigned char[1];
			*number = 0x00;
			return true;
		}
		for(unsigned int x = 0; x <= other.length; x++)
		{
			if(SubtractUCharFromPosition(*(other.number + x), x))
				return true;
		}
		for(unsigned int x = length; x != 0; x--)
		{
			if(*(number + x) == 0x00)
				length--;
			else
				break;
		}
		return false;
	}

	BigNumber * Modulus(const BigNumber &other)
	{
		BigNumber temp(number, length + 1);
		return temp.Divide(other);
	}

	bool IsDivisableBy(const BigNumber &other)
	{
		if(Modulus(other)->IsZero())
			return true;
		return false;
	}

	unsigned int GetLength()
	{
		return length;
	}

	BigNumber operator&(const BigNumber &other)
	{
		BigNumber * temp = new BigNumber();

		if(length > other.length)
			temp->length = other.length;
		else
			temp->length = length;

		unsigned char * num = new unsigned char[temp->length + 1];

		for(unsigned int x = 0; x <= temp->length; x++)
		{
			*(num + x) = *(number + x) & *(other.number + x);
		}

		temp->number = num;

		return *temp;
	}

	bool GetBitAt(unsigned long location)
	{
		unsigned int byte = location / 8;
		unsigned char bit = 0x01 << location % 8;
		if(*(number + byte) & bit > 0)
			return true;
		else
			return false;
	}

	BigNumber * Divide(const BigNumber &other)
	{
		BigNumber a;
		BigNumber b;
		long i = length;

		while(i >= 0)
		{
			a <<= 8;
			a.AddUCharToPosition(*(number + i), 0);
			b <<= 8;
			while(a >= other)
			{
				a.Subtract(other);
				b.AddUCharToPosition(0x01, 0);
			}
			i--;
		}

		BigNumber * temp = new BigNumber(a.number, a.length + 1);
		SetValue(b.number, b.length);
		return temp;
	}

	void FromString(unsigned char * input)
	{
		ReverseNumbers(input);
		unsigned char * one = new unsigned char[1];
		unsigned char * five = new unsigned char[1];
		*five = 0x05;
		*one = 0x01;
		for(unsigned int x = 0; x < strlen((char*)input); x++)
		{
			if(*(input + x) == 0x30)
				continue;
			*one = *(input + x) - 0x30;
			BigNumber toadd(one, 1);
			for(unsigned int y = 0; y < x; y++)
			{
				BigNumber tomultiply(five, 1);
				toadd <<= 1;
				toadd *= tomultiply;
			}
			Add(toadd);
		}
	}

	bool operator>(const BigNumber &other)
	{
		if(other.length > length)
			return false;
		if(other.length < length)
			return true;
		for(int x = length; x >= 0; x++)
		{
			if(*(number + x) > *(other.number + x))
				return true;
			else if(*(number + x) < *(other.number + x))
				return false;
		}
		return false;
	}

	bool operator<=(const BigNumber &other)
	{
		if(other.length > length)
			return true;
		if(other.length < length)
			return false;
		for(int x = length; x >= 0; x--)
		{
			if(*(number + x) > *(other.number + x))
				return false;
			else if(*(number + x) < *(other.number + x))
				return true;
		}
		return true;
	}

	bool operator>=(const BigNumber &other)
	{
		if(other.length > length)
			return false;
		if(other.length < length)
			return true;
		for(int x = length; x >= 0; x--)
		{
			if(*(number + x) > *(other.number + x))
				return true;
			else if(*(number + x) < *(other.number + x))
				return false;
		}
		return true;
	}

	BigNumber * sqrt()
	{
		BigNumber op(number, length + 1);
		BigNumber * res = new BigNumber();
		unsigned char * uno = new unsigned char[1];
		*uno = 0x01;
		BigNumber one(uno, 1);
		one <<= ((length + 1) * 8) - 2;

		while(one > op)
			one >>= 2;

		while(!one.IsZero())
		{
			if(op >= *res + one)
			{
				op.Subtract(*res + one);
				*res >>= 1;
				res->Add(one);
			}
			else
				*res >>= 1;
			one >>= 2;
		}
		delete [] uno;
		return res;
	}

	string ToString()
	{
		string out;
		BigNumber temp(number, length + 1);
		unsigned char * diez = new unsigned char[1];
		*diez = 10;
		BigNumber ten(diez, 1);
		unsigned char t;
		while(!temp.IsZero())
		{
			BigNumber * temp2 = temp.Divide(ten);
			t = *(temp2->number) + 0x30;
			out.insert(0, 1, t);
			delete temp2;
		}
		return out;
	}

	~BigNumber()
	{
		delete [] number;
	}

	unsigned char * number;
	unsigned int length;

	void ReverseNumbers(unsigned char * input)
	{
		unsigned char temp = *(input);
		for(unsigned int x = 0; x < strlen((char*)input) / 2; x++)
		{
			temp = *(input + x);
			*(input + x) = *(input + strlen((char*)input) - 1 - x);
			*(input + strlen((char*)input) - 1 - x) = temp;
		}
	}

	bool SubtractUCharFromPosition(unsigned char in, unsigned int position)
	{
		if(position > length || (position == length && in > *(number + length)))
		{
			delete [] number;
			length = 0;
			number = new unsigned char[1];
			*number = 0x00;
			return true;
		}

		bool carry = false;
		if(in > *(number + position))
			carry = true;
		else
		{
			*(number + position) -= in;
			return false;
		}

		*(number + position) -= in;

		while(carry)
		{
			position++;
			if(position > length)
			{
				delete [] number;
				length = 0;
				number = new unsigned char[1];
				*number = 0x00;
				return true;
			}

			if(*(number + position) == 0x00)
			{
				*(number + position) = 0xff;
			}
			else
			{
				*(number + position) = *(number + position) - 1;
				return false;
			}
		}
		return false;
	}

	void AddUCharToPosition(unsigned char in, unsigned int position)
	{
		if(position > length)
		{
			unsigned char * temp = new unsigned char[position + 1];
			memset(temp, 0, position + 1);
			memcpy(temp, number, length + 1);
			length = position ;
			delete [] number;
			number = temp;
		}
		if((int)in + (int)*(number + position) > 255)
			AddUCharToPosition(0x01, position + 1);
		*(number + position) += in;
	}
};

class PrimeFile
{
public:
	FILE * rFile;
	FILE * wFile;
	long lSize;

	PrimeFile(char * file)
	{
		wFile = fopen(file, "ab");
		rFile = fopen(file, "rb");
		fseek (rFile , 0 , SEEK_END);
		lSize = ftell (rFile);
		rewind (rFile);
	}

	BigNumber * GetNextPrime()
	{
		unsigned int * l = new unsigned int[1];
		fread(l, 4, 1, rFile);
		unsigned char * n = new unsigned char[*l + 1];
		fread(n, 1, *l + 1, rFile);
		BigNumber * bignum = new BigNumber();
		bignum->number = n;
		bignum->length = *l;
		return bignum;
	}

	void WriteNewPrime(BigNumber * number)
	{
		fseek (wFile , 0 , SEEK_END);
		fwrite(&(number->length), 4, 1, wFile);
		fwrite(number->number, 1, number->length + 1, wFile);
	}

	bool eof()
	{
		if(rFile == NULL || ftell(rFile) == lSize)
			return true;
		return feof(rFile);
	}

	void close()
	{
		fclose(wFile);
		fclose(rFile);
	}
};

unsigned long long sqrt(unsigned long long num) 
{
    unsigned long long op = num;
    unsigned long long res = 0;
    unsigned long long one = 1;
	one = one << ((sizeof(unsigned long long) * 8) - 2);

    while (one > op)
        one >>= 2;

    while (one != 0) {
        if (op >= res + one) {
            op -= res + one;
            res = (res >> 1) + one;
        }
        else
          res >>= 1;
        one >>= 2;
    }
    return res;
}

vector<BigNumber*> * FactorBigNumber(BigNumber * number)
{
	vector<BigNumber*> * factor = new vector<BigNumber*>();
	BigNumber tempnumber(number->number, number->length + 1);
	BigNumber * squareroot = number->sqrt();
	squareroot->AddUCharToPosition(0x01, 0);
	BigNumber t;
	++t;
	BigNumber * x = new BigNumber();
	PrimeFile pFile("primes");
	x->AddUCharToPosition(0x02, 0);

	while(*x <= *squareroot && !pFile.eof())
	{
		delete x;
		x = pFile.GetNextPrime();
		BigNumber * mod = tempnumber.Modulus(*x);
		while(mod->IsZero())
		{
			BigNumber * remainder = tempnumber.Divide(*x);
			delete remainder;
			BigNumber * temp = new BigNumber(x->number, x->length + 1);
			factor->push_back(temp);
			t *= *x;
			delete mod;
			mod = tempnumber.Modulus(*x);
			if(t == *number)
			{
				pFile.close();
				return factor;
			}
		}
		delete mod;
	}

	pFile.close();

	for(; *x <= *squareroot; x->AddUCharToPosition(0x01, 0))
	{
		BigNumber * mod = tempnumber.Modulus(*x);
		while(mod->IsZero())
		{
			BigNumber * remainder = tempnumber.Divide(*x);
			delete remainder;
			BigNumber * temp = new BigNumber(x->number, x->length + 1);
			factor->push_back(temp);
			t *= *x;
			delete mod;
			mod = tempnumber.Modulus(*x);
			if(t == *number)
				return factor;
		}
		delete mod;
	}
	BigNumber * temp = new BigNumber(tempnumber.number, tempnumber.length + 1);
	factor->push_back(temp);
	delete squareroot;
	return factor;
}

vector<unsigned long long> FactorULong(unsigned long long number)
{
	vector<unsigned long long> factor;
	unsigned long long tempnumber = number;
	unsigned long long squareroot = sqrt(number) + 1;
	unsigned long long t = 1;
	for(unsigned long long x = 2; x <= squareroot; x++)
	{
		while(tempnumber % x == 0)
		{
			tempnumber = tempnumber / x;
			factor.push_back(x);
			t = t * x;
			if(t == number)
				return factor;
		}
	}
	factor.push_back(tempnumber);
	return factor;
}

int main(int argc, char ** argv)
{
	BigNumber number;
	vector<BigNumber*> * factor;
	if(argc < 2)
	{
		cout << "Input the number you wish to factor: ";
		unsigned char * charnum = new unsigned char[65535];
		cin >> charnum;
		number.FromString(charnum);

		cout << number.ToString() << endl;
		if(number.IsZero())
		{
			cout << "Invalid number.\n";
			return 0;
		}
		
		factor = FactorBigNumber(&number);
		
		if(factor->size() == 1)
		{
			cout << number.ToString() << " is prime!\n";
		}
		else
		{
			cout << number.ToString() << " = ";
			for(int x = 0; x < factor->size(); x++)
			{
				cout << factor->at(x)->ToString();
				if(x != factor->size() - 1)
					cout << "*";
			}
			cout << endl;
		}
	}
	else if(argc == 2)
	{
		if(strcmp(argv[1], "-a") == 0)
		{
			//add to primes file
			PrimeFile pFile("primes");
			BigNumber * last = new BigNumber();
			last->AddUCharToPosition(0x01, 0);
			while(!pFile.eof())
			{
				delete last;
				last = pFile.GetNextPrime();
			}

			cout << "Input the number of primes you would like to add: ";
			unsigned char * charnum = new unsigned char[65535];
			cin >> charnum;
			BigNumber count;
			count.FromString(charnum);
			BigNumber x;

			while(x <= count)
			{
				last->AddUCharToPosition(0x01, 0);
				factor = FactorBigNumber(last);
				
				if(factor->size() == 1)
				{
					pFile.WriteNewPrime(last);
					x.AddUCharToPosition(0x01, 0);
					cout << x.ToString() << ": " << last->ToString() << endl;
				}
			}
			pFile.close();
		}
		else
		{
			number.FromString((unsigned char*)argv[1]);
			cout << number.ToString() << endl;
			if(number.IsZero())
			{
				cout << "Invalid number.\n";
				return 0;
			}
			
			factor = FactorBigNumber(&number);
			
			if(factor->size() == 1)
			{
				cout << number.ToString() << " is prime!\n";
			}
			else
			{
				cout << number.ToString() << " = ";
				for(int x = 0; x < factor->size(); x++)
				{
					cout << factor->at(x)->ToString();
					if(x != factor->size() - 1)
						cout << "*";
				}
				cout << endl;
			}
		}
	}
	//cin.get();
}
