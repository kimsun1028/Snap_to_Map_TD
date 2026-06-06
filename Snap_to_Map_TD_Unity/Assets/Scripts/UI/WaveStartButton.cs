using UnityEngine;
using UnityEngine.UI;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class WaveStartButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private WaveManager waveManager;

        private void Start()
        {
            waveManager.onWaveReady.AddListener(Show);
            waveManager.onWaveStart.AddListener(_ => Hide());
            waveManager.onAllWavesCleared.AddListener(Hide);
            GameManager.Instance?.onGameOver.AddListener(Hide);

            Hide();
        }

        private void Show() => button.gameObject.SetActive(true);
        private void Hide() => button.gameObject.SetActive(false);

        public void OnClick()
        {
            Hide();
            waveManager.StartNextWave();
        }
    }
}
