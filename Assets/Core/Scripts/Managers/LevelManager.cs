using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Should be called SceneManager, but Unity uses that. Perhaps change this to something more descriptive like SceneLoaderManager or equally exciting
public class LevelManager : MonoBehaviour
{
    public static LevelManager GetInstance { get; private set;}

    private int m_NextSceneIdx;
    private int m_CurrentSceneIdx;


    public enum EScene
    {
        MAIN_MENU = 0,
        OPTIONS,
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
        //GameObject.FindGameObjectWithTag("SceneTransitionFade").GetComponent<Animator>().SetTrigger("FadeOut");

        // TODO: Fix the fading between scenes
        FadeCompleteCallback();
    }


    public void FadeCompleteCallback()
    {
        SceneManager.LoadScene(m_NextSceneIdx);
        m_CurrentSceneIdx = m_NextSceneIdx;
    }


    private void Init()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        DontDestroyOnLoad(gameObject);

        m_NextSceneIdx = -1;
        m_CurrentSceneIdx = -1;
    }


    private void Awake()
    {
        Init();
    }
}
