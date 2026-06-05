using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnapToMapTD.Towers;
using SnapToMapTD.Game;

namespace SnapToMapTD.UI
{
    public class TowerInfoPanel : MonoBehaviour
    {
        public static TowerInfoPanel Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI powerText;
        [SerializeField] private TextMeshProUGUI sellText;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private RangeIndicator rangeIndicator;

        private Tower selectedTower;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void Show(Tower tower)
        {
            selectedTower = tower;
            Refresh();
            gameObject.SetActive(true);
            rangeIndicator?.Show(tower.transform.position, tower.Range);
        }

        public void Hide()
        {
            selectedTower = null;
            gameObject.SetActive(false);
            rangeIndicator?.Hide();
        }

        public void OnUpgrade()
        {
            if (selectedTower == null || !selectedTower.CanUpgrade) return;
            if (!GameManager.Instance.TrySpendGold(selectedTower.UpgradeCost)) return;
            selectedTower.Upgrade();
            Refresh();
        }

        public void OnSell()
        {
            if (selectedTower == null) return;
            GameManager.Instance.AddGold(selectedTower.SellPrice);
            Destroy(selectedTower.gameObject);
            Hide();
        }

        private void Refresh()
        {
            if (selectedTower == null) return;
            towerNameText.text = selectedTower.TowerName;
            levelText.text = $"Lv {selectedTower.Level}";
            powerText.text = $"Power {selectedTower.Power:0}";
            sellText.text = $"Sell +{selectedTower.SellPrice}G";

            if (selectedTower.CanUpgrade)
            {
                upgradeButton.interactable = true;
                upgradeText.text = $"Up -{selectedTower.UpgradeCost}G";
            }
            else
            {
                upgradeButton.interactable = false;
                upgradeText.text = "Max Lv";
            }
        }
    }
}
