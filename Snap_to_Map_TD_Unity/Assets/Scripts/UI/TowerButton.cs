using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using SnapToMapTD.Towers;

namespace SnapToMapTD.UI
{
    public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TowerData towerData;
        [SerializeField] private TowerPlacer placer;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI costText;

        private void Start()
        {
            if (iconImage != null && towerData != null && towerData.icon != null)
                iconImage.sprite = towerData.icon;

            RefreshCost();
            GetComponent<Button>().onClick.AddListener(OnClick);

            if (placer != null)
                placer.onTowerPlaced += RefreshCost;
        }

        private void OnDestroy()
        {
            if (placer != null)
                placer.onTowerPlaced -= RefreshCost;
        }

        private void RefreshCost()
        {
            if (costText != null && towerData != null && placer != null)
                costText.text = $"{placer.GetCurrentCost(towerData)}G";
        }

        private void OnClick()
        {
            if (towerData != null && placer != null)
                placer.SelectTower(towerData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (towerData != null)
                TowerInfoPanel.Instance?.ShowPreview(towerData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TowerInfoPanel.Instance?.HidePreview();
        }
    }
}
