using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class AIProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI cardsLeftText;
        [SerializeField] private Image profileImage;
        [SerializeField] private Image background;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color turnColor;

        private Player player;
        
        public void Initialize(Player player)
        {
            this.player = player;
            player.money.TakeUntilDestroy(this).Subscribe(UpdateMoney);
            player.hand.cards.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(UpdateCardsLeft);
            player.face.TakeUntilDestroy(this).Subscribe(UpdateFace);
            Blackboard.Game.ITurn.TakeUntilDestroy(this).Subscribe(UpdateBackground);

            UpdateCardsLeft(player.hand.cards.Count);
            profileImage.sprite = player.profile.NormalFace;
            nameText.text = player.profile.PlayerName;
        }

        private void UpdateMoney(int money)
        {
            moneyText.text = $"${money:n0}";
        }

        private void UpdateCardsLeft(int count)
        {
            cardsLeftText.text = count.ToString();
        }

        private void UpdateBackground(int index)
        {
            background.color = index == player.Index ? turnColor : defaultColor;
        }

        private void UpdateFace(Sprite sprite)
        {
            profileImage.sprite = sprite;
        }
    }
}
