using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateDash : IState
{
    public P_StateDash(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }

    private PlayerCtrl m_Owner;
    private float m_DashTime;


    //----------------------------------------------------------------------------------------------------


    public void Enter()
    {
        //Debug.Log("Dash");
        m_DashTime = PlayerManager.GetInstance.GetPlayerMoveScr.m_DashTime;
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

        GunManager.GetInstance.Fire();
        GunManager.GetInstance.Reload();
        GunManager.GetInstance.ScrollWeapons();

        m_DashTime -= Time.deltaTime;
        if(m_DashTime < 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.WALK);
        }
    }


    public void Exit()
    {

    }
}
