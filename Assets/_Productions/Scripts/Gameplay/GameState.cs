namespace CapsaBanting
{
    [System.Serializable]
    public class GameState
    {
        public CardHand lastPlayerHand = new();
        public CardCombinationType lastCombinationType;
        public Player lastPlayerTurn;
    }
}
