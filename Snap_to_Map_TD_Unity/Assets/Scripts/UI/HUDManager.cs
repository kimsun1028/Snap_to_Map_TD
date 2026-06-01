using TMPro;
using UnityEngine;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI livesText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private WaveManager waveManager;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.onGoldChanged.AddListener(UpdateGold);
                GameManager.Instance.onLivesChanged.AddListener(UpdateLives);
                UpdateGold(GameManager.Instance.Gold);
                UpdateLives(GameManager.Instance.Lives);
            }

            if (waveManager != null)
            {
                waveManager.onWaveStart.AddListener(UpdateWave);
                UpdateWave(0);
            }
        }

        private void UpdateGold(int amount) => goldText.text = $"Gold: {amount}";
        private void UpdateLives(int amount) => livesText.text = $"Lives: {amount}";
        private void UpdateWave(int wave)
        {
            if (waveManager == null) return;
            string display = wave == 0 ? "Wave: -" : $"Wave: {wave} / {waveManager.TotalWaves}";
            waveText.text = display;
        }
    }
}
