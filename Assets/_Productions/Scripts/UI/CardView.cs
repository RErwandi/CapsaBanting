using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;

        private RectTransform rect;
        private bool isSelected;
        private Player player;
        private int index;
        private bool isInitialized;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            button.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OnCardClicked());
            isSelected = false;
        }

        private void OnCardClicked()
        {
            if (!isSelected)
            {
                player.SelectCard(index);
            }
            else
            {
                player.UnSelectCard(index);
            }
        }

        public void SetCard(Card card, int index, Player player)
        {
            image.sprite = card.sprite;
            this.player = player;
            this.index = index;

            if (!isInitialized)
            {
                player.selected.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => CheckSelected());
                isInitialized = true;
            }
        }

        private void CheckSelected()
        {
            if (player.selected.Contains(index))
            {
                rect.DOAnchorPosY(100f, 0.2f).SetEase(Ease.Linear);
                isSelected = true;
            }
            else
            {
                rect.DOAnchorPosY(0f, 0.2f).SetEase(Ease.Linear);
                isSelected = false;
            }
        }
    }
}
