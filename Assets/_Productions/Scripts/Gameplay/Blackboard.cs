using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    public class Blackboard : Singleton<Blackboard>
    {
        protected override bool IsPersistBetweenScenes => false;
        
        [SerializeField, Required] private GameController gameController;
        public static GameController Game => Instance.gameController;

        [SerializeField, Required] private AIController aiController;
        public static AIController AI => Instance.aiController;
        
        [ShowInInspector, ReadOnly] public static Player LocalPlayer { get; set; }

        private void Start()
        {
            gameController.Initialize();

            GameEvent.Trigger(Constants.EVENT_GAME_INITIALIZED);
        }
    }
}
