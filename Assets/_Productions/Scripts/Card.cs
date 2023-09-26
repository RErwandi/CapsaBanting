using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    [CreateAssetMenu(order = 0, fileName = "Card", menuName = "Capsa/Card")]
    public class Card : ScriptableObject
    {
        [BoxGroup("Value")]
        public CardFace face;
        [BoxGroup("Value")]
        public CardSuit suit;
        
        [PreviewField(250, ObjectFieldAlignment.Center), HideLabel]
        public Sprite sprite;
    }
}
