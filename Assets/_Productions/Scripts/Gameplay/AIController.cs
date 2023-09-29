using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CapsaBanting
{
    public class AIController : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] public List<Player> AIPlayers { get; set; } = new();
    }
}
