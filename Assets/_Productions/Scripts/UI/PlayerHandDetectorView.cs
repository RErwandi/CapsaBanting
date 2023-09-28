using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class PlayerHandDetectorView : MonoBehaviour, IEventListener<GameEvent>
    {
        private Player player;
        [SerializeField] private Button singleButton;
        [SerializeField] private Button pairButton;
        [SerializeField] private Button threeButton;
        [SerializeField] private Button straightButton;
        [SerializeField] private Button flushButton;
        [SerializeField] private Button fullHouseButton;
        [SerializeField] private Button fourButton;
        [SerializeField] private Button straightFlushButton;
        [SerializeField] private Button royalFlushButton;
        
        private void OnEnable()
        {
            EventManager.AddListener(this);
            singleButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => SingleClicked());
            pairButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => PairClicked());
            threeButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => ThreeClicked());
            straightButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => StraightClicked());
            flushButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => FlushClicked());
            fullHouseButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => FullHouseClicked());
            fourButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => FoursClicked());
            straightFlushButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => StraightFlushClicked());
            royalFlushButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => RoyalFlushClicked());
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(this);
        }

        private void Subscribe()
        {
            player = Blackboard.Controller.LocalPlayer;
            player.hasPair.TakeUntilDestroy(this).Subscribe(OnHasPairChanged);
            player.hasThree.TakeUntilDestroy(this).Subscribe(OnHasThreeChanged);
            player.hasStraight.TakeUntilDestroy(this).Subscribe(OnHasStraightChanged);
            player.hasFlush.TakeUntilDestroy(this).Subscribe(OnHasFlushChanged);
            player.hasFullHouse.TakeUntilDestroy(this).Subscribe(OnHasFullHouseChanged);
            player.hasFours.TakeUntilDestroy(this).Subscribe(OnHasFourChanged);
            player.hasStraightFlush.TakeUntilDestroy(this).Subscribe(OnHasStraightFlushChanged);
            player.hasRoyalFlush.TakeUntilDestroy(this).Subscribe(OnHasRoyalFlushChanged);
        }

        private void OnHasPairChanged(bool value)
        {
            pairButton.gameObject.SetActive(value);
        }
        
        private void OnHasThreeChanged(bool value)
        {
            threeButton.gameObject.SetActive(value);
        }
        
        private void OnHasStraightChanged(bool value)
        {
            straightButton.gameObject.SetActive(value);
        }
        
        private void OnHasFlushChanged(bool value)
        {
            flushButton.gameObject.SetActive(value);
        }
        
        private void OnHasFullHouseChanged(bool value)
        {
            fullHouseButton.gameObject.SetActive(value);
        }
        
        private void OnHasFourChanged(bool value)
        {
            fourButton.gameObject.SetActive(value);
        }
        
        private void OnHasStraightFlushChanged(bool value)
        {
            straightFlushButton.gameObject.SetActive(value);
        }
        
        private void OnHasRoyalFlushChanged(bool value)
        {
            royalFlushButton.gameObject.SetActive(value);
        }
        
        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_INITIALIZED)
            {
                Subscribe();
            }
        }
        
        private void SingleClicked()
        {
            player.SelectSingle();
        }

        private void PairClicked()
        {
            player.SelectPair();
        }

        private void ThreeClicked()
        {
            player.SelectThrees();
        }

        private void StraightClicked()
        {
            player.SelectStraight();
        }
        private void FlushClicked()
        {
            player.SelectFlush();
        }
        
        private void FullHouseClicked()
        {
            player.SelectFullHouse();
        }
        
        private void FoursClicked()
        {
            player.SelectFours();
        }
        
        private void StraightFlushClicked()
        {
            player.SelectStraightFlush();
        }
        
        private void RoyalFlushClicked()
        {
            player.SelectRoyalFlush();
        }
    }
}
