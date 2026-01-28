using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class ComicPanel
    {
        [Header("Visuals")]
        public Sprite panelImage;
        [Tooltip("Text to display in the speech bubble")]
        [TextArea(3, 5)]
        public string dialogue;

        [Header("Audio")]
        public AudioClip voiceClip;
        public AudioClip sfxClip;

        [Header("Settings")]
        [Tooltip("How long to show this panel (seconds). Set 0 for manual advance.")]
        public float duration = 3f;
        public bool shakeCameraOnAppear = false;
    }
}
