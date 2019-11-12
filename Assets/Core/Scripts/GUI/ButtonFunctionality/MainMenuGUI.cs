using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuGUI : MonoBehaviour
{
    private Transform m_HowToPlayTransform;
    private Transform m_CreditsTransform;


    private void Start()
    {
        m_HowToPlayTransform = transform.Find("HowToPlayButton").gameObject.transform.Find("HowToPlayImg").gameObject.transform;
        m_HowToPlayTransform.gameObject.SetActive(false);

        m_CreditsTransform = transform.Find("CreditsButton").gameObject.transform.Find("CreditsImg").gameObject.transform;
        m_CreditsTransform.gameObject.SetActive(false);
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
        if (m_CreditsTransform.gameObject.activeInHierarchy == true)
        {
            m_CreditsTransform.gameObject.SetActive(false);
        }

        if (m_HowToPlayTransform.gameObject.activeInHierarchy == false)
        {
            m_HowToPlayTransform.gameObject.SetActive(true);
        }
        else
        {
            m_HowToPlayTransform.gameObject.SetActive(false);
        }
    }


    public void Credits()
    {
        if (m_HowToPlayTransform.gameObject.activeInHierarchy == true)
        {
            m_HowToPlayTransform.gameObject.SetActive(false);
        }

        if (m_CreditsTransform.gameObject.activeInHierarchy == false)
        {
            m_CreditsTransform.gameObject.SetActive(true);
        }
        else
        {
            m_CreditsTransform.gameObject.SetActive(false);
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
