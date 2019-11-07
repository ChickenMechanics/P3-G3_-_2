using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateWalk : IState
{
    public P_StateWalk(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
        m_IsAvailable = true;

        m_FootSoundFreq = 0.35f;
        m_NowFootSoundTime = m_FootSoundFreq;
        m_TriggerFootSound = true;
    }

    private PlayerCtrl m_Owner;
    private bool m_IsAvailable;

    private float m_FootSoundFreq;
    private float m_NowFootSoundTime;
    private bool m_TriggerFootSound;


    //----------------------------------------------------------------------------------------------------

        
    private void FootStepSoundTimer()
    {
        m_NowFootSoundTime -= Time.deltaTime;
        if(m_NowFootSoundTime < 0.0f)
        {
            m_NowFootSoundTime = m_FootSoundFreq;
            m_TriggerFootSound = true;
        }
    }


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
        //Debug.Log("Walk");
    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {
        m_Owner.UpdateLookInput();
        m_Owner.UpdateMoveInput();
        m_Owner.UpdateDashInput();

        GunManager.GetInstance.AdsHip();
        GunManager.GetInstance.Fire();
        GunManager.GetInstance.Reload();
        GunManager.GetInstance.ScrollWeapons();

        if (m_TriggerFootSound == false)
        {
            FootStepSoundTimer();
        }
        else if (m_TriggerFootSound == true)
        {
            m_TriggerFootSound = false;
            if(PlayerManager.GetInstance.GetPlayerMoveScr.m_GetIsGrounded)
            {
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.PLAYER_FOOTSTEPS, m_Owner.transform.position);
            }
        }

        PlayerCtrl.BasicInput currentInput = m_Owner.GetBasicInput;
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
            if (m_Owner.GetFsm().GetState(PlayerCtrl.EPlayerState.DASH).GetIsAvailable() == true)
            {
                m_Owner.GetFsm().ChangeState(PlayerCtrl.EPlayerState.DASH);
            }
        }
    }


    public void LateUpdate()
    {

    }


    public void Exit()
    {

    }
}
