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
        CRAWLER_TAKE_DAMAGE,
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

    public SoundClip[] m_SoundClips;


    // ----------------------------------------------------------------------------------------------------


    public void PlaySoundClip(ESoundClip soundClipKey)
    {
        if(CanPlaySound(soundClipKey) == true)
        {
            GameObject go = new GameObject("PlayerSoundClip");
            AudioSource source = go.AddComponent<AudioSource>();
            source.PlayOneShot(m_SoundClips[(int)soundClipKey].m_AudioClip);
        }
    }


    private bool CanPlaySound(ESoundClip soundClipKey)
    {
        switch(soundClipKey)
        {
            case ESoundClip.PLAYER_WALK:
                return TimeChecker(soundClipKey);
                //break;

            default: return false;
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
    }
}
