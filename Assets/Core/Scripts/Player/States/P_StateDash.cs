using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateDash : IState
{
    public P_StateDash(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
        m_DashTimePassed = 0.0f;
        m_DashCooldownTimePassed = 0.0f;
        m_FovDecreaseTarget = 40.0f;
        m_FovIncreaseTarget = 60.0f;
        m_DashDecreaseLerp = 0.5f;
        m_DashIncreaseLerp = 0.05f;
        m_IsAvailable = true;
    }

    private PlayerCtrl m_Owner;
    private float m_DashTimePassed;
    private float m_DashCooldownTimePassed;
    private float m_FovDecreaseTarget;
    private float m_FovIncreaseTarget;
    private float m_DashDecreaseLerp;
    private float m_DashIncreaseLerp;
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
        //Debug.Log("Dash");
        m_DashTimePassed = PlayerManager.GetInstance.GetPlayerMoveScr.m_DashActiveTime;
        m_DashCooldownTimePassed = PlayerManager.GetInstance.GetPlayerMoveScr.m_DashCooldown;

        SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.PLAYER_DASH, m_Owner.transform.position);
    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {
        m_Owner.UpdateLookInput();

        //GunManager.GetInstance.Fire();
        //GunManager.GetInstance.Reload();
        //GunManager.GetInstance.ScrollWeapons();

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, m_FovDecreaseTarget, m_DashDecreaseLerp);

        m_DashTimePassed -= Time.deltaTime;
        if(m_DashTimePassed < 0.0f)
        {
            m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.WALK);
        }
    }


    public void LateUpdate()
    {

    }


    public void Exit()
    {
        m_IsAvailable = false;
        m_Owner.TriggerDashCooldown(m_DashCooldownTimePassed);
        m_Owner.TriggerDashFovDecrease(m_FovIncreaseTarget, m_DashIncreaseLerp);
        m_Owner.GetBasicInput.DashInput = 0.0f;
    }
}
