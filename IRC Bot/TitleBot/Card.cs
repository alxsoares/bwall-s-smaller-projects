using System;
using System.Collections.Generic;
using System.Text;

namespace TitleBot
{
    class Card : IComparable<Card>
    {
        public int suit = -1;
        public int value = -1;

        public Card() { }

        public Card(int v, int s)
        {
            suit = s;
            value = v;
        }

        public override string ToString()
        {
            string ssuite = "";
            switch (suit)
            {
                case 0:
                    ssuite = "C";
                    break;
                case 1:
                    ssuite = "D";
                    break;
                case 2:
                    ssuite = "H";
                    break;
                case 3:
                    ssuite = "S";
                    break;
            }
            switch (value)
            {
                case 1:
                    return "A" + ssuite;
                case 11:
                    return "J" + ssuite;
                case 12:
                    return "Q" + ssuite;
                case 13:
                    return "K" + ssuite;
                default:
                    return value.ToString() + ssuite;
            }
        }

        public int CompareTo(Card other)
        {
            if (value == 1)
            {
                if (other.value != 1)
                    return 14 - other.value;
                else
                    return 0;
            }
            if (other.value == 1)
            {
                if (value != 1)
                    return value - 14;
                else
                    return 0;
            }
            return value.CompareTo(other.value);
        }
    }
}
