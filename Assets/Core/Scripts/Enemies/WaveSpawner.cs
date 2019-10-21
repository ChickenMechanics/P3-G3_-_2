using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState
    {
        SPAWN,
        WAIT,
        BETWEENWAVES
    }

    [Serializable]
    public class Wave
    {
        public string waveName;
        public SubWave[] subWaves;
    }

    [Serializable]
    public class SubWave
    {
        public string subWaveName;
        public float spawnStartDelay;
        public EnemyType[] enemyTypes;

        public bool GetHasSpawned() { return m_HasSpawned; }
        public void SetHasSpawned(bool hasSpawned) { m_HasSpawned = hasSpawned; }
        private bool m_HasSpawned;
    }

    [Serializable]
    public class EnemyType
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    public Transform[] spawnPoints;
    public float safeSpawnDistance;
    public float timeBetweenWaves;

    private Transform m_Player;
    private const float SearchCountdown = 1f;

    private int m_CurrentWaveIndex;
    private int m_CurrentSubWaveIndex;
    private float m_CurrentWaveDuration;

    //private bool m_HasSpawnedWave;

    private SpawnState m_SpawnState;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_SpawnState = SpawnState.SPAWN;
    }

    private void Update()
    {
        switch (m_SpawnState)
        {
            case SpawnState.SPAWN:
                Spawn();
                break;
            case SpawnState.WAIT:
                StartCoroutine(Wait());
                break;
            case SpawnState.BETWEENWAVES:
                StartCoroutine(WaitForNextWave());
                break;
        }

        m_CurrentWaveDuration += Time.deltaTime;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(SearchCountdown);

        var currentWave = waves[m_CurrentWaveIndex];
        var currentSubWave = currentWave.subWaves[m_CurrentSubWaveIndex];

        if (currentSubWave.spawnStartDelay <= m_CurrentWaveDuration && currentSubWave.GetHasSpawned() == false)
            m_SpawnState = SpawnState.SPAWN;
        else if (IsWaveCompleted(waves[m_CurrentWaveIndex]))
            WaveCompleted();
    }

    private void Spawn()
    {
        var currentWave = waves[m_CurrentWaveIndex];
        var currentSubWave = currentWave.subWaves[m_CurrentSubWaveIndex];

        StartCoroutine(SpawnSubWave(currentSubWave));

        if (currentWave.subWaves.Length == 0)
        {
            Debug.Log("No subwaves to spawn");
            return;
        }

        if (m_CurrentSubWaveIndex < currentWave.subWaves.Length - 2)
            m_CurrentSubWaveIndex++;

        m_SpawnState = SpawnState.WAIT;
    }

    private IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        m_SpawnState = SpawnState.SPAWN;
        m_CurrentWaveDuration = 0;
    }

    private static bool IsWaveCompleted(Wave wave)
    {
        var allSubWavesSpawned = wave.subWaves.All(subWave => subWave.GetHasSpawned());
        var anyEnemiesAlive = GameObject.FindGameObjectWithTag("Enemy") != null;

        return anyEnemiesAlive == false && allSubWavesSpawned;
    }

    private void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        if (m_CurrentWaveIndex == waves.Length - 1)
            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);
        else
        {
            m_CurrentWaveIndex++;
            m_SpawnState = SpawnState.BETWEENWAVES;
        }
    }

    //private IEnumerator SpawnWave(Wave wave)
    //{
    //    Debug.Log("Spawning Wave: " + wave.waveName);

    //    if (wave.subWaves[m_CurrentSubWaveIndex] != null)
    //    {
    //        m_SpawnState = SpawnState.SPAWN;

    //        yield return
    //            StartCoroutine(
    //                SpawnSubWave(
    //                    wave.subWaves[m_CurrentSubWaveIndex]));
    //    }
    //    else
    //        Debug.Log("First subwave does not exist");

    //    m_SpawnState = SpawnState.WAIT;
    //}

    private IEnumerator SpawnSubWave(SubWave subWave)
    {
        if (subWave.enemyTypes.Length == 0)
        {
            Debug.Log("No enemyTypes to spawn");
            yield break;
        }

        foreach (var enemyType in subWave.enemyTypes)
        {
            if (enemyType.enemy == null)
            {
                Debug.Log("No enemies to spawn");
                yield break;
            }

            SpawnEnemy(enemyType.enemy);

            var spawnRate = 1f / enemyType.rate;

            for (var i = 1; i < enemyType.count; ++i)
            {
                if (enemyType.count == 0)
                {
                    Debug.Log("No " + enemyType.name + "s to spawn");
                    yield break;
                }

                yield return new WaitForSeconds(spawnRate);

                SpawnEnemy(enemyType.enemy);
            }
        }

        subWave.SetHasSpawned(true);
    }

    private void SpawnEnemy(Transform enemy)
    {
        if (spawnPoints.Length == 0)
            Debug.LogError("No spawn points referenced");

        Vector3 playerPos = m_Player.position;

        Random rnd = new System.Random(System.DateTime.Now.Millisecond);

        const int numberOfTries = 100;

        for (int i = 0; i < numberOfTries; i++)
        {
            Transform spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Length)];
            Vector3 distanceToPlayer = playerPos - spawnPoint.position;

            if (distanceToPlayer.magnitude < safeSpawnDistance) continue;

            Debug.Log("Spawning Enemy: " + enemy.name);

            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            return;
        }

        Debug.Log("Enemy did not spawn since no spawn point was available");
    }
}