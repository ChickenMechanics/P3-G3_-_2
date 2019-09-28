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
        //Debug.Log("Idle");

        //Debug.Log(m_Owner.GetMoveInput());
    }


    public void FixedUpdate()
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update()
    {
        if (m_Owner.GetMoveInput().x != 0.0f ||
            m_Owner.GetMoveInput().y != 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.WALK);
            m_Owner.GetFsm().ChangeState(state);
        }

        //UpdateIdle(dT);

        //m_Owner.IsGrounded();
        //m_Owner.UpdateGrfxRot();
        //m_Owner.UpdateMoveDir();
    }


    public void Exit()
    {

    }
}
