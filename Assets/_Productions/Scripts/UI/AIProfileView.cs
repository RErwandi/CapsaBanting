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
        
        public void Initialize(Player player)
        {
            player.money.TakeUntilDestroy(this).Subscribe(UpdateMoney);
            player.hand.cards.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(UpdateCardsLeft);

            UpdateCardsLeft(player.hand.cards.Count);
            profileImage.sprite = player.Profile.NormalFace;
            nameText.text = player.Profile.PlayerName;
        }

        private void UpdateMoney(int money)
        {
            moneyText.text = $"{money:n0}";
        }

        private void UpdateCardsLeft(int count)
        {
            cardsLeftText.text = count.ToString();
        }
    }
}
