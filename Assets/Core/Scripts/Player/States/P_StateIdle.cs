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


    //----------------------------------------------------------------------------------------------------


    public void Enter()
    {
        //Debug.Log("Idle");
    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {

    }


    public void LateUpdate()
    {
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();

        GunManager.GetInstance.Fire();
        GunManager.GetInstance.Reload();
        GunManager.GetInstance.ScrollWeapons();

        PlayerCtrl.BasicInput currentInput = m_Owner.GetBasicInput;
        if (currentInput.MoveInput.x != 0.0f || currentInput.MoveInput.z != 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.WALK);
        }
    }


    public void Exit()
    {

    }
}
