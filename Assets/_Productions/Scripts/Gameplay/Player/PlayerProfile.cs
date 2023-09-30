using UnityEngine;

namespace CapsaBanting
{
    [CreateAssetMenu(order = 0, fileName = "Player Profile", menuName = "Capsa/Player Profile")]
    public class PlayerProfile : ScriptableObject
    {
        [SerializeField] private string playerName;
        [SerializeField] private Sprite[] playerIcon;
        [SerializeField] private AudioClipSettings voiceDeal;
        [SerializeField] private AudioClipSettings voicePass;
        [SerializeField] private AudioClipSettings voiceLose;
        [SerializeField] private AudioClipSettings voiceWin;

        public string PlayerName => playerName;
        public Sprite NormalFace => playerIcon[0];
        public Sprite HappyFace => playerIcon[1];
        public Sprite SadFace => playerIcon[2];
        public AudioClipSettings VoiceDeal => voiceDeal;
        public AudioClipSettings VoicePass => voicePass;
        public AudioClipSettings VoiceLose => voiceLose;
        public AudioClipSettings VoiceWin => voiceWin;
    }
}
