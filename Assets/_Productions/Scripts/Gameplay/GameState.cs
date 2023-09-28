namespace CapsaBanting
{
    [System.Serializable]
    public class GameState
    {
        public CardHand lastPlayerHand = new();
        public Player lastPlayerTurn;
    }
}
