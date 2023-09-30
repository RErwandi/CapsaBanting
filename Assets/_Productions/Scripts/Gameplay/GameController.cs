using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField, BoxGroup("Game Settings")] private List<PlayerProfile> profiles;
        
        [BoxGroup("Required")]
        [SerializeField, Required] private Player playerPrefab;
        [BoxGroup("Required")]
        [SerializeField, Required] private Deck deckTemplate;
        [BoxGroup("Required")]
        [SerializeField, Required] private StateMachine stateMachine;

        private List<Player> players = new();
        private Deck deck;
        private int iTurn;
        
        [ShowInInspector, ReadOnly] private GameState gameState = new();
        public GameState GameState => gameState;

        public StateMachine StateMachine => stateMachine;

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
                player.Initialize(this, initialMoney, i, profiles[i]);
                players.Add(player);

                if (i == 0)
                {
                    Blackboard.LocalPlayer = player;
                }
                else
                {
                    Blackboard.AI.AIPlayers.Add(player);
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
            iTurn = 0;
            CheckTurn();
        }

        private void CheckTurn()
        {
            stateMachine.SetState(iTurn == 0 ? "Player Turn" : "Enemy Turn");
        }
        
        private void NextTurn()
        {
            iTurn++;
            if (iTurn >= players.Count)
            {
                iTurn = 0;
            }
            
            CheckTurn();
            CheckClear();

            if (iTurn > 0)
            {
                players[iTurn].DealBest();
            }
        }

        private void CheckClear()
        {
            if (iTurn == gameState.lastPlayerTurn)
            {
                Debug.Log($"Clear table");
                gameState.Clear();
                GameEvent.Trigger(Constants.EVENT_CARDS_DEALT);
            }
        }

        public void DealCards(int playerIndex, CardHand hand)
        {
            StartCoroutine(DealCardsCoroutine(playerIndex, hand));
        }

        private IEnumerator DealCardsCoroutine(int playerIndex, CardHand hand)
        {
            var msg = $"Player {playerIndex + 1} deal ";
            msg = hand.cards.Aggregate(msg, (current, card) => current + $"{card.face}_{card.suit}");

            Debug.Log(msg);
            gameState.lastPlayerTurn = playerIndex;
            gameState.lastPlayerHands.Add(hand);

            GameEvent.Trigger(Constants.EVENT_CARDS_DEALT);
            yield return new WaitForSeconds(1f);
            NextTurn();
        }

        public void Pass(int playerIndex)
        {
            StartCoroutine(PassCoroutine(playerIndex));
        }

        private IEnumerator PassCoroutine(int playerIndex)
        {
            Debug.Log($"Player {playerIndex + 1} passed.");
            yield return new WaitForSeconds(1f);
            NextTurn();
        }
    }
}
