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
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI sellText;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private RangeIndicator rangeIndicator;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI skillDescriptionText;
        [SerializeField] private Image skillCooldownGauge;
        [SerializeField] private GameObject statsSection;
        [SerializeField] private GameObject descriptionSection;

        private Tower selectedTower;
        private bool isPreview;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isPreview || selectedTower == null) return;

            if (skillCooldownGauge != null)
                skillCooldownGauge.fillAmount = selectedTower.SkillCooldown > 0f
                    ? selectedTower.SkillTimeRemaining / selectedTower.SkillCooldown
                    : 0f;

            if (attackSpeedText != null)
                attackSpeedText.text = selectedTower.IsBuffed
                    ? $"SPD  {selectedTower.AttackSpeed:0.0} ▲"
                    : $"SPD  {selectedTower.AttackSpeed:0.0}";
        }

        public void Show(Tower tower)
        {
            EnemyInfoPanel.Instance?.Hide();
            isPreview = false;
            selectedTower = tower;
            SetStatsVisible(true);
            SetDescriptionVisible(false);
            Refresh();
            gameObject.SetActive(true);
            rangeIndicator?.Show(tower.transform.position, tower.Range);
        }

        public void ShowPreview(TowerData data)
        {
            isPreview = true;
            SetStatsVisible(false);
            SetDescriptionVisible(true);
            rangeIndicator?.Hide();
            towerNameText.text = data.towerName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (skillDescriptionText != null) skillDescriptionText.text = data.skillDescription;
            gameObject.SetActive(true);
        }

        public void HidePreview()
        {
            isPreview = false;
            if (selectedTower != null)
            {
                SetStatsVisible(true);
                Refresh();
                rangeIndicator?.Show(selectedTower.transform.position, selectedTower.Range);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            isPreview = false;
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

        private void SetStatsVisible(bool visible)
        {
            if (statsSection != null) statsSection.SetActive(visible);
        }

        private void SetDescriptionVisible(bool visible)
        {
            if (descriptionSection != null) descriptionSection.SetActive(visible);
        }

        private void Refresh()
        {
            if (selectedTower == null) return;
            towerNameText.text = selectedTower.TowerName;
            levelText.text = $"Lv {selectedTower.Level}";
            powerText.text = $"Power {selectedTower.Power:0}";
            sellText.text = $"Sell +{selectedTower.SellPrice}G";

            var data = selectedTower.Data;
            if (descriptionText != null) descriptionText.text = data != null ? data.description : "";
            if (skillDescriptionText != null) skillDescriptionText.text = data != null ? data.skillDescription : "";

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
