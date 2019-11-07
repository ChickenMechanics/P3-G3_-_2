using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
        m_IsAvailable = true;
    }

    private PlayerCtrl m_Owner;
    private bool m_IsAvailable;


    //----------------------------------------------------------------------------------------------------


    public bool GetIsAvailable()
    {
        return m_IsAvailable;
    }


    public void SetIsAvailable(bool isAvailable)
    {
        m_IsAvailable = isAvailable;
    }


    public void Enter()
    {
        //Debug.Log("Idle");
        PlayerCtrl.BasicInput currentInput = m_Owner.GetBasicInput;
        currentInput.MoveInput.x = 0.0f;
        currentInput.MoveInput.z = 0.0f;
        currentInput.DashInput = 0.0f;
    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();

        GunManager.GetInstance.AdsHip();
        GunManager.GetInstance.Fire();
        GunManager.GetInstance.Reload();
        GunManager.GetInstance.ScrollWeapons();

        PlayerCtrl.BasicInput currentInput = m_Owner.GetBasicInput;
        if (currentInput.MoveInput.x != 0.0f || currentInput.MoveInput.z != 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.WALK);
        }
    }


    public void LateUpdate()
    {

    }


    public void Exit()
    {

    }
}
