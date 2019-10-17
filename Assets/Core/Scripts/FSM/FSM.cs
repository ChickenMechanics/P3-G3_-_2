using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSM
{
    private List<IState> m_States;
    public IController GetOwner { private set; get; }
    public IState GetCurrentState { private set; get; }
    public int GetCurrentStateIdx { private set; get; }


    //----------------------------------------------------------------------------------------------------
    // Create fsm, add states, call init. Then, call the updates you need.
    //----------------------------------------------------------------------------------------------------


    public void Init()
    {
        GetCurrentState.Enter();
    }


    public FSM(IController owner)
    {
        GetOwner = owner;
        m_States = new List<IState>();
    }


    public void AddState(IState state)
    {
        m_States.Add(state);
        GetCurrentState = m_States[0];
    }


    public IState GetState(PlayerCtrl.EPlayerState state)
    {
        if(m_States[(int)state] == null)
        {
            return null;
        }

        return m_States[(int)state];
    }


    public void ChangeState(PlayerCtrl.EPlayerState state)
    {
        GetCurrentState.Exit();
        GetCurrentStateIdx = (int)state;
        GetCurrentState = m_States[GetCurrentStateIdx];
        GetCurrentState.Enter();
    }


    public void FixedUpdate()
    {
        if (GetCurrentState != null)
        {
            GetCurrentState.FixedUpdate();
        }
    }


    public void Update()
    {
        if (GetCurrentState != null)
        {
            GetCurrentState.Update();
        }
    }


    public void LateUpdate()
    {
        GetCurrentState.LateUpdate();
    }
}

