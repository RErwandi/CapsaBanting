using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class Player : MonoBehaviour, IEventListener<GameEvent>, IEventListener<PLayerWinEvent>
    {
        public IntReactiveProperty money = new();
        public CardHand hand = new();
        public ReactiveCollection<int> selected = new();
        public BoolReactiveProperty canDealtAny = new();
        public ReactiveProperty<Sprite> face = new();
        public PlayerProfile profile;
        public StateMachine stateMachine;

        public BoolReactiveProperty hasPair = new();
        public BoolReactiveProperty hasThree = new();
        public BoolReactiveProperty hasStraight = new();
        public BoolReactiveProperty hasFlush = new();
        public BoolReactiveProperty hasFullHouse = new();
        public BoolReactiveProperty hasFours = new();
        public BoolReactiveProperty hasStraightFlush = new();
        public BoolReactiveProperty hasRoyalFlush = new();

        private List<List<int>> single = new ();
        private List<List<int>> pairs = new ();
        private List<List<int>> threes = new ();
        private List<List<int>> straight = new ();
        private List<List<int>> flush = new ();
        private List<List<int>> fullHouse = new ();
        private List<List<int>> fours = new ();
        private List<List<int>> straightFlush = new ();
        private List<List<int>> royalFlush = new ();

        private int iPlayer;
        private int iSelect = -1;
        private int iCategory = 0;
        private GameController controller;

        public CardHand SelectedHand => ConvertIndexToCardHand(selected.ToList());
        public int Index => iPlayer;

        private void OnEnable()
        {
            EventManager.AddListener<GameEvent>(this);
            EventManager.AddListener<PLayerWinEvent>(this);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<GameEvent>(this);
            EventManager.RemoveListener<PLayerWinEvent>(this);
        }

        public void Initialize(GameController controller, int money, int index, PlayerProfile profile)
        {
            this.controller = controller;
            this.money.Value = money;
            this.profile = profile;
            iPlayer = index;
            face.Value = profile.NormalFace;
            
            hand.cards.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => CheckHand());
            CheckHand();
            CheckCards();
        }

        public void AddMoney(int amount)
        {
            money.Value += amount;
        }

        public void SubtractMoney(int amount)
        {
            money.Value -= amount;
        }

        public void AddCard(Card card)
        {
            hand.AddCard(card);
        }

        public void RemoveCard(Card card)
        {
            hand.RemoveCard(card);
        }

        public void SelectCard(int index)
        {
            selected.Add(index);
        }
        
        public void UnSelectCard(int index)
        {
            selected.Remove(index);
        }
        
        public void ResetSelected()
        {
            selected.Clear();
        }

        public void SortCards()
        {
            var sortedCards = hand.cards.OrderByDescending(card => card.GetValue()).Reverse().ToList();
            hand.cards.Clear();
            foreach (var card in sortedCards)
            {
                hand.AddCard(card);
            }
        }
        
        private void SelectCards(List<int> indexes)
        {
            ResetSelected();
            foreach (var index in indexes)
            {
                selected.Add(index);
            }
        }
        
        private void CheckHand()
        {
            iSelect = -1;

            single = hand.HasSingle();
            
            pairs = hand.HasPair();
            hasPair.Value = pairs.Count > 0;
            
            threes = hand.HasThreeOfAKind();
            hasThree.Value = threes.Count > 0;
            
            straight = hand.HasStraight();
            hasStraight.Value = straight.Count > 0;
            
            flush = hand.HasFlush();
            hasFlush.Value = flush.Count > 0;
            
            fullHouse = hand.HasFullHouse();
            hasFullHouse.Value = fullHouse.Count > 0;
            
            fours = hand.HasFourOfAKind();
            hasFours.Value = fours.Count > 0;
            
            straightFlush = hand.HasStraightFlush();
            hasStraightFlush.Value = straightFlush.Count > 0;
            
            royalFlush = hand.HasRoyalFlush();
            hasRoyalFlush.Value = royalFlush.Count > 0;
        }
        
        public void SelectSingle()
        {
            ResetSelected();
            CheckCategory(0, hand.cards.Count);
            SelectCard(iSelect);
        }

        public void SelectPair()
        {
            if (!hasPair.Value) return;

            CheckCategory(1, pairs.Count);
            SelectCards(pairs[iSelect]);
        }

        public void SelectThrees()
        {
            if (!hasThree.Value) return;

            CheckCategory(2, threes.Count);
            SelectCards(threes[iSelect]);
        }

        public void SelectStraight()
        {
            if (!hasStraight.Value) return;

            CheckCategory(3, straight.Count);
            SelectCards(straight[iSelect]);
        }
        
        public void SelectFlush()
        {
            if (!hasFlush.Value) return;

            CheckCategory(4, flush.Count);
            SelectCards(flush[iSelect]);
        }
        
        public void SelectFullHouse()
        {
            if (!hasFullHouse.Value) return;

            CheckCategory(5, fullHouse.Count);
            SelectCards(fullHouse[iSelect]);
        }
        
        public void SelectFours()
        {
            if (!hasFours.Value) return;

            CheckCategory(6, fours.Count);
            SelectCards(fours[iSelect]);
        }
        
        public void SelectStraightFlush()
        {
            if (!hasStraightFlush.Value) return;

            CheckCategory(7, straightFlush.Count);
            SelectCards(straightFlush[iSelect]);
        }
        
        public void SelectRoyalFlush()
        {
            if (!hasRoyalFlush.Value) return;

            CheckCategory(8, royalFlush.Count);
            SelectCards(royalFlush[iSelect]);
        }

        private void NextSelect(int maxIndex)
        {
            iSelect++;
            if (iSelect >= maxIndex)
            {
                iSelect = 0;
            }
        }

        private void CheckCategory(int category, int maxIndex)
        {
            if (iCategory != category)
            {
                iCategory = category;
                iSelect = 0;
            }
            else
            {
                NextSelect(maxIndex);
            }
        }
        
        public void DealSelected()
        {
            DealCards(selected.ToList());
            selected.Clear();
        }
        
        private void DealCards(List<int> indexes)
        {
            var dealtHand = new CardHand();
            
            foreach (var index in indexes)
            {
                var card = hand.GetCardByIndex(index);
                dealtHand.AddCard(card);
            }

            foreach (var card in dealtHand.cards)
            {
                RemoveCard(card);
            }

            stateMachine.SetState(Constants.STATE_PLAYER_DEALING);
            Blackboard.Game.DealCards(iPlayer, dealtHand);
        }

        private CardHand ConvertIndexToCardHand(List<int> indexes)
        {
            var cardHand = new CardHand();
            
            foreach (var card in indexes.Select(i => hand.cards[i]))
            {
                cardHand.AddCard(card);
            }

            return cardHand;
        }

        private void CheckCards()
        {
            canDealtAny.Value = false;
            switch (controller.GameState.LastPlayerHand.CombinationType)
            {
                case CardCombinationType.Invalid:
                    canDealtAny.Value = true;
                    break;
                case CardCombinationType.Single:
                    canDealtAny.Value = true;
                    break;
                case CardCombinationType.Pair:
                    if (hasPair.Value) CheckHigher(pairs);
                    break;
                case CardCombinationType.Triple:
                    if (hasThree.Value) CheckHigher(threes);
                    break;
                case CardCombinationType.Flush:
                    if (hasFlush.Value) CheckHigher(flush);
                    break;
                case CardCombinationType.Straight:
                    if (hasStraight.Value) CheckHigher(straight);
                    break;
                case CardCombinationType.FullHouse:
                    if (hasFullHouse.Value) CheckHigher(fullHouse);
                    break;
                case CardCombinationType.FourOfAKind:
                    if (hasFours.Value) CheckHigher(fours);
                    break;
                case CardCombinationType.StraightFlush:
                    if (hasStraightFlush.Value) CheckHigher(straightFlush);
                    break;
                case CardCombinationType.RoyalFlush:
                    if (hasRoyalFlush.Value) CheckHigher(royalFlush);
                    break;
                default:
                    canDealtAny.Value = true;
                    break;
            }
        }

        private void CheckHigher(List<List<int>> sets)
        {
            foreach (var set in sets)
            {
                var setHand = new CardHand();
                foreach (var i in set)
                {
                    var card = hand.cards[i];
                    setHand.AddCard(card);
                }
                
                if (setHand.HighCard > controller.GameState.LastPlayerHand.HighCard)
                {
                    canDealtAny.Value = true;
                }
                else if (setHand.HighCard == controller.GameState.LastPlayerHand.HighCard)
                {
                    if (setHand.BestSuit > controller.GameState.LastPlayerHand.BestSuit)
                    {
                        canDealtAny.Value = true;
                    }
                }
            }
        }

        public void DealBest()
        {
            ResetSelected();
            var tableHand = controller.GameState.LastPlayerHand;
            if (!canDealtAny.Value)
            {
                Pass();
                return;
            }

            if (hasRoyalFlush.Value && (tableHand.IsRoyalFlush || tableHand.IsInvalid))
            {
                TryDealCards(royalFlush);
            }
            else if (hasStraightFlush.Value && (tableHand.IsStraightFlush || tableHand.IsInvalid))
            {
                TryDealCards(straightFlush);
            }
            else if (hasFours.Value && (tableHand.IsFourOfAKind || tableHand.IsInvalid))
            {
                TryDealCards(fours);
            }
            else if (hasFullHouse.Value && (tableHand.IsFullHouse || tableHand.IsInvalid))
            {
                TryDealCards(fullHouse);
            }
            else if (hasStraight.Value && (tableHand.IsStraight || tableHand.IsInvalid))
            {
                TryDealCards(straight);
            }
            else if (hasFlush.Value && (tableHand.IsFlush || tableHand.IsInvalid))
            {
                TryDealCards(flush);
            }
            else if (hasThree.Value && (tableHand.IsThreeOfKind || tableHand.IsInvalid))
            {
                TryDealCards(threes);
            }
            else if (hasPair.Value && (tableHand.IsPair || tableHand.IsInvalid))
            {
                TryDealCards(pairs);
            }
            else if (tableHand.IsSingle || tableHand.IsInvalid)
            {
                TryDealCards(single);
            }
        }

        private void TryDealCards(List<List<int>> attemptCards)
        {
            foreach (var attempt in attemptCards)
            {
                var attemptHand = ConvertIndexToCardHand(attempt);
                if (attemptHand.IsHigherThan(controller.GameState.LastPlayerHand))
                {
                    DealCards(attempt);
                    return;
                }
            }
            
            Pass();
        }

        public void Pass()
        {
            stateMachine.SetState(Constants.STATE_PLAYER_PASSING);
            Blackboard.Game.Pass(iPlayer);
        }

        public void Wait()
        {
            stateMachine.SetState(Constants.STATE_PLAYER_IDLE);
        }
        
        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_CARDS_DEALT || e.eventName == Constants.EVENT_TABLE_CLEAR)
            {
                CheckCards();
            }
        }
        
        public void OnEvent(PLayerWinEvent e)
        {
            if (e.indexPlayer == Index)
            {
                stateMachine.SetState(Constants.STATE_PLAYER_WINNING);
            }
            else
            {
                stateMachine.SetState(Constants.STATE_PLAYER_LOSING);
            }
        }

        #region State Machine Methods

        public void Idle()
        {
            face.Value = profile.NormalFace;
        }

        public void Dealing()
        {
            AudioManager.Instance.PlaySound(profile.VoiceDeal);
            face.Value = profile.HappyFace;
        }

        public void Passing()
        {
            AudioManager.Instance.PlaySound(profile.VoicePass);
            face.Value = profile.SadFace;
        }

        public void Winning()
        {
            AudioManager.Instance.PlaySound(profile.VoiceWin);
            face.Value = profile.HappyFace;
        }

        public void Losing()
        {
            AudioManager.Instance.PlaySound(profile.VoiceLose);
            face.Value = profile.SadFace;
        }

        #endregion
    }
}
