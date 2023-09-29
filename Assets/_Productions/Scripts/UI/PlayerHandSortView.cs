using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class PlayerHandSortView : MonoBehaviour
    {
        [SerializeField] private Button sortButton;
        [SerializeField] private Button resetButton;

        private void OnEnable()
        {
            resetButton.OnClickAsObservable().TakeUntilDisable(this)
                .Subscribe(_ => Blackboard.LocalPlayer.ResetSelected());
            sortButton.OnClickAsObservable().TakeUntilDisable(this)
                .Subscribe(_ => Blackboard.LocalPlayer.SortCards());
        }
    }
}
