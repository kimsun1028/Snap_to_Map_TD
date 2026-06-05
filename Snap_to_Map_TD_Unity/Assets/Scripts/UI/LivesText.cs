using TMPro;
using UnityEngine;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class LivesText : MonoBehaviour
    {
        private TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.onLivesChanged.AddListener(UpdateLives);
                UpdateLives(GameManager.Instance.Lives);
            }
        }

        private void UpdateLives(int amount) => text.text = $"Lives: {amount}";
    }
}
