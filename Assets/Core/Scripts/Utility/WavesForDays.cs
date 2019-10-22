using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WavesForDays : MonoBehaviour
{
    // wave
    [System.Serializable]
    public class Wave
    {
        public string m_Alias;
        public List<SubWave> m_SubWaves;

        [HideInInspector]
        public int m_NowSub;
    }

    // sub
    [System.Serializable]
    public class SubWave
    {
        public string m_Alias;
        public int m_NumSpawns;
        public float m_SpawnFreqSec;
        public List<GameObject> m_EnemyTypesPrefab;

        [HideInInspector]
        public float m_SpawnTimer;
        [HideInInspector]
        public int m_NowSpawn;
    }

    public List<Wave> m_Waves;
    private int m_NowWave;

    private bool m_Flag;



    //----------------------------------------------------------------------------------------------------


    private void Init()
    {
        for(int i = 0; i < m_Waves.Count; ++i)
        {
            m_Waves[i].m_NowSub = 0;

            for (int j = 0; j < m_Waves[i].m_SubWaves.Count; ++j)
            {
                m_Waves[i].m_SubWaves[j].m_SpawnTimer = 0.0f;
                m_Waves[i].m_SubWaves[j].m_NowSpawn = 0;
            }
        }


        m_NowWave = 0;
        m_Flag = true;
    }


    private IEnumerator Spawn()
    {
        //int subIdx = m_Waves[m_NowWave].m_NowSub; // can't use as something something refrence double increment
        m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_SpawnTimer += Time.deltaTime;

        while(  m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_SpawnTimer <
                m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_SpawnFreqSec)
        {
            yield return null;
        }
            

        // spawn
        //Debug.Log("New Spawn");
        Instantiate(m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_EnemyTypesPrefab[0], transform);


        if( m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_NowSpawn <
            m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_NumSpawns - 1)
        {
            ++m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_NowSpawn;
            m_Waves[m_NowWave].m_SubWaves[m_Waves[m_NowWave].m_NowSub].m_SpawnTimer = 0.0f;

            yield return null;
        }


        if (m_Waves[m_NowWave].m_NowSub < m_Waves[m_NowWave].m_SubWaves.Count - 1)
        {
            //Debug.Log("New Sub");
            ++m_Waves[m_NowWave].m_NowSub;

            yield return null;
        }


        if (m_NowWave < m_Waves.Count - 1)
        {
            //Debug.Log("New Wave");
            ++m_NowWave;
        }
    }


    private void Awake()
    {
        Init();
    }


    private void Update()
    {
        if(m_Flag == true)
        {
            StartCoroutine(Spawn());
        }
    }
}
