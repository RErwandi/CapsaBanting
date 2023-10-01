using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class GameEndedView : MonoBehaviour, IEventListener<PLayerWinEvent>
    {
        [SerializeField] private GameObject overlay;
        [SerializeField] private RectTransform panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;

        private void OnEnable()
        {
            EventManager.AddListener(this);
            
            retryButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OnRetryClicked());
            exitButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OnExitClicked());
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void OnRetryClicked()
        {
            Hide();
            Blackboard.Game.RestartGame();
        }

        private void OnExitClicked()
        {
            Application.Quit();
        }

        private void Hide()
        {
            overlay.SetActive(false);
        }

        private void Show()
        {
            overlay.SetActive(true);
            panel.DOScale(0f, 0f);
            panel.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }

        private void Win()
        {
            Show();
            titleText.text = $"You Win!";
        }

        private void Lose()
        {
            Show();
            titleText.text = $"You Lose!";
        }

        public void OnEvent(PLayerWinEvent e)
        {
            if (e.indexPlayer == 0)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }
    }
}
