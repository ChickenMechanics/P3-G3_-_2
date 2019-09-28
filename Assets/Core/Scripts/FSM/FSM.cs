using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSM
{
    private IState m_CurrentState;


    public FSM(IState initState)
    {
        m_CurrentState = initState;
        m_CurrentState.Enter();
    }


    public void ChangeState(IState state)
    {
        m_CurrentState.Exit();
        m_CurrentState = state;
        m_CurrentState.Enter();
    }


    public void Update()
    {
        if(m_CurrentState != null)
        {
            m_CurrentState.Update();
        }
    }


    public void FixedUpdate()
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.FixedUpdate();
        }
    }
}

