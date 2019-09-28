using System;
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

    private Transform player;
    public float timeBetweenWaves;
    private int subWaveIndex;
    private int currentWave;
    private float waveCountdown;
    private float searchCountdown = 1f;
    private float totalWaveTime;
    private float timeToNextSpawn;

    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
    }

    void Update()
    {
        if (state == SpawnState.WAITING && EnemyIsAlive() == false)
            SubWaveCompleted(ref subWaveIndex, waves[currentWave].subWaves.Length);

        if (waveCountdown <= 0 && state != SpawnState.SPAWNING)
        {
            SpawnWave(waves[currentWave]);
            totalWaveTime = 0;
        }
        else
            waveCountdown -= Time.deltaTime;

        foreach (var subWave in waves[currentWave].subWaves)
        {
            if (subWave.spawnStartDelay <= totalWaveTime && subWave.GetHasSpawned() == false)
                SpawnSubWave(subWave);
        }

        //var entireSubWaveSpawned = false;

        //var currentSubWave = wave.subWaves[subWaveIndex];

        //while (entireSubWaveSpawned == false)
        //{
        //    SpawnSubWave(currentSubWave);

        //    entireSubWaveSpawned =
        //        currentSubWave.enemyTypes.All(
        //            enemyType => currentSubWave.GetHasSpawned());
        //}


        totalWaveTime += Time.deltaTime;
    }

    private void SubWaveCompleted(ref int subWaveIndex, int subWaveLength)
    {
        if (subWaveIndex != subWaveLength)
        {
            subWaveIndex++;
            SpawnSubWave(waves[currentWave].subWaves[subWaveIndex]);
        }
        else 
            WaveCompleted();
    }


    void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (currentWave >= waves.Length - 1)
        {
            currentWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping...");
        }
        else
            currentWave++;
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        
        if (searchCountdown > 0f) return true;

        searchCountdown = 1f;

        return GameObject.FindGameObjectWithTag("Enemy") != null;
    }

    private void SpawnWave(Wave wave)
    {
        Debug.Log("Spawning Wave: " + wave.waveName);

        state = SpawnState.SPAWNING;

        //StartCoroutine(SpawnSubWave(wave.subWaves[0]));



        SpawnSubWave(wave.subWaves[0]);

        state = SpawnState.WAITING;
    }

    //IEnumerator SpawnSubWave(SubWave subWave)
    //{
    //    foreach (var enemyType in subWave.enemyTypes)
    //    {
    //        for (var i = 0; i < enemyType.count; ++i)
    //        {
    //            SpawnEnemy(enemyType.enemy);
    //            yield return new WaitForSeconds(1f / enemyType.rate);
    //        }
    //    }
    //}

    void SpawnSubWave(SubWave subWave)
    {
        foreach (var enemyType in subWave.enemyTypes)
        {
            for (var i = 0; i < enemyType.count; ++i)
            {
                //timeToNextSpawn -= Time.deltaTime;

                //var spawnRate = 1f / enemyType.rate;

                //if (timeToNextSpawn >= spawnRate) continue;

                SpawnEnemy(enemyType.enemy);
               // timeToNextSpawn = 1f;
            }

            subWave.SetHasSpawned(true);
        }
    }

    void SpawnEnemy(Transform enemy)
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
