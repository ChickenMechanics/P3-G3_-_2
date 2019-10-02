using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class WaveSpawner : MonoBehaviour
{
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

    private Transform player;
    private int subWaveIndex;
    private int currentWave;
    private float searchCountdown = 1f;
    private float currentWaveDuration;
    private float timeToNextSpawn;
    private float timeToNextWave;
    private bool isBetweenWaves;

    private SpawnState spawnState = SpawnState.COUNTING;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnWave(waves[0]));
    }

    private void Update()
    {
        if (IsWaveCompleted(waves[currentWave]))
            WaveCompleted();

        if (timeToNextWave <= timeBetweenWaves && isBetweenWaves /*spawnState != SpawnState.SPAWNING*/)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
            currentWaveDuration = 0;
            isBetweenWaves = false;
        }

        foreach (var subWave in waves[currentWave].subWaves)
        {
            if (subWave.spawnStartDelay <= currentWaveDuration && subWave.GetHasSpawned() == false)
                StartCoroutine(SpawnSubWave(subWave));
        }
        
        currentWaveDuration += Time.deltaTime;
    }

    private bool IsWaveCompleted(Wave wave)
    {
        if (spawnState == SpawnState.WAITING && IsEnemyAlive() == false)
            return wave.subWaves.Any(subWave => subWave.GetHasSpawned() == false);

        return false;
    }

    private void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        spawnState = SpawnState.COUNTING;

        if (currentWave == waves.Length - 1)
        {
            currentWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping...");
        }
        else
            currentWave++;

        isBetweenWaves = true;
    }

    private bool IsEnemyAlive()
    {
        searchCountdown -= Time.deltaTime;
        
        if (searchCountdown > 0f) return true;

        searchCountdown = 1f;

        return GameObject.FindGameObjectWithTag("Enemy") != null;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Spawning Wave: " + wave.waveName);

        spawnState = SpawnState.SPAWNING;

        yield return StartCoroutine(SpawnSubWave(wave.subWaves[0]));

        spawnState = SpawnState.WAITING;
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

        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var playerPos = player.position;
        var distanceToPlayer = playerPos.magnitude - spawnPoint.position.magnitude;

        while (Math.Abs(distanceToPlayer) < safeSpawnDistance)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            playerPos = player.position;
            distanceToPlayer = playerPos.magnitude - spawnPoint.position.magnitude;
        }

        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }
}