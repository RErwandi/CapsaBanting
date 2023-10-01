using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    public class Blackboard : Singleton<Blackboard>
    {
        protected override bool IsPersistBetweenScenes => false;
        
        [SerializeField, Required] private GameController gameController;
        public static GameController Game => Instance.gameController;

        public static Player LocalPlayer { get; set; }
        public static List<Player> AIPlayers { get; set; } = new();

        private void Start()
        {
            gameController.Initialize();

            GameEvent.Trigger(Constants.EVENT_GAME_INITIALIZED);
        }
    }
}
