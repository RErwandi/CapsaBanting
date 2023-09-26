using System.Collections.Generic;
using UnityEngine;

namespace CapsaBanting
{
    [CreateAssetMenu(order = 0, fileName = "Deck", menuName = "Capsa/Deck")]
    public class Deck : ScriptableObject
    {
        public List<Card> cards = new();
    }

}