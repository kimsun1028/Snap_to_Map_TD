using UnityEngine;
using UnityEngine.Events;

namespace SnapToMapTD.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int startingGold = 150;
        [SerializeField] private int startingLives = 20;

        public int Gold { get; private set; }
        public int Lives { get; private set; }
        public bool IsGameOver { get; private set; }

        [Header("Events")]
        public UnityEvent<int> onGoldChanged;
        public UnityEvent<int> onLivesChanged;
        public UnityEvent onGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            Gold = startingGold;
            Lives = startingLives;
            onGoldChanged?.Invoke(Gold);
            onLivesChanged?.Invoke(Lives);
        }

        public bool TrySpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            onGoldChanged?.Invoke(Gold);
            return true;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
            onGoldChanged?.Invoke(Gold);
        }

        public void LoseLife(int amount = 1)
        {
            if (IsGameOver) return;
            Lives = Mathf.Max(0, Lives - amount);
            onLivesChanged?.Invoke(Lives);
            if (Lives <= 0)
            {
                IsGameOver = true;
                onGameOver?.Invoke();
            }
        }
    }
}
