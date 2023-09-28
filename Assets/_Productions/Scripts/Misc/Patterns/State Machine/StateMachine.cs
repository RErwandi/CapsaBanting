using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace CapsaBanting
{
    public class StateMachine : MonoBehaviour
    {
        [ValueDropdown("states")]
        public State defaultState;
        public State[] states = Array.Empty<State>();

        [ShowInInspector, ReadOnly]
        public ReactiveProperty<State> currentState = new ReactiveProperty<State>();
    
        private void Start()
        {
            foreach (var state in states)
            {
                if(state.gameObject.activeSelf)
                    state.gameObject.SetActive(false);
            }
                
            SetState(defaultState.StateName);
        }

        /// <summary>
        /// Change state. this will call OnStateExit on the current state before calling OnStateEnter on the target state.
        /// </summary>
        /// <param name="stateName">Name of the state</param>
        public void SetState(string stateName)
        {
            var newState = states.FirstOrDefault(o => o.StateName == stateName);
    
            if(newState != null)
            {
                if (currentState.Value != null)
                {
                    currentState.Value.onStateExit?.Invoke();
                    currentState.Value.gameObject.SetActive(false);
                }
    
                newState.gameObject.SetActive(true);
                currentState.Value = newState;
                currentState.Value.onStateEnter?.Invoke();
            }
            else
                Debug.Log($"{gameObject.name} : Trying to set unknown state {stateName}");
        }
    }
}