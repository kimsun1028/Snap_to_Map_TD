using UnityEngine;
using TMPro;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.UI
{
    public class EnemyInfoPanel : MonoBehaviour
    {
        public static EnemyInfoPanel Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI livesText;

        private Enemy trackedEnemy;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (trackedEnemy == null)
            {
                Hide();
                return;
            }
            hpText.text = $"HP  {trackedEnemy.CurrentHealth} / {trackedEnemy.MaxHealth}";
        }

        public void Show(Enemy enemy)
        {
            TowerInfoPanel.Instance?.Hide();
            trackedEnemy = enemy;
            enemyNameText.text = enemy.EnemyName;
            speedText.text = $"Speed  {enemy.MoveSpeed:0.#}";
            goldText.text = $"Gold  +{enemy.GoldReward}G";
            hpText.text = $"HP  {enemy.CurrentHealth} / {enemy.MaxHealth}";
            if (livesText != null)
                livesText.text = $"Life  {enemy.LivesTolLose}";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            trackedEnemy = null;
            gameObject.SetActive(false);
        }
    }
}
