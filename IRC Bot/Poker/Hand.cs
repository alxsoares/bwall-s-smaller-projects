using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace PokerBot
{
    class Hand
    {
        public Card highcard;
        public int handvalue;
        public List<Card> cards = new List<Card>();
        public List<Card> relavantCards = new List<Card>();

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

        //do later
        public string GetHandName()
        {
            return null;
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
                    foreach (Card ca in cards)
                    {
                        if (card.Key == ca.value)
                            relavant.Add(ca);
                    }
                }
                else if (card.Value == 2)
                {
                    two = true;
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
            return false;
        }

        bool CFThreeOfAKind()
        {
            return false;
        }

        bool CFTwoPair()
        {
            return false;
        }

        bool CFPair()
        {
            return false;
        }

        List<Card> GetHighestCards()
        {
            return null;
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

    }
}
