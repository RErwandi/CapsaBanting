using System.Collections.Generic;
using UnityEngine;

namespace CapsaBanting
{
    [CreateAssetMenu(order = 0, fileName = "Deck", menuName = "Capsa/Deck")]
    public class Deck : ScriptableObject
    {
        public List<Card> cards;
        
        public Deck(List<Card> cards)
        {
            this.cards = cards;
        }

        public Card GetRandomCard()
        {
            var selectedCard = cards[Random.Range(0, cards.Count)];
            cards.Remove(selectedCard);
            return selectedCard;
        }
    }

}