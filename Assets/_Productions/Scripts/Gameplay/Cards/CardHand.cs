using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace CapsaBanting
{
    [Serializable]
    public class CardHand
    {
        public ReactiveCollection<Card> cards = new();

        public void AddCard(Card card)
        {
            cards.Add(card);
        }
        
        public void RemoveCard(Card card)
        {
            cards.Remove(card);
        }
        
        public List<List<Card>> HasPair()
        {
            List<List<Card>> pairs = new List<List<Card>>();
            Dictionary<CardFace, List<Card>> faceToCards = new Dictionary<CardFace, List<Card>>();

            // Group the cards by face value
            foreach (Card card in cards)
            {
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<Card>();
                }
                faceToCards[card.face].Add(card);
            }

            // Check for pairs (two or more cards with the same face)
            foreach (var kvp in faceToCards)
            {
                if (kvp.Value.Count >= 2)
                {
                    List<Card> pair = kvp.Value;

                    // Generate all unique combinations of pairs from the group
                    List<List<Card>> combinations = GenerateCombinations(pair, 2);

                    pairs.AddRange(combinations);
                }
            }

            return pairs;
        }
        
        private List<List<Card>> GenerateCombinations(List<Card> cards, int k)
        {
            List<List<Card>> result = new List<List<Card>>();
            List<Card> combination = new List<Card>();
            GenerateCombinationsHelper(cards, k, 0, combination, result);
            return result;
        }

        private void GenerateCombinationsHelper(List<Card> cards, int k, int start, List<Card> combination, List<List<Card>> result)
        {
            if (combination.Count == k)
            {
                result.Add(new List<Card>(combination));
                return;
            }

            for (int i = start; i < cards.Count; i++)
            {
                combination.Add(cards[i]);
                GenerateCombinationsHelper(cards, k, i + 1, combination, result);
                combination.RemoveAt(combination.Count - 1);
            }
        }
        
        public List<List<Card>> HasThreeOfAKind()
        {
            List<List<Card>> threeOfAKindList = new List<List<Card>>();
            Dictionary<CardFace, List<Card>> faceToCards = new Dictionary<CardFace, List<Card>>();

            // Group the cards by face value
            foreach (Card card in cards)
            {
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<Card>();
                }
                faceToCards[card.face].Add(card);
            }

            // Check for three-of-a-kind (three or more cards with the same face)
            foreach (var kvp in faceToCards)
            {
                if (kvp.Value.Count >= 3)
                {
                    List<Card> threeOfAKind = kvp.Value;

                    // Generate all unique combinations of three-of-a-kind from the group
                    List<List<Card>> combinations = GenerateCombinations(threeOfAKind, 3);

                    threeOfAKindList.AddRange(combinations);
                }
            }

            return threeOfAKindList;
        }

        public List<List<Card>> HasStraight()
        {
            List<List<Card>> straightList = new List<List<Card>>();
            List<Card> sortedCards = new List<Card>(cards);
            sortedCards.Sort((card1, card2) => card1.face.CompareTo(card2.face));

            int consecutiveCount = 1; // Number of consecutive cards

            for (int i = 1; i < sortedCards.Count; i++)
            {
                if (sortedCards[i].face == sortedCards[i - 1].face + 1)
                {
                    consecutiveCount++;

                    if (consecutiveCount >= 5)
                    {
                        // Create a new list for the straight
                        List<Card> straight = new List<Card>();
                        for (int j = i - 4; j <= i; j++)
                        {
                            straight.Add(sortedCards[j]);
                        }
                        straightList.Add(straight);
                    }
                }
                else if (sortedCards[i].face != sortedCards[i - 1].face + 1)
                {
                    consecutiveCount = 1;
                }
            }

            return straightList;
        }
        
        public List<List<Card>> HasFlush()
        {
            List<List<Card>> flushList = new List<List<Card>>();

            foreach (CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit)))
            {
                List<Card> cardsInSuit = new();
                foreach (var card in cards)
                {
                    if (card.suit == suit)
                    {
                        cardsInSuit.Add(card);
                    }
                }

                if (cardsInSuit.Count >= 5)
                {
                    // Create combinations of 5 cards from the same suit
                    List<List<Card>> combinations = GenerateCombinations(cardsInSuit, 5);

                    flushList.AddRange(combinations);
                }
            }

            return flushList;
        }
        
        public List<List<Card>> HasFullHouse()
        {
            List<List<Card>> fullHouseList = new List<List<Card>>();
            Dictionary<CardFace, List<Card>> faceToCards = new Dictionary<CardFace, List<Card>>();

            // Group the cards by face value
            foreach (Card card in cards)
            {
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<Card>();
                }
                faceToCards[card.face].Add(card);
            }

            // Check for three-of-a-kind and pairs
            foreach (var threeOfAKindKvp in faceToCards)
            {
                if (threeOfAKindKvp.Value.Count >= 3)
                {
                    foreach (var pairKvp in faceToCards)
                    {
                        if (pairKvp.Value.Count >= 2 && pairKvp.Key != threeOfAKindKvp.Key)
                        {
                            List<Card> fullHouse = new List<Card>(threeOfAKindKvp.Value.Take(3));
                            fullHouse.AddRange(pairKvp.Value.Take(2));

                            if (fullHouse.Count == 5) // Ensure exactly 5 cards in the combination
                            {
                                fullHouseList.Add(fullHouse);
                            }
                        }
                    }
                }
            }

            return fullHouseList;
        }
        
        public List<List<Card>> HasFourOfAKind()
        {
            List<List<Card>> fourOfAKindList = new List<List<Card>>();
            Dictionary<CardFace, List<Card>> faceToCards = new Dictionary<CardFace, List<Card>>();

            // Group the cards by face value
            foreach (Card card in cards)
            {
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<Card>();
                }
                faceToCards[card.face].Add(card);
            }

            // Check for four-of-a-kind (four or more cards with the same face)
            foreach (var kvp in faceToCards)
            {
                if (kvp.Value.Count >= 4)
                {
                    // Create a list for the four-of-a-kind
                    List<Card> fourOfAKind = new List<Card>(kvp.Value);

                    // Find any card that doesn't belong to the four-of-a-kind
                    foreach (var otherKvp in faceToCards)
                    {
                        if (otherKvp.Key != kvp.Key)
                        {
                            foreach (Card card in otherKvp.Value)
                            {
                                List<Card> fourOfAKindWithFifth = new List<Card>(fourOfAKind);
                                fourOfAKindWithFifth.Add(card);
                                fourOfAKindList.Add(fourOfAKindWithFifth);
                            }
                            break; // Add only one card from another face value
                        }
                    }
                }
            }

            return fourOfAKindList;
        }
        
        public List<List<Card>> HasStraightFlush()
        {
            List<List<Card>> straightFlushList = new List<List<Card>>();

            // First, check for flushes
            List<List<Card>> flushes = HasFlush();

            foreach (var flush in flushes)
            {
                List<Card> sortedFlush = new List<Card>(flush);
                sortedFlush.Sort((card1, card2) => card1.face.CompareTo(card2.face));

                int consecutiveCount = 1; // Number of consecutive cards

                for (int i = 1; i < sortedFlush.Count; i++)
                {
                    if (sortedFlush[i].face == sortedFlush[i - 1].face + 1)
                    {
                        consecutiveCount++;

                        if (consecutiveCount >= 5)
                        {
                            // Create a new list for the straight flush
                            List<Card> straightFlush = new List<Card>();
                            for (int j = i - 4; j <= i; j++)
                            {
                                straightFlush.Add(sortedFlush[j]);
                            }
                            straightFlushList.Add(straightFlush);
                        }
                    }
                    else if (sortedFlush[i].face != sortedFlush[i - 1].face + 1)
                    {
                        consecutiveCount = 1;
                    }
                }
            }

            return straightFlushList;
        }
        
        public List<List<Card>> HasRoyalFlush()
        {
            List<List<Card>> royalFlushList = new List<List<Card>>();
    
            // First, check for flushes
            List<List<Card>> flushes = HasFlush();
    
            foreach (var flush in flushes)
            {
                // Sort the flush cards by face value
                flush.Sort((card1, card2) => card1.face.CompareTo(card2.face));
        
                // Check if the flush contains 10, Jack, Queen, King, Ace
                if (flush.Any(card => card.face == CardFace.Ten) &&
                    flush.Any(card => card.face == CardFace.Jack) &&
                    flush.Any(card => card.face == CardFace.Queen) &&
                    flush.Any(card => card.face == CardFace.King) &&
                    flush.Any(card => card.face == CardFace.Ace))
                {
                    royalFlushList.Add(flush);
                }
            }

            return royalFlushList;
        }
    }
}
