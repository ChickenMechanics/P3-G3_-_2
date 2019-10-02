using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }

    public FSM m_Fsm { set; get; }
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
        Vector3 currentMoveInput = m_Owner.GetBasicInput().MoveInput;
        if (currentMoveInput.x != 0.0f || currentMoveInput.z != 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.WALK);
        }
    }


    public void LateUpdate()
    {
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();

        GunManager.GetInstance.Fire();
        GunManager.GetInstance.Reload();
        GunManager.GetInstance.ScrollWeapons();
    }


    public void Exit()
    {

    }
}
