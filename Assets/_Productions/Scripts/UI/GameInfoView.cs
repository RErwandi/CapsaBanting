using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CapsaBanting
{
    public class GameInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomText;
        [SerializeField] private TextMeshProUGUI betText;

        private void Start()
        {
            roomText.text = $"Room: {Blackboard.Game.NumberOfPlayers} Players";
            betText.text = $"Bet: ${Blackboard.Game.Bet:n0}";
        }
    }
}
