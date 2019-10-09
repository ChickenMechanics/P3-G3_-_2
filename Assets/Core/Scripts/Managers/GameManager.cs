using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour, IController
{
    public static GameManager GetInstance { private set; get; }

    private FSM m_Fsm;


    //----------------------------------------------------------------------------------------------------


    private void CreateStates()
    {
        m_Fsm = new FSM(this);
        m_Fsm.AddState(new G_StateMainMenu(this));
    }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
        DontDestroyOnLoad(gameObject);

        CreateStates();
    }


    private void Update()
    {
        m_Fsm.Update();
    }


    private void LateUpdate()
    {
        m_Fsm.LateUpdate();
    }
}
