using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class PlayerHandView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private CardView cardView;
        [SerializeField] private Transform cardsContainer;

        private List<CardView> views = new();
        private CardHand Hand => Blackboard.Controller.LocalPlayer.hand;

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
            Hand.cards.ObserveCountChanged().TakeUntilDestroy(this)
                .Subscribe(_ => OnCardsChanged());
            
            OnCardsChanged();
        }

        private void OnCardsChanged()
        {
            ResetCards();
            foreach (var card in Hand.cards)
            {
                var view = Pool.Spawn(cardView, cardsContainer);
                view.SetCard(card);
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
