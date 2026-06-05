using TMPro;
using UnityEngine;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class WaveText : MonoBehaviour
    {
        [SerializeField] private WaveManager waveManager;

        private TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            text.text = "Wave: -";

            if (waveManager != null)
            {
                waveManager.onWaveStart.AddListener(UpdateWave);
            }
        }

        private void UpdateWave(int wave)
        {
            if (waveManager == null) return;
            string display = wave == 0 ? "Wave: -" : $"Wave: {wave} / {waveManager.TotalWaves}";
            text.text = display;
        }
    }
}
