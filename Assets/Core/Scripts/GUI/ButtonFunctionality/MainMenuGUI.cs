﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuGUI : MonoBehaviour
{
    private Transform m_HighScoreTransform;
    private Transform m_HowToPlayTransform;
    private Transform m_CreditsTransform;


    private void Start()
    {
        m_HighScoreTransform = transform.Find("HighScoreButton").gameObject.transform.Find("HighScoreImg").gameObject.transform;
        m_HighScoreTransform.gameObject.SetActive(false);

        m_HowToPlayTransform = transform.Find("HowToPlayButton").gameObject.transform.Find("HowToPlayImg").gameObject.transform;
        m_HowToPlayTransform.gameObject.SetActive(false);

        m_CreditsTransform = transform.Find("CreditsButton").gameObject.transform.Find("CreditsImg").gameObject.transform;
        m_CreditsTransform.gameObject.SetActive(false);
    }


    private void ButtonSound()
    {
        SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.MENU_BUTTON_CLICK, transform.position);
    }


    public void NewGame()
    {
        ButtonSound();

        ScoreManager.GetInstance.ResetPlayerStats();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.ARENA);
    }


    public void HighScore()
    {
        ButtonSound();

        if (m_HowToPlayTransform.gameObject.activeInHierarchy == true)
        {
            m_HowToPlayTransform.gameObject.SetActive(false);
        }
        if (m_CreditsTransform.gameObject.activeInHierarchy == true)
        {
            m_CreditsTransform.gameObject.SetActive(false);
        }

        if (m_HighScoreTransform.gameObject.activeInHierarchy == false)
        {
            m_HighScoreTransform.gameObject.SetActive(true);
        }
        else
        {
            m_HighScoreTransform.gameObject.SetActive(false);
        }
    }


    //public void Options()
    //{
    //    LevelManager.GetInstance.ChangeScene(LevelManager.EScene.OPTIONS);
    //}


    public void HowToPlay()
    {
        ButtonSound();

        if (m_HighScoreTransform.gameObject.activeInHierarchy == true)
        {
            m_HighScoreTransform.gameObject.SetActive(false);
        }
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
        ButtonSound();

        if (m_HighScoreTransform.gameObject.activeInHierarchy == true)
        {
            m_HighScoreTransform.gameObject.SetActive(false);
        }
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
        ButtonSound();

        Application.Quit();
    }
}
