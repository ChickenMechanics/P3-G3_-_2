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
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                HUDManager.GetInstance.HighScoreEnable();
            }

            if (PlayerManager.GetInstance.GetIsAlive == false)
            {
                HUDManager.GetInstance.HighScoreEnable();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                PlayerManager.GetInstance.GetIsGod = !PlayerManager.GetInstance.GetIsGod;

                string msg = PlayerManager.GetInstance.GetIsGod ? "Godmode On" : "Godmode Off";
                Debug.LogWarning(msg);
            }
        }
    }
}
