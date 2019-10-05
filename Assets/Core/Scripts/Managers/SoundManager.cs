using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    #region design vars
    public enum ESoundClip
    {
        // music ------------------------------------------------------------------------------------------
        //MUSIC_MAIN_MENU,
        MUSIC_GAMEPLAY,
        // score -----------------------------------------------------------------------------------------
        //SCORE_POINTS_BASIC,
        // player -----------------------------------------------------------------------------------------
        //PLAYER_WALK,
        //PLAYER_TAKE_DAMAGE,
        //PLAYER_DEATH,
        // gun
        GUN_AR1_SHOT,
        //GUN_AR1_RELOAD,
        //GUN_AR2_SHOT,
        //GUN_AR2_RELOAD,
        // bullet -----------------------------------------------------------------------------------------
        //BULLET_AUTOFIRE_DESTROY,
        // enemy ------------------------------------------------------------------------------------------
        //CRAWLER_SPAWN,
        CRAWLER_BULLET_DAMAGE,
        CRAWLER_DEATH,
        // size / needed for code -------------------------------------------------------------------------
        SIZE
    }
    #endregion


    // ----------------------------------------------------------------------------------------------------


    public static SoundManager GetInstance { private set; get; }

    [System.Serializable]
    public class SoundClip
    {
        public ESoundClip m_ESound;
        public AudioClip m_AudioClip;
        public float m_SpecialBlend = 0.5f;
        public float m_MaxTriggerInterval = 0.0f;
        public bool m_Loop = false;
        [HideInInspector]
        public float m_PrevTime;
    }

    [Range(0.0f, 100.0f)]
    public float m_MasterVolume = 100.0f;
    public List<SoundClip> m_SoundClips;

    private GameObject m_AudioFolder;
    private float m_VolumeScaler;


    // ----------------------------------------------------------------------------------------------------


    public void PlaySoundClip(ESoundClip soundClipKey, Vector3 position, float startDelay = 0.0f, float dopplerLvl = 0.0f)
    {
        if (CanPlaySound(soundClipKey) == true)
        {
            if (startDelay != 0.0f)
            {
                startDelay = 44100.0f * startDelay; // seconds to Hz
            }

            GameObject go = new GameObject("Sound");
            go.transform.position = position;
            go.transform.parent = m_AudioFolder.transform;
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = m_SoundClips[(int)soundClipKey].m_AudioClip;
            source.spatialBlend = m_SoundClips[(int)soundClipKey].m_SpecialBlend;
            source.loop = m_SoundClips[(int)soundClipKey].m_Loop;
            source.volume = m_VolumeScaler;
            source.maxDistance = 100.0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.dopplerLevel = dopplerLvl;
            //source.PlayDelayed((ulong)startDelay);
            source.PlayOneShot(source.clip);
            Destroy(go, source.clip.length);
        }
    }


    private bool CanPlaySound(ESoundClip soundClipKey)
    {
        switch(soundClipKey)
        {
            //case ESoundClip.PLAYER_WALK:
            //    return TimeChecker(soundClipKey);
                //break;

            default: return true;
        }
    }


    private bool TimeChecker(ESoundClip soundClipKey)
    {
        for(int i = 0; i < (int)ESoundClip.SIZE; ++i)
        {
            if (m_SoundClips[i].m_ESound == soundClipKey)
            {
                if (m_SoundClips[i].m_MaxTriggerInterval > 0.0f)
                {
                    float localTimer = m_SoundClips[i].m_MaxTriggerInterval;
                    float prevTime = m_SoundClips[i].m_PrevTime;
                    float timeNow = Time.time;
                    if (timeNow > localTimer + prevTime)
                    {
                        m_SoundClips[i].m_PrevTime = Time.time;
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

        m_AudioFolder = new GameObject("AudioFolder");
        m_AudioFolder.transform.position = Vector3.zero;

        SoundClip[] tmp = m_SoundClips.ToArray();
        m_SoundClips.Clear();
        m_SoundClips.Capacity = (int)ESoundClip.SIZE;
        for (int i = 0; i < m_SoundClips.Capacity; ++i)
        {
            m_SoundClips.Add(null);
        }

        for(int i = 0; i < tmp.Length; ++i)
        {
            m_SoundClips[(int)tmp[i].m_ESound] = tmp[i];
        }

        m_VolumeScaler = m_MasterVolume / 100.0f;

        //PlaySoundClip(ESoundClip.MUSIC_GAMEPLAY, new Vector3(0.0f, 10.0f, 0.0f));
    }
}
