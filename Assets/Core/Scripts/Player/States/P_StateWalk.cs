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

    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();

        Vector3 currentMoveInput = m_Owner.GetBasicInput().MoveInput;
        if (currentMoveInput.x == 0.0f && currentMoveInput.z == 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.IDLE);
            m_Owner.GetFsm().ChangeState(state);
        }


        Debug.Log("Walk");
    }


    public void Exit()
    {

    }
}