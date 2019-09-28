using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateWalk : IState
{
    private PlayerCtrl m_Owner;


    public P_StateWalk(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }


    public void Enter()
    {
        //Debug.Log("Walk");

        //Debug.Log(m_Owner.GetMoveInput());
    }


    public void FixedUpdate()
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update()
    {
        if (m_Owner.GetMoveInput().x == 0.0f &&
            m_Owner.GetMoveInput().y == 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.IDLE);
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