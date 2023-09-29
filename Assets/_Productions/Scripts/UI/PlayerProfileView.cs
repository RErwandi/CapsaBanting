using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class PlayerProfileView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private Image profileImage;
        
        private Player LocalPlayer => Blackboard.LocalPlayer;
        
        private void OnEnable()
        {
            EventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void Subscribe()
        {
            LocalPlayer.money.TakeUntilDestroy(this).Subscribe(OnMoneyChanged);
        }

        private void OnMoneyChanged(int value)
        {
            moneyText.text = $"${value:n0}";
        }
        
        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_INITIALIZED)
            {
                Subscribe();
            }
        }
    }
}
