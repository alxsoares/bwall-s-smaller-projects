using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace TitleBot
{
    class Hand : IComparable<Hand>
    {
        public Card highcard;
        public int handvalue;
        public List<Card> cards = new List<Card>();
        public List<Card> relavantCards = new List<Card>();
        public int TripValue = -1;
        public int PairValue = -1;
        public int PairValue2 = -1;

        public void AddCard(Card c)
        {
            cards.Add(c);
        }

        public void CheckForHand()
        {
            if (CFStraightFlush())
            {
                handvalue = 8;
                return;
            }
            if (CFFourOfAKind())
            {
                handvalue = 7;
                return;
            }
            if (CFFullHouse())
            {
                handvalue = 6;
                return;
            }
            if (CFFlush())
            {
                handvalue = 5;
                return;
            }
            if (CFStraight())
            {
                handvalue = 4;
                return;
            }
            if (CFThreeOfAKind())
            {
                handvalue = 3;
                return;
            }
            if (CFTwoPair())
            {
                handvalue = 2;
                return;
            }
            if (CFPair())
            {
                handvalue = 1;
                return;
            }
            handvalue = 0;
        }

        public void ClearHand()
        {
            cards.Clear();
            relavantCards.Clear();
        }

        public string GetHandName()
        {
            switch (handvalue)
            {
                case 8:
                    return "Straight Flush";
                case 7:
                    return "Four of Kind";
                case 6:
                    return "Full House";
                case 5:
                    return "Flush";
                case 4:
                    return "Straigth";
                case 3:
                    return "Three of a Kind";
                case 2:
                    return "Two Pair";
                case 1:
                    return "Pair";
                default:
                    return "High Card";
            }
        }

        public string GetCardName(int card)
        {
            return cards[card].ToString();
        }

        void MoveCardToRelavant(int suit, int value)
        {
            cards.Remove(new Card(value, suit));
            relavantCards.Add(new Card(value, suit));
        }

        bool CFStraightFlush()
        {
            Dictionary<int, Card> c = new Dictionary<int, Card>();
            for (int x = 0; x < cards.Count; x++)
            {
                if (c.ContainsValue(cards[x]))
                    c.Add(cards[x].value, cards[x]);
            }
            List<Card> str = new List<Card>();
            int last = 0;
            int lasts = 0;
            bool acepresent = false;
            List<int> acesuites = new List<int>();
            foreach (KeyValuePair<int, Card> card in c)
            {
                if (last == 0)
                {
                    if (card.Key == 1)
                    {
                        acepresent = true;
                        acesuites.Add(card.Value.suit);
                    }
                    last = card.Key;
                    str.Add(new Card(1, lasts));
                    last = card.Value.suit;
                }
                else if (last + 1 == card.Key && lasts == card.Value.suit)
                {
                    last++;
                    str.Add(card.Value);
                    if (acepresent && card.Key == 13)
                    {
                        for (int x = 0; x < acesuites.Count; x++)
                        {
                            if (acesuites[x] == lasts)
                            {
                                str.Add(new Card(1, lasts));
                            }
                        }
                    }
                }
                else
                {
                    str.Clear();
                    last = 0;
                    lasts = 0;
                }
            }
            c.Clear();
            if (str.Count >= 5)
            {
                for (int x = str.Count - 5; x < str.Count; x++)
                {
                    highcard = str[x];
                    if (highcard.value == 1)
                        highcard.value = 14;
                    MoveCardToRelavant(str[x].suit, str[x].value);               
                }
                str.Clear();
                return true;
            }
            str.Clear();
            return false;
        }

        bool CFFourOfAKind()
        {
            Dictionary<int, int> count = new Dictionary<int, int>();
            foreach (Card card in cards)
            {
                if (count.ContainsKey(card.value))
                    count[card.value]++;
                else
                    count.Add(card.value, 1);
            }
            foreach (KeyValuePair<int, int> c in count)
            {
                if (c.Value == 4)
                {
                    foreach (Card card in cards)
                    {
                        if (card.value == c.Key)
                        {
                            highcard = card;
                            MoveCardToRelavant(card.suit, card.value);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        bool CFFullHouse()
        {
            Dictionary<int, int> c = new Dictionary<int, int>();
            for (int x = 0; x < cards.Count; x++)
            {
                if (c.ContainsKey(cards[x].value))
                    c[cards[x].value]++;
                else
                    c.Add(cards[x].value, 1);
            }
            bool three = false;
            bool two = false;
            List<Card> relavant = new List<Card>();
            foreach (KeyValuePair<int, int> card in c)
            {
                if (card.Value == 3)
                {
                    three = true;
                    TripValue = card.Key;
                    foreach (Card ca in cards)
                    {
                        if (card.Key == ca.value)
                            relavant.Add(ca);
                    }
                }
                else if (card.Value == 2)
                {
                    two = true;
                    PairValue = card.Key;
                    foreach (Card ca in cards)
                    {
                        if (card.Key == ca.value)
                            relavant.Add(ca);
                    }
                }
            }
            if (three && two)
            {
                foreach (Card ca in relavant)
                {
                    if (highcard.value < ca.value)
                        highcard = ca;
                    MoveCardToRelavant(ca.suit, ca.value);
                }
                return true;
            }
            return false;
        }

        bool CFFlush()
        {
            Dictionary<int, int> c = new Dictionary<int, int>();
            foreach (Card ca in cards)
            {
                if (c.ContainsKey(ca.suit))
                    c[ca.suit]++;
                else
                    c.Add(ca.suit, 1);
            }
            bool flush = false;
            int ssuit = -1;
            foreach (KeyValuePair<int, int> ca in c)
            {
                if (ca.Value >= 5)
                {
                    flush = true;
                    ssuit = ca.Key;
                }
            }
            if (flush)
            {
                List<Card> rel = new List<Card>();
                foreach (Card card in cards)
                {
                    if (card.suit == ssuit)
                    {
                        rel.Sort();
                        if (rel.Count == 5)
                        {
                            if (card.CompareTo(rel[0]) > 0)
                            {
                                rel.RemoveAt(0);
                                rel.Add(card);
                            }
                        }
                        else
                            rel.Add(card);
                    }
                }
                for (int x = 0; x < 5; x++)
                    MoveCardToRelavant(rel[x].suit, rel[x].value);
                return true;
            }
            return false;
        }

        bool CFStraight()
        {
            SortedDictionary<int, Card> c = new SortedDictionary<int, Card>();
            foreach (Card ca in cards)
            {
                c[ca.value] = ca;
            }
            List<Card> str = new List<Card>();
            Card ace = new Card();
            ace.value = 1;
            bool acepresent = false;
            int last = 0;
            foreach (KeyValuePair<int, Card> ca in c)
            {
                if (last == 0)
                {
                    if (ca.Key == 1)
                    {
                        acepresent = true;
                        ace.suit = ca.Value.suit;
                    }
                    last = ca.Key;
                    str.Add(ca.Value);
                }
                else if (last + 1 == ca.Key)
                {
                    last++;
                    str.Add(ca.Value);
                    if (acepresent && ca.Key == 13)
                    {
                        str.Add(ace);
                    }
                }
                else
                {
                    str.Clear();
                    last = 0;
                }
            }
            if (str.Count > 4)
            {
                for (int x = str.Count - 5; x < str.Count; x++)
                {
                    highcard = str[x];
                    if (highcard.value == 1)
                        highcard.value = 14;
                    MoveCardToRelavant(str[x].suit, str[x].value);
                }
                return true;
            }
            return false;
        }

        bool CFThreeOfAKind()
        {
            Dictionary<int, int> count = new Dictionary<int, int>();
            bool trips = false;
            int tripval = 0;
            foreach (Card c in cards)
            {
                if (count.ContainsKey(c.value))
                {
                    count[c.value]++;
                    if (count[c.value] == 3)
                    {
                        trips = true;                     
                        if (c.value > tripval)
                            tripval = c.value;
                    }
                }
                else
                    count.Add(c.value, 1);
            }
            if (trips)
            {
                TripValue = tripval;
                foreach (Card c in cards)
                {
                    if (c.value == tripval)
                    {
                        highcard = c;
                        MoveCardToRelavant(c.suit, c.value);
                    }
                }
                return true;
            }
            return false;
        }

        bool CFTwoPair()
        {
            Dictionary<int, int> c = new Dictionary<int, int>();
            for (int x = 0; x < cards.Count; x++)
            {
                if (c.ContainsKey(cards[x].value))
                    c[cards[x].value]++;
                else
                    c.Add(cards[x].value, 1);
            }
            bool three = false;
            bool two = false;
            List<Card> relavant = new List<Card>();
            foreach (KeyValuePair<int, int> card in c)
            {
                if (card.Value == 2)
                {
                    if (three)
                    {
                        two = true;
                        PairValue2 = card.Key;
                    }
                    else
                    {
                        PairValue = card.Key;
                        three = true;
                    }
                    foreach (Card ca in cards)
                    {
                        if (card.Key == ca.value)
                            relavant.Add(ca);
                    }
                }
            }
            if (three && two)
            {
                foreach (Card ca in relavant)
                {
                    if (highcard.value < ca.value)
                        highcard = ca;
                    MoveCardToRelavant(ca.suit, ca.value);
                }
                return true;
            }
            return false;
        }

        bool CFPair()
        {
            Dictionary<int, int> c = new Dictionary<int, int>();
            for (int x = 0; x < cards.Count; x++)
            {
                if (c.ContainsKey(cards[x].value))
                    c[cards[x].value]++;
                else
                    c.Add(cards[x].value, 1);
            }
            bool three = false;
            bool two = false;
            List<Card> relavant = new List<Card>();
            foreach (KeyValuePair<int, int> card in c)
            {
                if (card.Value == 2)
                {
                    two = true;
                    PairValue = card.Key;
                    foreach (Card ca in cards)
                    {
                        if (card.Key == ca.value)
                            relavant.Add(ca);
                    }
                }
            }
            if (two)
            {
                foreach (Card ca in relavant)
                {
                    if (highcard.value < ca.value)
                        highcard = ca;
                    MoveCardToRelavant(ca.suit, ca.value);
                }
                return true;
            }
            return false;
        }

        List<Card> GetHighestCards()
        {
            cards.Sort();
            cards.Reverse();
            return cards;
        }

        List<Card> GetTopCardsOfSuit(int suit)
        {
            List<Card> rel = new List<Card>();
            foreach (Card card in cards)
            {
                if (card.suit == suit)
                {
                    rel.Sort();
                    if (rel.Count == 5)
                    {
                        if (card.CompareTo(rel[0]) > 0)
                        {
                            rel.RemoveAt(0);
                            rel.Add(card);
                        }
                    }
                    else
                        rel.Add(card);
                }
            }
            return rel;
        }


        public int CompareTo(Hand other)
        {
            CheckForHand();
            other.CheckForHand();
            if (handvalue == other.handvalue)
            {
                switch (handvalue)
                {
                    case 8:
                        return highcard.CompareTo(other.highcard);
                    case 7:
                        if (highcard.CompareTo(other.highcard) == 0)                        
                            return GetHighestCards()[0].CompareTo(other.GetHighestCards()[0]);                        
                        else                        
                            return highcard.CompareTo(other.highcard);  
                    case 6:
                        if (TripValue == other.TripValue)
                        {
                            if (PairValue == other.PairValue)
                                return 0;
                            return new Card(PairValue, 0).CompareTo(new Card(other.PairValue, 0));
                        }
                        else
                            return new Card(TripValue, 0).CompareTo(new Card(other.TripValue,0));
                    case 5:
                        //Flush
                        relavantCards.Sort();
                        relavantCards.Reverse();
                        other.relavantCards.Sort();
                        other.relavantCards.Reverse();
                        for (int x = 0; x < relavantCards.Count; x++)
                        {
                            if (relavantCards[x].CompareTo(other.relavantCards[x]) != 0)
                                return relavantCards[x].CompareTo(other.relavantCards[x]);
                        }
                        return 0;
                    case 4:
                        //Straight
                        return highcard.CompareTo(other.highcard);
                    case 3:
                        if (TripValue == other.TripValue)
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                if (GetHighestCards()[x].CompareTo(other.GetHighestCards()[x]) != 0)
                                    return GetHighestCards()[x].CompareTo(other.GetHighestCards()[x]);
                            }
                            return 0;
                        }
                        else
                        {
                            return new Card(TripValue, 0).CompareTo(new Card(other.TripValue, 0));
                        }
                    case 2:
                        if (PairValue == other.PairValue)
                        {
                            for (int x = 0; x < 3; x++)
                            {
                                if (GetHighestCards()[x].CompareTo(other.GetHighestCards()[x]) != 0)
                                    return GetHighestCards()[x].CompareTo(other.GetHighestCards()[x]);
                            }
                            return 0;
                        }
                        else
                            return new Card(PairValue, 0).CompareTo(new Card(other.PairValue, 0));
                    case 1:
                        for (int x = 0; x < 5; x++)
                        {
                            if (GetHighestCards()[x].CompareTo(other.GetHighestCards()[x]) != 0)
                                return GetHighestCards()[x].CompareTo(other.GetHighestCards()[x]);
                        }
                        return 0;
                }
                return 0;
            }
            else
                return handvalue - other.handvalue;
        }
    }
}
