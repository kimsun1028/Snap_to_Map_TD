using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class GameResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button restartButton;
        [SerializeField] private WaveManager waveManager;

        private void Start()
        {
            panel.SetActive(false);

            if (GameManager.Instance != null)
                GameManager.Instance.onGameOver.AddListener(ShowGameOver);

            if (waveManager != null)
                waveManager.onAllWavesCleared.AddListener(ShowStageClear);

            restartButton.onClick.AddListener(Restart);
        }

        private void ShowGameOver()
        {
            titleText.text = "Game Over";
            panel.SetActive(true);
        }

        private void ShowStageClear()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
            titleText.text = "Stage Clear!";
            panel.SetActive(true);
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
