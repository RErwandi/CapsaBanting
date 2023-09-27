namespace CapsaBanting
{
    public static class CardExtensions
    {
        public static int GetValue(this Card card)
        {
            return (int)card.face;
        }
    }
}
