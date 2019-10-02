using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    #region design vars
    [Header("Movement")]
    public float m_MoveAcceleration = 100.0f;
    [HideInInspector]
    public float m_StrafeAcceleration = 100.0f;
    public float m_DashAcceleration = 400.0f;
    public float m_MaxMoveSpeed = 10.0f;
    #endregion

    private PlayerCtrl m_PlayerCtrlScr;
    private PlayerLook m_PlayerLookScr;
    private GameObject m_MoveObj;
    private Rigidbody m_Rb;
    private Vector3 m_ForwardForce;
    private Vector3 m_StrafeForce;
    private Vector3 m_DashForce;
    private Vector3 m_CurrentInput;
    private float m_ForwardAccel;
    private float m_StrafeAccel;
    private float m_DashAccel;
    private float m_AccelScaler;

    private int m_CurrentState; // experimental


    //----------------------------------------------------------------------------------------------------


    private void StateUpdate()
    {
        if(m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx != m_CurrentState)
        {
            // remove below conditional when all states are known
            if (m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.IDLE ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.WALK)
            {
                m_CurrentState = m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx;

            }
        }
    }


    private void RunState()
    {
        switch (m_CurrentState)
        {
            case (int)PlayerCtrl.EPlayerState.IDLE: Idle(); break;
            case (int)PlayerCtrl.EPlayerState.WALK: Walk(); break;
            case (int)PlayerCtrl.EPlayerState.DASH: Dash(); break;
            default: break;
        }
    }


    private void Idle()
    {
        m_DashForce = Vector3.zero;
        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;
    }


    private void Walk()
    {
        m_DashForce = Vector3.zero;

        m_ForwardForce = transform.forward;
        m_ForwardForce *= m_ForwardAccel * m_CurrentInput.z * Time.deltaTime;

        m_StrafeForce = transform.right;
        m_StrafeForce *= m_StrafeAccel * m_CurrentInput.x * Time.deltaTime;
    }


    private void Dash()
    {
        m_DashForce = m_CurrentInput;
        m_DashForce *= m_DashAccel * Time.deltaTime;

        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;
    }


    private void Awake()
    {
        m_MoveObj = transform.Find("Move").gameObject;
        m_Rb = m_MoveObj.GetComponent<Rigidbody>();
        m_PlayerCtrlScr = GetComponent<PlayerCtrl>();
        m_PlayerLookScr = GetComponent<PlayerLook>();

        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;
        m_DashForce = Vector3.zero;
        m_CurrentInput = Vector3.zero;

        m_AccelScaler = 50.0f;
        m_ForwardAccel = m_MoveAcceleration * m_AccelScaler;
        m_StrafeAccel = m_StrafeAcceleration * m_AccelScaler;
        m_DashAccel = m_DashAcceleration * m_AccelScaler;

        StateUpdate();
    }


    private void FixedUpdate()
    {
        // Rotation
        m_MoveObj.transform.eulerAngles = m_PlayerLookScr.GetPlayerCapsuleRotDir();

        // Position
        if (m_CurrentInput.x != 0.0f || m_CurrentInput.z != 0.0f)
        {
            if (m_Rb.velocity.magnitude < m_MaxMoveSpeed)
            {
                m_Rb.AddRelativeForce((m_ForwardForce + m_StrafeForce), ForceMode.Force);
            }
        }
    }


    private void LateUpdate()
    {
        m_CurrentInput = m_PlayerCtrlScr.GetBasicInput().MoveInput;
        if (m_CurrentInput.x != 0.0f && m_CurrentInput.z != 0.0f)
        {
            m_CurrentInput /= Mathf.Sqrt(m_CurrentInput.x * m_CurrentInput.x + m_CurrentInput.z * m_CurrentInput.z);
        }

        StateUpdate();
        RunState();
    }
}
