using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
        MAIN = 0,
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
        m_Animator.SetTrigger("FadeOut");
        m_Animator.ResetTrigger("FadeOutGate");
    }


    public void FadeCompleteCallback()
    {
        SceneManager.LoadScene(m_NextSceneIdx);
        m_CurrentSceneIdx = m_NextSceneIdx;
        m_Animator.SetTrigger("FadeOutGate");
        m_Animator.ResetTrigger("FadeOut");
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
    }


    private void Awake()
    {
        Init();
    }
}
