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
    public GameObject m_SpawnEffect;

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

    public float waveTextX;
    public float waveTextY;
    public float showWaveNumberTime;
    public int TextWidth;

    private Transform m_Player;
    private int m_CurrentWaveIndex;
    private int m_CurrentSubWaveIndex;
    private float m_CurrentWaveDuration;
    private SpawnState m_SpawnState;
    private float m_TimeToNextWave;
    private bool m_BeginShowingWaveNumber;
    private float m_TempShowWaveNumberTime;

    private void Start()
    {
        m_TempShowWaveNumberTime = showWaveNumberTime;
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_SpawnState = SpawnState.SPAWN;
        m_TimeToNextWave = timeBetweenWaves;
    }

    private void Update()
    {
        switch (m_SpawnState)
        {
            case SpawnState.SPAWN: Spawn(); break;
            case SpawnState.WAIT: Wait(); break;
            case SpawnState.BETWEENWAVES: WaitForNextWave(); break;
        }

        m_CurrentWaveDuration += Time.deltaTime;
    }

    void OnGUI()
    {
        if (m_SpawnState == SpawnState.SPAWN && m_CurrentSubWaveIndex == 0)
            m_BeginShowingWaveNumber = true;

        if (!m_BeginShowingWaveNumber) return;

        if (m_TempShowWaveNumberTime > 0)
        {
            var rect = new Rect(waveTextX, waveTextY, Screen.width, TextWidth);

            var text = $"Wave: {m_CurrentWaveIndex + 1:0.}";

            var style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = TextWidth,
                normal = { textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) }
            };

            GUI.Label(rect, text, style);

            m_TempShowWaveNumberTime -= Time.deltaTime;
        }
        else
        {
            m_BeginShowingWaveNumber = false;
            m_TempShowWaveNumberTime = showWaveNumberTime;
        }
    }
    
    private void Wait()
    {
        var currentWave = waves[m_CurrentWaveIndex];
        var currentSubWave = currentWave.subWaves[m_CurrentSubWaveIndex];

        if (currentSubWave.spawnStartDelay <= m_CurrentWaveDuration &&
            m_CurrentSubWaveIndex < currentWave.subWaves.Length &&
            currentSubWave.GetHasSpawned() == false)
        {
            m_SpawnState = SpawnState.SPAWN;
        }
        else if (IsWaveCompleted(waves[m_CurrentWaveIndex]))
            WaveCompleted();
    }

    private void Spawn()
    {
        if (m_CurrentSubWaveIndex == 0)
        {
            SoundManager.GetInstance.PlaySoundClip(
                SoundManager.ESoundClip.WAVE_BEGIN,
                Camera.main.transform.position);
        }

        var currentWave = waves[m_CurrentWaveIndex];
        var currentSubWave = currentWave.subWaves[m_CurrentSubWaveIndex];

        if (currentWave.subWaves.Length == 0)
        {
            Debug.Log("No subwaves to spawn");
            return;
        }

        if (currentSubWave.GetHasSpawned() == false &&
            m_CurrentWaveDuration >= currentSubWave.spawnStartDelay)
        {
            StartCoroutine(SpawnSubWave(currentSubWave));

            if (m_CurrentSubWaveIndex < currentWave.subWaves.Length - 1)
                m_CurrentSubWaveIndex++;
        }

        m_SpawnState = SpawnState.WAIT;
    }

    private void WaitForNextWave()
    {
        if (m_TimeToNextWave - Time.deltaTime <= 0)
        {
            m_TimeToNextWave = timeBetweenWaves;
            m_CurrentWaveDuration = 0;
            m_CurrentSubWaveIndex = 0;
            m_SpawnState = SpawnState.SPAWN;
        }

        m_TimeToNextWave -= Time.deltaTime;
    }

    private static bool IsWaveCompleted(Wave wave)
    {
        var allSubWavesSpawned = wave.subWaves.All(subWave => subWave.GetHasSpawned());
        var allEnemiesDead = GameObject.FindGameObjectWithTag("Enemy") == null;

        return allEnemiesDead && allSubWavesSpawned;
    }

    private void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        if (m_CurrentWaveIndex < waves.Length - 1)
        {
            m_CurrentWaveIndex++;
            m_SpawnState = SpawnState.BETWEENWAVES;
        }
        else
            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);
    }
    
    private IEnumerator SpawnSubWave(SubWave subWave)
    {
        if (subWave.enemyTypes.Length == 0)
        {
            Debug.Log("No enemyTypes to spawn");
            yield break;
        }

        subWave.SetHasSpawned(true);

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
    }

    private void SpawnEnemy(Transform enemy)
    {
        if (spawnPoints.Length == 0)
            Debug.LogError("No spawn points referenced");

        var playerPos = m_Player.position;

        var rnd = new Random(DateTime.Now.Millisecond);

        const int numberOfTries = 100;

        for (var i = 0; i < numberOfTries; i++)
        {
            var spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Length)];
            var distanceToPlayer = playerPos - spawnPoint.position;

            if (distanceToPlayer.magnitude < safeSpawnDistance) continue;

            GameObject go = Instantiate(m_SpawnEffect, spawnPoint.position, Quaternion.identity);
            Destroy(go, 0.75f);

            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            return;
        }

        Debug.Log("Enemy did not spawn since no spawn point was available");
    }
}