using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateWalk : IState
{
    public P_StateWalk(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }

    private PlayerCtrl m_Owner;


    //----------------------------------------------------------------------------------------------------


    public void Enter()
    {
        //Debug.Log("Walk");
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
        m_Owner.UpdateDashInput();

        GunManager.GetInstance.Fire();
        GunManager.GetInstance.Reload();
        GunManager.GetInstance.ScrollWeapons();

        PlayerCtrl.BasicInput currentInput = m_Owner.GetBasicInput();
        if (currentInput.MoveInput.x == 0.0f && currentInput.MoveInput.z == 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.IDLE);
            return;
        }

        //if (currentInput.RunInput != 0.0f)
        //{
        //    m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.RUN);
        //    return;
        //}

        if (currentInput.DashInput != 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.DASH);
        }
    }


    public void Exit()
    {

    }
}
