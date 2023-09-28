using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class Player : MonoBehaviour
    {
        public CardHand hand;
        public ReactiveCollection<int> selected;

        [ShowInInspector, ReadOnly] private List<List<int>> pairs = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> threes = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> straight = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> flush = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> fullHouse = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> fours = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> straightFlush = new ();
        [ShowInInspector, ReadOnly] private List<List<int>> royalFlush = new ();

        public ReactiveProperty<bool> hasPair;
        public ReactiveProperty<bool> hasThree;
        public ReactiveProperty<bool> hasStraight;
        public ReactiveProperty<bool> hasFlush;
        public ReactiveProperty<bool> hasFullHouse;
        public ReactiveProperty<bool> hasFours;
        public ReactiveProperty<bool> hasStraightFlush;
        public ReactiveProperty<bool> hasRoyalFlush;

        private int iSelect = -1;
        private int iCategory = 0;
        
        private void Start()
        {
            hand.cards.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => CheckHand());
            CheckHand();
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

        [Button, HideInEditorMode]
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
    }
}
