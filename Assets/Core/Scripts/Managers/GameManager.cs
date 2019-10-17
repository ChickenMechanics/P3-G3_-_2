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
        // TODO. Move and clean this to game manager when/if that is up
        if(PlayerManager.GetInstance != null)
        {
            if (PlayerManager.GetInstance.GetIsAlive == false &&
                PlayerManager.GetInstance.GetIsGod == false)
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

#if DEBUG
            if (Input.GetKeyDown(KeyCode.G))
            {
                PlayerManager.GetInstance.GetIsGod = !PlayerManager.GetInstance.GetIsGod;

                string msg = PlayerManager.GetInstance.GetIsGod ? "Godmode On" : "Godmode Off";
                Debug.LogError(msg);
            }
#endif
        }
    }
}
