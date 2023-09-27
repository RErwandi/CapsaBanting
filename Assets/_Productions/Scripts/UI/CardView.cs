using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetCard(Card card)
        {
            image.sprite = card.sprite;
        }
    }
}
