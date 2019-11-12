using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuGUI : MonoBehaviour
{
    private Transform m_HowToPlayTransform;


    private void Start()
    {
        m_HowToPlayTransform = transform.Find("HowToPlayButton").gameObject.transform.Find("HowToPlayImg").gameObject.transform;
        m_HowToPlayTransform.gameObject.SetActive(false);
    }


    public void NewGame()
    {
        ScoreManager.GetInstance.ResetPlayerStats();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.ARENA);
    }


    public void Options()
    {
        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.OPTIONS);
    }


    public void HowToPlay()
    {
        if(m_HowToPlayTransform.gameObject.activeInHierarchy == false)
        {
            m_HowToPlayTransform.gameObject.SetActive(true);
        }
        else
        {
            m_HowToPlayTransform.gameObject.SetActive(false);
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
