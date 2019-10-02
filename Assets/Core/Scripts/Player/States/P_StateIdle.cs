using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }

    private PlayerCtrl m_Owner;


    public void Enter()
    {
        //Debug.Log("Idle");
    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {
        // rot and pos
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();

        // fire
        if (Input.GetMouseButton(0))
        {
            GunManager.GetInstance.Fire();
        }

        // reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            GunManager.GetInstance.Reload();
        }

        // state change
        Vector3 currentMoveInput = m_Owner.GetBasicInput().MoveInput;
        if (currentMoveInput.x != 0.0f || currentMoveInput.z != 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.WALK);
            m_Owner.GetFsm().ChangeState(state);
        }
    }


    


    public void Exit()
    {
        //Debug.Log("Idle");
    }
}
