using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CapsaBanting
{
    public class CharacterSelectionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button submitButton;

        private void OnEnable()
        {
            submitButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => Submit());
        }

        public void Select(PlayerProfile profile)
        {
            nameText.text = profile.PlayerName;
            var profileIndex = Blackboard.Game.Profiles.IndexOf(profile);
            Blackboard.Game.Profiles.Swap(0, profileIndex);
        }

        private void Submit()
        {
            gameObject.SetActive(false);
            Blackboard.Instance.StartGame();
        }
    }
}
