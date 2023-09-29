using TMPro;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class AIProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI cardsLeftText;
        
        public void Initialize(Player player)
        {
            player.money.TakeUntilDestroy(this).Subscribe(UpdateMoney);
            player.playerName.TakeUntilDestroy(this).Subscribe(UpdateName);
            player.hand.cards.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(UpdateCardsLeft);

            UpdateCardsLeft(player.hand.cards.Count);
        }

        private void UpdateMoney(int money)
        {
            moneyText.text = $"{money:n0}";
        }

        private void UpdateName(string name)
        {
            nameText.text = $"{name}";
        }

        private void UpdateCardsLeft(int count)
        {
            cardsLeftText.text = count.ToString();
        }
    }
}
