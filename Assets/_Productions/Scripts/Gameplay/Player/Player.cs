using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class Player : MonoBehaviour
    {
        public IntReactiveProperty money = new();
        public CardHand hand = new();
        public ReactiveCollection<int> selected = new();

        public BoolReactiveProperty hasPair = new();
        public BoolReactiveProperty hasThree = new();
        public BoolReactiveProperty hasStraight = new();
        public BoolReactiveProperty hasFlush = new();
        public BoolReactiveProperty hasFullHouse = new();
        public BoolReactiveProperty hasFours = new();
        public BoolReactiveProperty hasStraightFlush = new();
        public BoolReactiveProperty hasRoyalFlush = new();

        private List<List<int>> pairs = new ();
        private List<List<int>> threes = new ();
        private List<List<int>> straight = new ();
        private List<List<int>> flush = new ();
        private List<List<int>> fullHouse = new ();
        private List<List<int>> fours = new ();
        private List<List<int>> straightFlush = new ();
        private List<List<int>> royalFlush = new ();

        private int iSelect = -1;
        private int iCategory = 0;
        private GameController controller;
        
        public void Initialize(GameController controller, int money)
        {
            this.controller = controller;
            this.money.Value = money;
            
            hand.cards.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => CheckHand());
            CheckHand();
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
            selected.Clear();
            foreach (var index in indexes)
            {
                selected.Add(index);
            }
        }
        
        private void CheckHand()
        {
            iSelect = -1;
            
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

        [Button]
        private void DealSelected()
        {
            DealCards(selected.ToList());
        }

        [Button]
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

            Blackboard.Controller.DealCards(this, dealtHand);
        }
    }
}
