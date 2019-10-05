using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    #region design vars
    public enum ESoundClip
    {
        // music ------------------------------------------------------------------------------------------
        MUSIC_MAIN_MENU,
        MUSIC_GAMEPLAY,
        // score -----------------------------------------------------------------------------------------
        SCORE_POINTS_BASIC,
        // player -----------------------------------------------------------------------------------------
        PLAYER_WALK,
        PLAYER_TAKE_DAMAGE,
        PLAYER_DEATH,
        // gun
        GUN_AUTOFIRE_SHOT,
        GUN_AUTOFIRE_RELOAD,
        // bullet -----------------------------------------------------------------------------------------
        BULLET_AUTOFIRE_DESTROY,
        // enemy ------------------------------------------------------------------------------------------
        CRAWLER_SPAWN,
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
        public float m_TimeTriggerThreshhold;
        [HideInInspector]
        public float m_PrevTime;
    }

    public List<SoundClip> m_SoundClips;
    private GameObject m_AudioFolder;


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
            go.transform.parent = m_AudioFolder.transform;
            go.transform.position = position;
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = m_SoundClips[(int)soundClipKey].m_AudioClip;
            source.maxDistance = 100.0f;
            source.spatialBlend = 1.0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.dopplerLevel = dopplerLvl;
            source.PlayDelayed((ulong)startDelay);

            Destroy(go, source.clip.length);
        }
    }


    private bool CanPlaySound(ESoundClip soundClipKey)
    {
        switch(soundClipKey)
        {
            case ESoundClip.PLAYER_WALK:
                return TimeChecker(soundClipKey);
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
                if (m_SoundClips[i].m_TimeTriggerThreshhold > 0.0f)
                {
                    float localTimer = m_SoundClips[i].m_TimeTriggerThreshhold;
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
        m_AudioFolder.transform.parent = transform;

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
    }
}
