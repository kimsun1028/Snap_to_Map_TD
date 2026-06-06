using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Game
{
    [Serializable]
    public struct EnemySpawn
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
    }

    [Serializable]
    public struct WaveEntry
    {
        public EnemySpawn[] enemies;
        public float delayAfterEntry;
    }

    [Serializable]
    public struct Wave
    {
        public WaveEntry[] entries;
        public float delayBeforeWave;
    }

    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }
        [Header("References")]
        [SerializeField] private MapManager mapManager;

        [Header("Waves")]
        [SerializeField] private Wave[] waves;
        [SerializeField] private float cooldownBetweenWaves = 5f;

        [Header("Events")]
        public UnityEvent<int> onWaveStart;
        public UnityEvent onWaveReady;
        public UnityEvent onAllWavesCleared;

        private int currentWave;
        private int activeEnemyCount;
        private bool waveStartRequested;

        public int CurrentWave => currentWave;
        public int TotalWaves => waves.Length;
        public bool IsWaveRunning { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartCoroutine(RunWaves());
        }

        public void StartNextWave()
        {
            waveStartRequested = true;
        }

        private IEnumerator RunWaves()
        {
            yield return null; // WaveStartButton.Start()가 구독할 때까지 한 프레임 대기
            while (currentWave < waves.Length)
            {
                waveStartRequested = false;
                IsWaveRunning = false;
                onWaveReady?.Invoke();
                yield return new WaitUntil(() => waveStartRequested);

                Wave wave = waves[currentWave];
                yield return new WaitForSeconds(wave.delayBeforeWave);

                IsWaveRunning = true;
                onWaveStart?.Invoke(currentWave + 1);

                foreach (WaveEntry entry in wave.entries)
                {
                    foreach (EnemySpawn spawn in entry.enemies)
                        StartCoroutine(SpawnEnemies(spawn));

                    if (entry.delayAfterEntry > 0f)
                        yield return new WaitForSeconds(entry.delayAfterEntry);
                }

                yield return new WaitUntil(() => activeEnemyCount > 0);
                yield return new WaitUntil(() => activeEnemyCount <= 0);
                currentWave++;
            }

            onAllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnEnemies(EnemySpawn spawn)
        {
            for (int i = 0; i < spawn.count; i++)
            {
                SpawnEnemy(spawn.enemyPrefab);
                if (i < spawn.count - 1)
                    yield return new WaitForSeconds(spawn.spawnInterval);
            }
        }

        private void SpawnEnemy(GameObject prefab)
        {
            if (prefab == null) return;

            GameObject go = Instantiate(prefab);
            Enemy enemy = go.GetComponent<Enemy>();
            if (enemy == null) return;

            enemy.Initialize(new List<Vector3>(mapManager.worldWaypoints));
            enemy.OnDeath += HandleEnemyDeath;
            enemy.OnReachEnd += HandleEnemyReachEnd;
            activeEnemyCount++;
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            activeEnemyCount--;
            GameManager.Instance?.AddGold(enemy.GoldReward);
        }

        private void HandleEnemyReachEnd(Enemy enemy)
        {
            activeEnemyCount--;
            for (int i = 0; i < enemy.LivesTolLose; i++)
                GameManager.Instance?.LoseLife();
        }
    }
}
