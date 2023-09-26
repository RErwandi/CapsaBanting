using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    public class Player : MonoBehaviour
    {
        public Deck deck;
        public CardHand playerHand;
        
        [ShowInInspector, ReadOnly] private List<List<Card>> pairs = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> threes = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> straight = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> flush = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> fullHouse = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> fours = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> straightFlush = new ();
        [ShowInInspector, ReadOnly] private List<List<Card>> royalFlush = new ();
        
        private Deck currentDeck;
        
        [Button, HideInEditorMode]
        public void GiveCardsFromDeck(int amount)
        {
            var tmpCards = new List<Card>(deck.cards);
            playerHand.cards.Clear();
            for (int i = 0; i < amount; i++)
            {
                var selectedCard = tmpCards[Random.Range(0, tmpCards.Count)];
                playerHand.AddCard(selectedCard);
                tmpCards.Remove(selectedCard);
            }
            
            CheckHand();
        }

        [Button, HideInEditorMode]
        private void CheckHand()
        {
            pairs = playerHand.HasPair();
            threes = playerHand.HasThreeOfAKind();
            straight = playerHand.HasStraight();
            flush = playerHand.HasFlush();
            fullHouse = playerHand.HasFullHouse();
            fours = playerHand.HasFourOfAKind();
            straightFlush = playerHand.HasStraightFlush();
            royalFlush = playerHand.HasRoyalFlush();
        }
    }
}
