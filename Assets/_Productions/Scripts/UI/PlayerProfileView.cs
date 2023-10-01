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
            LocalPlayer.face.TakeUntilDestroy(this).Subscribe(UpdateFace);
            nameText.text = LocalPlayer.profile.PlayerName;
            profileImage.sprite = LocalPlayer.profile.NormalFace;
        }

        private void OnMoneyChanged(int value)
        {
            moneyText.text = $"${value:n0}";
        }
        
        private void UpdateFace(Sprite sprite)
        {
            profileImage.sprite = sprite;
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
