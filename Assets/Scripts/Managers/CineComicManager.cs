using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Data;

namespace Managers
{
    public class CineComicManager : MonoBehaviour
    {
        public static CineComicManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject comicCanvas;
        [SerializeField] private Image panelDisplay;
        [SerializeField] private Text dialogueText;

        private Queue<ComicPanel> _panelQueue = new Queue<ComicPanel>();
        private bool _isPlaying = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (comicCanvas != null)
                comicCanvas.SetActive(false);
        }

        public void PlayComicSequence(List<ComicPanel> panels)
        {
            if (_isPlaying) return;
            
            _panelQueue.Clear();
            foreach (var p in panels) _panelQueue.Enqueue(p);

            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            _isPlaying = true;
            if (comicCanvas) comicCanvas.SetActive(true);

            // Pause Game?
            // Time.timeScale = 0f; 

            while (_panelQueue.Count > 0)
            {
                ComicPanel current = _panelQueue.Dequeue();
                ShowPanel(current);

                // Wait for duration or input
                if (current.duration > 0)
                    yield return new WaitForSecondsRealtime(current.duration);
                else
                {
                    // Wait for click
                    yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                }
            }

            EndComic();
        }

        private void ShowPanel(ComicPanel panel)
        {
            if (panelDisplay && panel.panelImage)
                panelDisplay.sprite = panel.panelImage;
            
            if (dialogueText)
                dialogueText.text = panel.dialogue;

            if (panel.shakeCameraOnAppear)
            {
                // TODO: Call Camera Shake
                Debug.Log("Camera SHAKE!");
            }
        }

        private void EndComic()
        {
            _isPlaying = false;
            if (comicCanvas) comicCanvas.SetActive(false);
            // Time.timeScale = 1f;
        }
    }
}
