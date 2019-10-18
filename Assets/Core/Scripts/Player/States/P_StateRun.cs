using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateRun : IState
{
    public P_StateRun(IController controller)
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
        //Debug.Log("Run");
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
        if (currentInput.RunInput == 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.WALK);
        }
    }


    public void Exit()
    {

    }
}
