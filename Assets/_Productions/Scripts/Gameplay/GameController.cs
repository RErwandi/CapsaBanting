using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    public class GameController : MonoBehaviour
    {
        [SerializeField, LabelText("Num. Players", SdfIconType.Person), SuffixLabel("Players"), BoxGroup("Game Settings"), ReadOnly] 
        private int numberOfPlayer = 4;
        [SerializeField, LabelText("Initial Money", SdfIconType.CurrencyDollar), SuffixLabel("USD", overlay:true), BoxGroup("Game Settings")] 
        private int initialMoney = 1000000;
        [SerializeField, LabelText("Bet", SdfIconType.CurrencyDollar), SuffixLabel("USD", overlay:true), BoxGroup("Game Settings")] 
        private int bet = 200000;
        
        [BoxGroup("Required")]
        [SerializeField, Required] private Player playerPrefab;
        [BoxGroup("Required")]
        [SerializeField, Required] private Deck deckTemplate;

        private List<Player> players = new();
        private Deck deck;
        
        [ShowInInspector, ReadOnly] private GameState gameState = new();
        public GameState GameState => gameState;
        
        public Player LocalPlayer { get; set; }

        public void Initialize()
        {
            InitiateDeck();
            InitiatePlayers();
            GiveCardsToPlayers();
            InitiateGame();
        }

        private void InitiatePlayers()
        {
            for (int i = 0; i < numberOfPlayer; i++)
            {
                var player = Instantiate(playerPrefab, transform);
                player.gameObject.name = $"Player {i + 1}";
                player.Initialize(this, initialMoney);
                players.Add(player);

                if (i == 0)
                {
                    LocalPlayer = player;
                }
            }
        }

        private void InitiateDeck()
        {
            deck = ScriptableObject.CreateInstance<Deck>();
            deck.cards = new List<Card>(deckTemplate.cards);
        }

        private void GiveCardsToPlayers()
        {
            var cardsPerPlayer = deck.cards.Count / numberOfPlayer;
            foreach (var player in players)
            {
                for (int i = 0; i < cardsPerPlayer; i++)
                {
                    var card = deck.GetRandomCard();
                    player.AddCard(card);
                }
            }
        }

        private void InitiateGame()
        {
            
        }

        public void DealCards(Player player, CardHand hand)
        {
            gameState.lastPlayerHand = hand;
            gameState.lastPlayerTurn = player;
            
            GameEvent.Trigger(Constants.EVENT_CARDS_DEALT);
        }
    }
}
