using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    private PlayerCtrl m_Owner;


    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }


    public void Enter()
    {
        Debug.Log("Idle");
    }


    public void FixedUpdate()
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update()
    {
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();

        Vector3 currentMoveInput = m_Owner.GetBasicInput().MoveInput;
        if (currentMoveInput.x != 0.0f || currentMoveInput.z != 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.WALK);
            m_Owner.GetFsm().ChangeState(state);
        }
    }


    public void Exit()
    {
        Debug.Log("Idle");
    }
}
