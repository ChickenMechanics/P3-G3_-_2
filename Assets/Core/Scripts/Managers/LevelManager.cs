using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Should be called SceneManager, but Unity uses that. Perhaps change this to something more descriptive like SceneLoaderManager or equally exciting
public class LevelManager : MonoBehaviour
{
    public static LevelManager GetInstance { get; private set;}

    private Animator m_Animator;
    private int m_NextSceneIdx;
    private int m_CurrentSceneIdx;


    // matches the order in build settings
    public enum EScene
    {
        MAIN = 1,
        OPTIONS,
        END,
        ARENA
    }


    public void ChangeScene(EScene scene)
    {
        if (m_CurrentSceneIdx == (int)scene)
        {
            Debug.LogError("LevelManager::ChangeScene(): Next scene index and current scene index are the same. No scene change made!");
            return;
        }

        m_NextSceneIdx = (int)scene;
        m_Animator.SetBool("FadeIn", false);
        m_Animator.SetBool("FadeOut", true);
    }

    
    public void FadeCompleteCallback()
    {
        SceneManager.LoadScene(m_NextSceneIdx);
        m_CurrentSceneIdx = m_NextSceneIdx;

        m_Animator.SetBool("FadeOut", false);
        m_Animator.SetBool("FadeIn", true);

        // TODO: Move this to game manager
        if (m_CurrentSceneIdx == (int)EScene.MAIN)
        {
            AudioSource source = SoundManager.GetInstance.GetAudioSourceByAlias("Main Menu Music 1");
            if(source == null)
            {
                SoundManager.GetInstance.DestroySoundClip("Combat Music");
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.MUSIC_MAIN_MENU, Vector3.zero);
            }
        }
        if (m_CurrentSceneIdx == (int)EScene.ARENA)
        {
            SoundManager.GetInstance.DestroySoundClip("Main Menu Music 1");
            SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.MUSIC_COMBAT, Vector3.zero);
        }
        //// unused scene delete or something
        //if (m_CurrentSceneIdx == (int)EScene.END)
        //{
        //    SoundManager.GetInstance.DestroySoundClip("Combat Music");

        //    AudioSource source = SoundManager.GetInstance.GetAudioSourceByAlias("Main Menu Music 1");
        //    if (source == null)
        //    {
        //        SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.MUSIC_MAIN_MENU, Vector3.zero);
        //    }
        //}
    }


    private void Init()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
        DontDestroyOnLoad(gameObject);

        DontDestroyOnLoad(transform.Find("FadeCanvas").gameObject);
        DontDestroyOnLoad(transform.Find("FadeCanvas").transform.Find("FadeImg").gameObject);

        m_Animator = GetComponent<Animator>();
        m_NextSceneIdx = -1;
        m_CurrentSceneIdx = -1;

        ChangeScene(EScene.MAIN);
    }


    private void Awake()
    {
        Init();
    }
}
