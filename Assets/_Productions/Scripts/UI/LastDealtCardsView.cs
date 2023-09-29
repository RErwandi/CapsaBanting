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
        private CardHand Hand => Blackboard.Game.GameState.lastPlayerHand;

        private void OnEnable()
        {
            EventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void OnCardsChanged()
        {
            ResetCards();
            SetUpCards();
        }

        private void SetUpCards()
        {
            for (var i = 0; i < Hand.cards.Count; i++)
            {
                var card = Hand.cards[i];
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
            if (e.eventName == Constants.EVENT_CARDS_DEALT)
            {
                OnCardsChanged();
            }
        }
    }
}
