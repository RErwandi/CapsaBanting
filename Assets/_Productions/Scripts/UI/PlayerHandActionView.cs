using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class PlayerHandActionView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private Button passButton;
        [SerializeField] private Button submitButton;
        
        private Player LocalPlayer => Blackboard.Controller.LocalPlayer;

        private void OnEnable()
        {
            EventManager.AddListener(this);
            submitButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => LocalPlayer.DealSelected());
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void Subscribe()
        {
            LocalPlayer.selected.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => OnSelectedChanged());
        }

        private void OnSelectedChanged()
        {
            var hand = LocalPlayer.GetSelectedCardHand();
            var table = Blackboard.Controller.GameState.lastPlayerHand;

            if (table.CombinationType == CardCombinationType.Invalid)
            {
                submitButton.interactable = true;
            }
            else
            {
                submitButton.interactable = hand.CombinationType == table.CombinationType && hand.HighCard > table.HighCard;
            }
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
