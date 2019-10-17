using System.Collections;
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


    //private void Init()
    //{
    //    GetFsm = new FSM(this);
    //    GetFsm.
    //}


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
        if(PlayerManager.GetInstance != null)
        {
            PlayerManager playerManScr = PlayerManager.GetInstance.GetComponent<PlayerManager>();

            if (playerManScr.GetIsAlive == false &&
                playerManScr.GetIsUndead == false)
            {
                LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);

                if (ScoreManager.GetInstance != null)
                {
                    ScoreManager.GetInstance.ResetPlayerStats();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);

                if (ScoreManager.GetInstance != null)
                {
                    ScoreManager.GetInstance.ResetPlayerStats();
                }
            }
        }
    }


    //private void LateUpdate()
    //{

    //}
}
