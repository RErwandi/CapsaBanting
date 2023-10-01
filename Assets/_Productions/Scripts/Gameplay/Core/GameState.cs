using UniRx;

namespace CapsaBanting
{
    [System.Serializable]
    public class GameState
    {
        public ReactiveCollection<CardHand> lastPlayerHands = new();
        public int lastPlayerTurn;

        public CardHand LastPlayerHand
        {
            get
            {
                if (lastPlayerHands.Count > 0)
                {
                    return lastPlayerHands[^1];
                }

                return new CardHand();
            }
        }

        public void Clear()
        {
            lastPlayerHands.Clear();
        }
    }
}
