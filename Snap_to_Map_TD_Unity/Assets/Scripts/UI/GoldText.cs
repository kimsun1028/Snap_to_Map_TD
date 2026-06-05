using TMPro;
using UnityEngine;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class GoldText : MonoBehaviour
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
                GameManager.Instance.onGoldChanged.AddListener(UpdateGold);
                UpdateGold(GameManager.Instance.Gold);
            }
        }

        private void UpdateGold(int amount) => text.text = $"Gold: {amount}";
    }
}
