namespace CapsaBanting
{
    public static class CardExtensions
    {
        public static int GetValue(this Card card)
        {
            return (int)card.face;
        }
        
        public static int GetSuitValue(this Card card)
        {
            return (int)card.suit;
        }

        public static bool IsHigherThan(this CardHand hand1, CardHand hand2)
        {
            if (hand1.HighCard > hand2.HighCard)
            {
                return true;
            }
            
            if (hand1.HighCard == hand2.HighCard)
            {
                if (hand1.BestSuit > hand2.BestSuit)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
