using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class PlayerHandView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private CardView cardView;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private LayoutGroup layoutGroup;

        private List<CardView> views = new();
        private Player LocalPlayer => Blackboard.Controller.LocalPlayer;
        private CardHand Hand => LocalPlayer.hand;

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
            SetUpCards();
        }

        private void SetUpCards()
        {
            for (var i = 0; i < Hand.cards.Count; i++)
            {
                var card = Hand.cards[i];
                var view = Pool.Spawn(cardView, cardsContainer);
                view.SetCard(card, i, LocalPlayer);
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
