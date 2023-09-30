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
            submitButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => SubmitClicked());
            passButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => PassClicked());
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void Hide()
        {
            passButton.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
        }

        private void Show()
        {
            passButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(true);
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
            var table = Blackboard.Game.GameState.LastPlayerHand;

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
                Hide();
            }
            
            if (state.StateName == "Player Turn")
            {
                Show();
            }
        }

        private void SubmitClicked()
        {
            Hide();
            LocalPlayer.DealSelected();
        }

        private void PassClicked()
        {
            Hide();
            Blackboard.Game.Pass(0);
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
