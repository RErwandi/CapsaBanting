using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class LastDealtCardsView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private CardView cardView;
        [SerializeField] private RectTransform cardsContainer;
        [SerializeField] private RectTransform containerTemplate;
        [SerializeField] private RectTransform[] spawners;
        
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
            SetUpCards();
        }

        private void SetUpCards()
        {
            var container = Instantiate(containerTemplate, cardsContainer);
            for (var i = 0; i < Blackboard.Game.GameState.LastPlayerHand.cards.Count; i++)
            {
                var card = Blackboard.Game.GameState.LastPlayerHand.cards[i];
                var view = Pool.Spawn(cardView, container);
                view.SetCard(card, i);
                views.Add(view);
            }

            var originPos = spawners[Blackboard.Game.GameState.lastPlayerTurn].position;
            container.DOMove(originPos, 0f);
            container.DOMove(cardsContainer.position, 1f).SetEase(Ease.Linear);
            container.DOLocalRotate(new Vector3(0f, 0f, Random.Range(-45f, 45f)), 0f);
        }

        private void ResetTable()
        {
            foreach (var view in views)
            {
                Pool.Despawn(view);
            }
            views.Clear();
            cardsContainer.gameObject.DestroyChildren();
        }

        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_INITIALIZED)
            {
                Subscribe();
            }

            if (e.eventName == Constants.EVENT_TABLE_CLEAR)
            {
                ResetTable();
            }
        }
    }
}
