using System;
using UnityEngine;

namespace CapsaBanting
{
    public class AIHandView : MonoBehaviour, IEventListener<GameEvent>
    {
        [SerializeField] private AIProfileView profileViewTemplate;
        [SerializeField] private Transform[] slots;

        private int iSlot;

        private void OnEnable()
        {
            EventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void Initialize()
        {
            foreach (var ai in Blackboard.AIPlayers)
            {
                var view = Instantiate(profileViewTemplate, slots[iSlot]);
                view.Initialize(ai);
                iSlot++;
            }
        }

        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_INITIALIZED)
            {
                Initialize();
            }
        }
    }
}
