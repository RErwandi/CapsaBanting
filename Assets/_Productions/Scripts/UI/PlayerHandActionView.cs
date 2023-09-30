using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class PlayerHandActionView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private Button passButton;
        [SerializeField] private GameObject passIndicator;
        [SerializeField] private Button submitButton;
        
        private Player LocalPlayer => Blackboard.LocalPlayer;

        private void OnEnable()
        {
            EventManager.AddListener(this);
            submitButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => LocalPlayer.DealSelected());
            passButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => Blackboard.Game.Pass(0));
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void Subscribe()
        {
            LocalPlayer.selected.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => OnSelectedChanged());
            LocalPlayer.canDealtAny.TakeUntilDestroy(this).Subscribe(OnCanDealtChanged);
            Blackboard.Game.StateMachine.currentState.TakeUntilDestroy(this).Subscribe(OnStateChanged);
            
            OnSelectedChanged();
        }

        private void OnSelectedChanged()
        {
            var hand = LocalPlayer.selectedHand;
            var table = Blackboard.Game.GameState.lastPlayerHand;

            if (hand.CombinationType == CardCombinationType.Invalid)
            {
                submitButton.interactable = false;
            }
            else if (table.CombinationType == CardCombinationType.Invalid && hand.CombinationType != CardCombinationType.Invalid)
            {
                submitButton.interactable = true;
            }
            else
            {
                submitButton.interactable = hand.CombinationType == table.CombinationType && hand.HighCard > table.HighCard;
            }
        }

        private void OnCanDealtChanged(bool value)
        {
            passIndicator.SetActive(!value);
        }

        private void OnStateChanged(State state)
        {
            if (state.StateName == "Enemy Turn")
            {
                passButton.gameObject.SetActive(false);
                submitButton.gameObject.SetActive(false);
            }
            
            if (state.StateName == "Player Turn")
            {
                passButton.gameObject.SetActive(true);
                submitButton.gameObject.SetActive(true);
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
