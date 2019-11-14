﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour, IController
{
    public static GameManager GetInstance { private set; get; }

    public FSM GetFsm { private set; get; }

    private enum EGameState
    {
        INIT,
        LOADING,
        MAIN,
        OPTIONS,
        ARENA,
        SIZE
    }
    private EGameState m_NowState;


    //----------------------------------------------------------------------------------------------------


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
        DontDestroyOnLoad(gameObject);

        m_NowState = EGameState.INIT;
    }


    private void Update()
    {
        if (PlayerManager.GetInstance != null)
        {
            //#if DEBUG
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                PlayerManager.GetInstance.GetIsAlive = false;
                HUDManager.GetInstance.HighScoreArenaEnable();
            }
            //#endif

            if (PlayerManager.GetInstance.GetIsAlive == false &&
                HUDManager.GetInstance.m_bDisplayHighScore == false)
            {
                HUDManager.GetInstance.HighScoreArenaEnable();
            }

            if (Input.GetKeyDown(KeyCode.G) &&
                PlayerManager.GetInstance.GetIsAlive == true)
            {
                PlayerManager.GetInstance.GetIsGod = !PlayerManager.GetInstance.GetIsGod;

                string msg = PlayerManager.GetInstance.GetIsGod ? "Godmode On" : "Godmode Off";
                Debug.LogWarning(msg);
            }
        }
    }
}
