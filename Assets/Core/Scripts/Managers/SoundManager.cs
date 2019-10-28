using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    #region design vars
    public enum ESoundClip
    {
        MUSIC_MAIN_MENU,
        MUSIC_COMBAT,
        PLAYER_HURT,
        GUN_AR_SHOT,
        GUN_CANNON,
        CRAWLER_AR_DAMAGE,
        CRAWLER_DEATH,
        MJAU,
        GUN_RELOAD_1,
        ENEMY_ATTACK,
        ENEMY_SPAWN,
        PLAYER_FOOTSTEPS,
        ENEMY_FOOTSTEPS,
        ROCKET_SHOT,
        ROCKET_IMPACT,
        SIZE
    }
    #endregion


    // ----------------------------------------------------------------------------------------------------


    public static SoundManager GetInstance { private set; get; }

    private GameObject m_AudioFolder;
    private float m_VolumeScaler;
    public float m_MaxTriggerInterval = 0.05f;


    [System.Serializable]
    public class SoundObj
    {
        public string AliasName;
        public ESoundClip m_ESound;
        public GameObject m_SoundSource;
        [HideInInspector]
        public float m_PrevTime;
    }
    public List<SoundObj> m_SoundObjs;


    // ----------------------------------------------------------------------------------------------------


    public void PlaySoundClip(ESoundClip soundClipKey, Vector3 position, float startDelay = 0.0f, float dopplerLvl = 0.0f)
    {
        if (CanPlaySound(soundClipKey) == true)
        {
            if (startDelay != 0.0f)
            {
                startDelay = 44100.0f * startDelay; // Hz to sec... or something like that
            }

            if(m_SoundObjs[(int)soundClipKey].m_SoundSource != null)
            {
                GameObject obj = Instantiate(m_SoundObjs[(int)soundClipKey].m_SoundSource, position, Quaternion.identity, m_AudioFolder.transform);
                obj.name = m_SoundObjs[(int)soundClipKey].AliasName;
                AudioSource source = obj.GetComponent<AudioSource>();
                source.PlayDelayed((ulong)startDelay);
                Destroy(obj, source.clip.length);
            }
        }
    }


    public void DestroySoundClip(string aliasName)
    {
        GameObject go = GameObject.Find(aliasName);
        if(go == null)
        {
            Debug.LogWarning("SoundManager::DestroySoundClip(): Broken!");
            return;
        }

        go.GetComponent<AudioSource>().Stop();
        Destroy(go);
    }


    public AudioSource GetAudioSourceByAlias(string aliasName)
    {
        GameObject go = GameObject.Find(aliasName);
        if (go == null)
        {
#if DEBUG
            Debug.LogWarning("SoundManager::GetAudioSourceByAlias(): Broken!");
#endif
            return null;
        }

        return go.GetComponent<AudioSource>();
    }


    private bool CanPlaySound(ESoundClip soundClipKey)
    {
        // only effect certain sounds with a trigger frequenzy limiter, TimeChecker()
        switch (soundClipKey)
        {
            case ESoundClip.CRAWLER_DEATH:      return TimeChecker(soundClipKey);
            case ESoundClip.CRAWLER_AR_DAMAGE:  return TimeChecker(soundClipKey);
            case ESoundClip.ENEMY_ATTACK:       return TimeChecker(soundClipKey);
            case ESoundClip.ENEMY_SPAWN:        return TimeChecker(soundClipKey);
            case ESoundClip.PLAYER_HURT:        return TimeChecker(soundClipKey);
            default:                            return true;
        }
    }


    private bool TimeChecker(ESoundClip soundClipKey)
    {
        for(int i = 0; i < (int)ESoundClip.SIZE; ++i)
        {
            if (m_SoundObjs[i].m_ESound == soundClipKey)
            {
                if (m_MaxTriggerInterval > 0.0f)
                {
                    float localTimer = m_MaxTriggerInterval;
                    float prevTime = m_SoundObjs[i].m_PrevTime;
                    float timeNow = Time.time;
                    if (timeNow > localTimer + prevTime)
                    {
                        m_SoundObjs[i].m_PrevTime = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
        DontDestroyOnLoad(gameObject);

        m_AudioFolder = new GameObject("AudioFolder");
        m_AudioFolder.transform.position = Vector3.zero;
        m_AudioFolder.transform.parent = transform;
        DontDestroyOnLoad(m_AudioFolder);

        if (m_SoundObjs.Count > 0)
        {
            SoundObj[] tmp = m_SoundObjs.ToArray();
            m_SoundObjs.Clear();
            m_SoundObjs.Capacity = (int)ESoundClip.SIZE;
            for (int i = 0; i < m_SoundObjs.Capacity; ++i)
            {
                m_SoundObjs.Add(null);
            }

            for (int i = 0; i < tmp.Length; ++i)
            {
                m_SoundObjs[(int)tmp[i].m_ESound] = tmp[i];
            }
        }

#if DEBUG
        WhatsThis();
#endif
    }


    #region the stash
#if DEBUG
    private List<GameObject> m_SuperSecret;
    private AudioSource m_NowSource;
    private int nowIdx;
    private int nxtIdx;
    private bool IsPaused;
    private void WhatsThis()
    {
        m_SuperSecret = new List<GameObject>();
        List<string> tunes = new List<string> { "HempressSativa-RockItInaDance", "Protoje-Protection", "Protoje-WhoKnows", "SamoryI-RastaNuhGangsta" };
        for (int i = 0; i < tunes.Count; ++i)
        {
            GameObject go = new GameObject("tunes");
            m_SuperSecret.Add(go);
            go.transform.position = Vector3.zero;
            go.transform.parent = m_AudioFolder.transform;
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = (AudioClip)Resources.Load("SecretStash/" + tunes[i]);
            source.playOnAwake = false;
            source.volume = 0.25f;
            source.loop = false;
            source.maxDistance = 100.0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.dopplerLevel = 0.0f;
        }

        nowIdx = 0;
        nxtIdx = nowIdx;
        IsPaused = false;
    }
    private void ShuffleTune()
    {
        m_SuperSecret[nowIdx].GetComponent<AudioSource>().Stop();
        while (nxtIdx == nowIdx) nxtIdx = Random.Range(0, m_SuperSecret.Count);
        nowIdx = nxtIdx;
        m_NowSource = m_SuperSecret[nowIdx].GetComponent<AudioSource>();
        m_NowSource.Play();
    }
    private void NxtTune(int prevOrNxt)
    {
        m_SuperSecret[nowIdx].GetComponent<AudioSource>().Stop();
        nowIdx += prevOrNxt;
        if (nowIdx > m_SuperSecret.Count - 1) nowIdx = 0;
        else if(nowIdx < 0) nowIdx = m_SuperSecret.Count - 1;
        m_NowSource = m_SuperSecret[nowIdx].GetComponent<AudioSource>();
        m_NowSource.Play();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            NxtTune(1);
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            NxtTune(-1);
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            AudioSource source = m_SuperSecret[nowIdx].GetComponent<AudioSource>();
            if (source.isPlaying) { source.Pause(); } else { source.Play(); }
            IsPaused = !IsPaused;
        }
        if(m_NowSource != null)
        {
            if(PlayerManager.GetInstance != null)
            {
                m_NowSource.transform.position = PlayerManager.GetInstance.GetPlayer.transform.position;
            }

            if (m_NowSource.isPlaying != true &&
                !IsPaused)
            {
                NxtTune(1);
            }
        }
    }
#endif
    #endregion
}
