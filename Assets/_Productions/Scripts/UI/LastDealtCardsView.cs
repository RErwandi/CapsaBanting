using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class LastDealtCardsView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private CardView cardView;
        [SerializeField] private Transform cardsContainer;
        
        private List<CardView> views = new();

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
            Blackboard.Game.GameState.lastPlayerHands.ObserveCountChanged().TakeUntilDestroy(this).Subscribe(_ => OnCardsChanged());
        }

        private void OnCardsChanged()
        {
            Debug.Log($"Cards changed");
            ResetCards();
            SetUpCards();
        }

        private void SetUpCards()
        {
            for (var i = 0; i < Blackboard.Game.GameState.LastPlayerHand.cards.Count; i++)
            {
                var card = Blackboard.Game.GameState.LastPlayerHand.cards[i];
                var view = Pool.Spawn(cardView, cardsContainer);
                view.SetCard(card, i);
                views.Add(view);
            }
        }

        private void ResetCards()
        {
            foreach (var view in views)
            {
                Pool.Despawn(view);
            }
            views.Clear();
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
