using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private RectTransform fillRect;

        private Enemy enemy;
        private float fullWidth;

        private void Awake()
        {
            enemy = GetComponentInParent<Enemy>();
            fullWidth = fillRect.rect.width;
        }

        private void Update()
        {
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullWidth * enemy.NormalizedHealth);
        }
    }
}
