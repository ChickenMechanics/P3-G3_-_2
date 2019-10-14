using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner GetInstance;

    public enum SpawnState { SPAWNING, WAITING, COUNTING }

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
        public void SetHasSpawned( bool hasBeenSpawned ) { m_HasSpawned = hasBeenSpawned; }

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
    public bool GetIsAllWavesCompleted { private set; get; }

    private Transform m_Player;
    private int m_SubWaveIndex;
    private int m_CurrentWave;
    private float m_SearchCountdown = 1f;
    private float m_CurrentWaveDuration;
    private float m_TimeToNextSpawn;
    private float m_TimeToNextWave;
    private bool m_IsBetweenWaves;

    private SpawnState m_SpawnState = SpawnState.COUNTING;

    private void Start()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnWave(waves[0]));
        GetIsAllWavesCompleted = false;
    }

    private void Update()
    {
        if (IsWaveCompleted(waves[m_CurrentWave]))
        {
            GetIsAllWavesCompleted = true;
            WaveCompleted();
        }

        if (m_TimeToNextWave <= timeBetweenWaves && m_IsBetweenWaves)
        {
            StartCoroutine(SpawnWave(waves[m_CurrentWave]));
            m_CurrentWaveDuration = 0;
            m_IsBetweenWaves = false;
        }

        foreach (var subWave in waves[m_CurrentWave].subWaves)
        {
            if (subWave.spawnStartDelay <= m_CurrentWaveDuration && subWave.GetHasSpawned() == false)
                StartCoroutine(SpawnSubWave(subWave));
        }
        
        m_CurrentWaveDuration += Time.deltaTime;
    }

    private bool IsWaveCompleted(Wave wave)
    {
        if (m_SpawnState == SpawnState.WAITING && IsEnemyAlive() == false)
            return wave.subWaves.Any(subWave => subWave.GetHasSpawned() == false);

        return false;
    }

    private void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        m_SpawnState = SpawnState.COUNTING;

        if (m_CurrentWave == waves.Length - 1)
        {
            m_CurrentWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping...");
        }
        else
            m_CurrentWave++;

        m_TimeToNextWave = timeBetweenWaves;

        m_IsBetweenWaves = true;
    }

    private bool IsEnemyAlive()
    {
        m_SearchCountdown -= Time.deltaTime;
        
        if (m_SearchCountdown > 0f) return true;

        m_SearchCountdown = 1f;

        return GameObject.FindGameObjectWithTag("Enemy") != null;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Spawning Wave: " + wave.waveName);

        m_SpawnState = SpawnState.SPAWNING;

        yield return StartCoroutine(SpawnSubWave(wave.subWaves[0]));

        m_SpawnState = SpawnState.WAITING;
    }
    
    private IEnumerator SpawnSubWave(SubWave subWave)
    {
        subWave.SetHasSpawned(true);

        foreach (var enemyType in subWave.enemyTypes)
        {
            var spawnRate = 1f / enemyType.rate;
    
            for (var i = 0; i < enemyType.count; ++i)
            {
                yield return new WaitForSeconds(spawnRate);
    
                SpawnEnemy(enemyType.enemy);
            }
        }
    }

    private void SpawnEnemy(Transform enemy)
    {
        Debug.Log("Spawning Enemy: " + enemy.name);

        if (spawnPoints.Length == 0)
            Debug.LogError("No spawn points referenced");

        Vector3 playerPos = m_Player.position;

        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);

        const int numberOfTries = 100;

        for (int i = 0; i < numberOfTries; i++)
        {
            Transform spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Length)];
            Vector3 distanceToPlayer = playerPos - spawnPoint.position;

            if (distanceToPlayer.magnitude < safeSpawnDistance) continue;

            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            return;
        }

        Debug.Log("Enemy did not spawn since no spawn point was available");
    }
}