using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;

namespace CapsaBanting
{
    [Serializable]
    public class CardHand
    {
        public ReactiveCollection<Card> cards = new();
        [ShowInInspector]
        public CardCombinationType CombinationType
        {
            get
            {
                if (IsRoyalFlush)
                    return CardCombinationType.RoyalFlush;

                if (IsStraightFlush)
                    return CardCombinationType.StraightFlush;

                if (IsFourOfAKind)
                    return CardCombinationType.FourOfAKind;

                if (IsFullHouse)
                    return CardCombinationType.FullHouse;

                if (IsFlush)
                    return CardCombinationType.Flush;

                if (IsStraight)
                    return CardCombinationType.Straight;

                if (IsThreeOfKind)
                    return CardCombinationType.Triple;

                if (IsPair)
                    return CardCombinationType.Pair;

                if (IsSingle)
                    return CardCombinationType.Single;

                return CardCombinationType.Invalid;
            }
        }

        public int HighCard
        {
            get
            {
                int highCard = 0;
                foreach (var card in cards)
                {
                    if (card.GetValue() > highCard)
                    {
                        highCard = card.GetValue();
                    }
                }

                return highCard;
            }
        }
        
        public int BestSuit
        {
            get
            {
                int highCard = 0;
                foreach (var card in cards)
                {
                    if (card.GetSuitValue() > highCard)
                    {
                        highCard = card.GetSuitValue();
                    }
                }

                return highCard;
            }
        }

        public bool IsSingle => cards.Count == 1;
        public bool IsPair => HasPair().Count >= 1 && cards.Count == 2;
        public bool IsThreeOfKind => HasThreeOfAKind().Count >= 1 && cards.Count == 3;
        public bool IsStraight => HasStraight().Count >= 1 && cards.Count == 5;
        public bool IsFlush => HasFlush().Count >= 1 && cards.Count == 5;
        public bool IsFullHouse => HasFullHouse().Count >= 1 && cards.Count == 5;
        public bool IsFourOfAKind => HasFourOfAKind().Count >= 1 && cards.Count == 5;
        public bool IsStraightFlush => HasStraightFlush().Count >= 1 && cards.Count == 5;
        public bool IsRoyalFlush => HasRoyalFlush().Count >= 1 && cards.Count == 5;
        public bool IsInvalid => CombinationType == CardCombinationType.Invalid;
        public bool IsEmpty => cards.Count == 0;

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void Clear()
        {
            cards.Clear();
        }
        
        public void RemoveCard(Card card)
        {
            cards.Remove(card);
        }

        public Card GetCardByIndex(int index)
        {
            return cards[index];
        }

        public List<List<int>> HasSingle()
        {
            List<List<int>> singleList = new List<List<int>>();
            for (var i = 0; i < cards.Count; i++)
            {
                var single = new List<int>();
                single.Add(i);

                singleList.Add(single);
            }

            return singleList;
        }

        public List<List<int>> HasPair()
        {
            List<List<int>> pairList = new List<List<int>>();
            Dictionary<CardFace, List<int>> faceToCards = new Dictionary<CardFace, List<int>>();

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<int>();
                }
                faceToCards[card.face].Add(i);
            }

            foreach (var kvp in faceToCards)
            {
                if (kvp.Value.Count >= 2)
                {
                    // Generate all possible combinations of pairs
                    List<int> cardIndexes = kvp.Value;
                    for (int i = 0; i < cardIndexes.Count - 1; i++)
                    {
                        for (int j = i + 1; j < cardIndexes.Count; j++)
                        {
                            List<int> pair = new List<int> { cardIndexes[i], cardIndexes[j] };
                            pairList.Add(pair);
                        }
                    }
                }
            }

