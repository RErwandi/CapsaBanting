using System;
using UnityEngine;

namespace CapsaBanting
{
    public class Blackboard : Singleton<Blackboard>
    {
        [SerializeField] private GameController gameController;
        public static GameController Controller => Instance.gameController;

        private void Start()
        {
            gameController.Initialize();

            GameEvent.Trigger(Constants.EVENT_GAME_INITIALIZED);
        }
    }
}