using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnapToMapTD.Towers;

namespace SnapToMapTD.UI
{
    public class TowerButton : MonoBehaviour
    {
        [SerializeField] private TowerData towerData;
        [SerializeField] private TowerPlacer placer;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI costText;

        private void Start()
        {
            if (iconImage != null && towerData != null && towerData.icon != null)
                iconImage.sprite = towerData.icon;

            if (costText != null && towerData != null)
                costText.text = $"{towerData.cost}G";

            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (towerData != null && placer != null)
                placer.SelectTower(towerData);
        }
    }
}
