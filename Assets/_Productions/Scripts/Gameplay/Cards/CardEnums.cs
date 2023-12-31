namespace CapsaBanting
{
    public enum CardFace
    {
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine =  9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
        Two = 15
    }
        
    public enum CardSuit
    {
        Diamonds = 1,
        Clubs = 2,
        Hearts = 3,
        Spades = 4
    }
    
    public enum CardCombinationType
    {
        Invalid,
        Single,
        Pair,
        Triple,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
}