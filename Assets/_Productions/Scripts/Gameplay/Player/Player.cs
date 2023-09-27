using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    public class Player : MonoBehaviour
    {
        public CardHand hand;
        
        [ShowInInspector, ReadOnly] private List<List<Card>> pairs = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> threes = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> straight = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> flush = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> fullHouse = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> fours = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> straightFlush = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> royalFlush = new ();

        public void AddCard(Card card)
        {
            hand.AddCard(card);
            CheckHand();
        }

        public void RemoveCard(Card card)
        {
            hand.RemoveCard(card);
            CheckHand();
        }

        [Button, HideInEditorMode]
        private void CheckHand()
        {
            pairs = hand.HasPair();
            threes = hand.HasThreeOfAKind();
            straight = hand.HasStraight();
            flush = hand.HasFlush();
            fullHouse = hand.HasFullHouse();
            fours = hand.HasFourOfAKind();
            straightFlush = hand.HasStraightFlush();
            royalFlush = hand.HasRoyalFlush();
        }
    }
}
