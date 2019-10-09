using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour, IController
{
    public static GameManager GetInstance { private set; get; }

    private delegate void NowState();
    private NowState m_NowState;


    //----------------------------------------------------------------------------------------------------


    private static void MainMenuState()
    {

    }


    private static void MainOptionsState()
    {

    }


    private static void GameState()
    {

    }


    private static void EndState()
    {

    }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
        DontDestroyOnLoad(gameObject);

        m_NowState = MainMenuState;
    }


    private void Update()
    {
        m_NowState();
    }


    private void LateUpdate()
    {

    }
}
