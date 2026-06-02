using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Game
{
    [Serializable]
    public struct WaveEntry
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
    }

    [Serializable]
    public struct Wave
    {
        public WaveEntry[] entries;
        public float delayBeforeWave;
    }

    public class WaveManager : MonoBehaviour
    {
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
            while (currentWave < waves.Length)
            {
                waveStartRequested = false;
                onWaveReady?.Invoke();
                yield return new WaitUntil(() => waveStartRequested);

                Wave wave = waves[currentWave];
                yield return new WaitForSeconds(wave.delayBeforeWave);

                onWaveStart?.Invoke(currentWave + 1);

                foreach (WaveEntry entry in wave.entries)
                {
                    for (int i = 0; i < entry.count; i++)
                    {
                        SpawnEnemy(entry.enemyPrefab);
                        yield return new WaitForSeconds(entry.spawnInterval);
                    }
                }

                yield return new WaitUntil(() => activeEnemyCount <= 0);
                currentWave++;
            }

            onAllWavesCleared?.Invoke();
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
            GameManager.Instance?.LoseLife();
        }
    }
}