            return pairList;
        }
        
        public List<List<int>> HasThreeOfAKind()
        {
            List<List<int>> threeOfAKindList = new List<List<int>>();
            Dictionary<CardFace, List<int>> faceToCards = new Dictionary<CardFace, List<int>>();

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<int>();
                }
                faceToCards[card.face].Add(i);
            }

            foreach (var kvp in faceToCards)
            {
                if (kvp.Value.Count >= 3)
                {
                    // Generate all possible combinations of three-of-a-kind
                    List<int> cardIndexes = kvp.Value;
                    for (int i = 0; i < cardIndexes.Count - 2; i++)
                    {
                        for (int j = i + 1; j < cardIndexes.Count - 1; j++)
                        {
                            for (int k = j + 1; k < cardIndexes.Count; k++)
                            {
                                List<int> threeOfAKind = new List<int> { cardIndexes[i], cardIndexes[j], cardIndexes[k] };
                                threeOfAKindList.Add(threeOfAKind);
                            }
                        }
                    }
                }
            }

            return threeOfAKindList;
        }

        public List<List<int>> HasStraight()
        {
            List<List<int>> straightList = new List<List<int>>();
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
                        List<int> straight = new List<int>();
                        for (int j = i - 4; j <= i; j++)
                        {
                            straight.Add(cards.IndexOf(sortedCards[j]));
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
        
        public List<List<int>> HasFlush()
        {
            List<List<int>> flushList = new List<List<int>>();

            foreach (CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit)))
            {
                List<int> cardsInSuitIndexes = new List<int>();

                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].suit == suit)
                    {
                        cardsInSuitIndexes.Add(i);
                    }
                }

                if (cardsInSuitIndexes.Count >= 5)
                {
                    // Generate all possible combinations of flushes
                    List<List<int>> combinations = GenerateCombinations(cardsInSuitIndexes, 5);

                    flushList.AddRange(combinations);
                }
            }

            return flushList;
        }
        
        public List<List<int>> HasFullHouse()
        {
            List<List<int>> fullHouseList = new List<List<int>>();
            Dictionary<CardFace, List<int>> faceToCards = new Dictionary<CardFace, List<int>>();

            // Group the cards by face value
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<int>();
                }
                faceToCards[card.face].Add(i);
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
                            // Generate all possible combinations of full houses
                            List<List<int>> threeOfAKindCombinations = GenerateCombinations(threeOfAKindKvp.Value, 3);
                            List<List<int>> pairCombinations = GenerateCombinations(pairKvp.Value, 2);

                            foreach (var threeOfAKindCombo in threeOfAKindCombinations)
                            {
                                foreach (var pairCombo in pairCombinations)
                                {
                                    List<int> fullHouseCombo = new List<int>(threeOfAKindCombo);
                                    fullHouseCombo.AddRange(pairCombo);
                                    fullHouseList.Add(fullHouseCombo);
                                }
                            }
                        }
                    }
                }
            }

            return fullHouseList;
        }
        
        public List<List<int>> HasFourOfAKind()
        {
            List<List<int>> fourOfAKindList = new List<List<int>>();
            Dictionary<CardFace, List<int>> faceToCards = new Dictionary<CardFace, List<int>>();

            // Group the cards by face value
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (!faceToCards.ContainsKey(card.face))
                {
                    faceToCards[card.face] = new List<int>();
                }
                faceToCards[card.face].Add(i);
            }

            // Check for four-of-a-kind (four or more cards with the same face)
            foreach (var kvp in faceToCards)
            {
                if (kvp.Value.Count >= 4)
                {
                    // Generate all possible combinations of four-of-a-kind
                    List<List<int>> combinations = GenerateCombinations(kvp.Value, 4);

                    foreach (var combination in combinations)
                    {
                        // Find any card that doesn't belong to the four-of-a-kind
                        foreach (var otherKvp in faceToCards)
                        {
                            if (otherKvp.Key != kvp.Key)
                            {
                                foreach (int cardIndex in otherKvp.Value)
                                {
                                    List<int> fourOfAKindCombo = new List<int>(combination);
                                    fourOfAKindCombo.Add(cardIndex);
                                    fourOfAKindList.Add(fourOfAKindCombo);
                                }
                                break; // Add only one card from another face value
                            }
                        }
                    }
                }
            }

            return fourOfAKindList;
        }
        
        public List<List<int>> HasStraightFlush()
        {
            List<List<int>> straightFlushList = new List<List<int>>();

            // First, check for flushes
            List<List<int>> flushes = HasFlush();

            foreach (var flush in flushes)
            {
                List<int> sortedFlush = new List<int>(flush);
                sortedFlush.Sort((index1, index2) => cards[index1].face.CompareTo(cards[index2].face));

                int consecutiveCount = 1; // Number of consecutive cards

                for (int i = 1; i < sortedFlush.Count; i++)
                {
                    if (cards[sortedFlush[i]].face == cards[sortedFlush[i - 1]].face + 1)
                    {
                        consecutiveCount++;

                        if (consecutiveCount >= 5)
                        {
                            // Create a new list for the straight flush
                            List<int> straightFlush = new List<int>();
                            for (int j = i - 4; j <= i; j++)
                            {
                                straightFlush.Add(sortedFlush[j]);
                            }
                            straightFlushList.Add(straightFlush);
                        }
                    }
                    else if (cards[sortedFlush[i]].face != cards[sortedFlush[i - 1]].face + 1)
                    {
                        consecutiveCount = 1;
                    }
                }
            }

            return straightFlushList;
        }
        
        public List<List<int>> HasRoyalFlush()
        {
            List<List<int>> royalFlushList = new List<List<int>>();

            // First, check for flushes
            List<List<int>> flushes = HasFlush();

            foreach (var flush in flushes)
            {
                // Sort the flush cards by face value
                List<int> sortedFlush = new List<int>(flush);
                sortedFlush.Sort((index1, index2) => cards[index1].face.CompareTo(cards[index2].face));

                // Check if the flush contains 10, Jack, Queen, King, Ace
                bool hasTen = false, hasJack = false, hasQueen = false, hasKing = false, hasAce = false;

                foreach (int index in sortedFlush)
                {
                    Card card = cards[index];
                    if (card.face == CardFace.Ten)
                        hasTen = true;
                    else if (card.face == CardFace.Jack)
                        hasJack = true;
                    else if (card.face == CardFace.Queen)
                        hasQueen = true;
                    else if (card.face == CardFace.King)
                        hasKing = true;
                    else if (card.face == CardFace.Ace)
                        hasAce = true;
                }

                if (hasTen && hasJack && hasQueen && hasKing && hasAce)
                {
                    royalFlushList.Add(sortedFlush);
                }
            }

            return royalFlushList;
        }
        
        private static List<List<int>> GenerateCombinations(List<int> elements, int k)
        {
            List<List<int>> combinations = new List<List<int>>();
            GenerateCombinationsHelper(elements, k, 0, new List<int>(), combinations);
            return combinations;
        }

        private static void GenerateCombinationsHelper(List<int> elements, int k, int start, List<int> currentCombination, List<List<int>> combinations)
        {
            if (k == 0)
            {
                combinations.Add(new List<int>(currentCombination));
                return;
            }

            for (int i = start; i < elements.Count; i++)
            {
                currentCombination.Add(elements[i]);
                GenerateCombinationsHelper(elements, k - 1, i + 1, currentCombination, combinations);
                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }
    }
}
