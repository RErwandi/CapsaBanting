using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
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
        private int allMoney = 0;
        
        private IntReactiveProperty iTurn = new();
        public IntReactiveProperty ITurn => iTurn;
        
        [ShowInInspector, ReadOnly] private GameState gameState = new();
        public GameState GameState => gameState;

        public List<PlayerProfile> Profiles => profiles;
        public int NumberOfPlayers => numberOfPlayer;
        public StateMachine StateMachine => stateMachine;
        public int Bet => bet;

        public void Initialize()
        {
            InitiateDeck();
            InitiatePlayers();
            GiveCardsToPlayers();
            SortAICards();
            InitiateGame();
        }
        
        public void RestartGame()
        {
            InitiateDeck();
            ResetPlayersCard();
            GiveCardsToPlayers();
            SortAICards();
            CheckClear();
            iTurn.Value--;
            initialMoney = 0;
            NextTurn();
        }
        
        private void InitiateDeck()
        {
            deck = ScriptableObject.CreateInstance<Deck>();
            deck.cards = new List<Card>(deckTemplate.cards);
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
                    Blackboard.AIPlayers.Add(player);
                }
            }
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

        private void SortAICards()
        {
            foreach (var aiPlayer in Blackboard.AIPlayers)
            {
                aiPlayer.SortCards();
            }
        }

        private void ResetPlayersCard()
        {
            foreach (var player in players)
            {
                player.hand.Clear();
            }
        }

        private void InitiateGame()
        {
            iTurn.Value = 0;
            CheckTurn();
        }

        private void NextTurn()
        {
            iTurn.Value++;
            if (iTurn.Value >= players.Count)
            {
                iTurn.Value = 0;
            }
            
            CheckTurn();
            CheckClear();
            ResetPlayerState();

            if (iTurn.Value > 0)
            {
                players[iTurn.Value].DealBest();
            }
        }

        private void ResetPlayerState()
        {
            foreach (var player in players)
            {
                player.Wait();
            }
        }
        
        private void CheckTurn()
        {
            stateMachine.SetState(iTurn.Value == 0 ? "Player Turn" : "Enemy Turn");
        }

        private bool CheckWin()
        {
            if (players[iTurn.Value].hand.IsEmpty)
            {
                Debug.Log($"Player {iTurn} won!");
                PLayerWinEvent.Trigger(iTurn.Value);
                return true;
            }

            return false;
        }

        private void CheckClear()
        {
            if (iTurn.Value == gameState.lastPlayerTurn)
            {
                gameState.Clear();
                GameEvent.Trigger(Constants.EVENT_TABLE_CLEAR);
            }
        }

        public void DealCards(int playerIndex, CardHand hand)
        {
            StartCoroutine(DealCardsCoroutine(playerIndex, hand));
        }

        private IEnumerator DealCardsCoroutine(int playerIndex, CardHand hand)
        {
            var msg = $"Player {playerIndex + 1} deal ";
            msg = hand.cards.Aggregate(msg, (current, card) => current + $"{card.face}_{card.suit} ");

            Debug.Log(msg);
            gameState.lastPlayerTurn = playerIndex;
            gameState.lastPlayerHands.Add(hand);

            GameEvent.Trigger(Constants.EVENT_CARDS_DEALT);
            yield return new WaitForSeconds(1f);

            if (CheckWin())
            {
                stateMachine.SetState("Game Ended");
                EvaluateBet();
            }
            else
            {
                NextTurn();
            }
        }

        private void EvaluateBet()
        {
            foreach (var player in players.Where(player => player.Index != iTurn.Value))
            {
                SubtractLosers(player);
            }

            foreach (var player in players.Where(player => player.Index == iTurn.Value))
            {
                AddWinner(player);
            }
        }

        private void SubtractLosers(Player loser)
        {
            var remainingCards = loser.hand.cards.Count;
            var loseMoney = remainingCards * bet;
            loser.SubtractMoney(loseMoney);
            allMoney += loseMoney;
        }

        private void AddWinner(Player winner)
        {
            winner.AddMoney(allMoney);
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
