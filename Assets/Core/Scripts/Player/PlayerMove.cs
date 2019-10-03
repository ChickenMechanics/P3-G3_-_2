using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    #region design vars
    [Header("Movement")]
    public float m_MoveAcceleration = 100.0f;
    public float m_MaxMoveSpeed = 10.0f;
    [HideInInspector]
    public float m_StrafeAcceleration = 100.0f;
    [HideInInspector]
    public float m_RunAcceleration = 500.0f;
    [HideInInspector]
    public float m_DashAcceleration = 1500.0f;
    [HideInInspector]
    public float m_DashTime = 1.0f;
    #endregion

    private PlayerCtrl m_PlayerCtrlScr;
    private PlayerLook m_PlayerLookScr;
    private GameObject m_MoveObj;
    private Rigidbody m_Rb;
    private PlayerCtrl.BasicInput m_Input;
    private Vector3 m_ForwardForce;
    private Vector3 m_StrafeForce;
    private Vector3 m_RunForce;
    private Vector3 m_DashForce;
    private Vector3 m_DashDir;
    private float m_ForwardAccel;
    private float m_StrafeAccel;
    private float m_RunAccel;
    private float m_DashAccel;
    private float m_AccelScaler;
    private int m_CurrentState;


    //----------------------------------------------------------------------------------------------------


    private void StateUpdate()
    {
        if(m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx != m_CurrentState)
        {
            // remove below conditional when all states are known as it wont be needed
            if (m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.IDLE ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.WALK ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.RUN ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.DASH)
            {
                m_CurrentState = m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx;

                m_ForwardForce = Vector3.zero;
                m_StrafeForce = Vector3.zero;
                m_RunForce = Vector3.zero;
                m_DashForce = Vector3.zero;

                m_DashDir = m_Input.MoveInput;
            }
        }
    }


    private void RunState()
    {
        switch (m_CurrentState)
        {
            case (int)PlayerCtrl.EPlayerState.IDLE: Idle(); break;
            case (int)PlayerCtrl.EPlayerState.WALK: Walk(); break;
            case (int)PlayerCtrl.EPlayerState.RUN:  Run();  break;
            case (int)PlayerCtrl.EPlayerState.DASH: Dash(); break;
            default: break;
        }
    }


    private void Idle()
    {
        //Debug.Log("Idle");
    }



    private void Walk()
    {
        m_ForwardForce = transform.forward;
        m_ForwardForce *= m_ForwardAccel * m_Input.MoveInput.z * Time.deltaTime;

        m_StrafeForce = transform.right;
        m_StrafeForce *= m_StrafeAccel * m_Input.MoveInput.x * Time.deltaTime;

        //Debug.Log("Walk");
    }


    private void Run()
    {
        m_RunForce = transform.forward;
        m_RunForce *= m_RunAccel * m_Input.MoveInput.z * Time.deltaTime;

        m_StrafeForce = transform.right;
        m_StrafeForce *= m_StrafeAccel * m_Input.MoveInput.x * Time.deltaTime;
    }


    private void Dash()
    {
        m_DashForce = m_DashDir;
        m_DashForce *= m_DashAccel * Time.deltaTime;

        //Debug.Log("Dash");
    }


    private void Awake()
    {
        m_MoveObj = transform.Find("Move").gameObject;
        m_Rb = m_MoveObj.GetComponent<Rigidbody>();
        m_PlayerCtrlScr = GetComponent<PlayerCtrl>();
        m_PlayerLookScr = GetComponent<PlayerLook>();

        m_Input = m_PlayerCtrlScr.GetBasicInput();

        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;
        m_DashForce = Vector3.zero;

        m_AccelScaler = 50.0f;
        m_ForwardAccel = m_MoveAcceleration * m_AccelScaler;
        m_StrafeAccel = m_StrafeAcceleration * m_AccelScaler;
        m_RunAccel = m_RunAcceleration * m_AccelScaler;
        m_DashAccel = m_DashAcceleration * m_AccelScaler;

        StateUpdate();
    }


    private void FixedUpdate()
    {
        // Rotation
        m_MoveObj.transform.eulerAngles = m_PlayerLookScr.GetPlayerCapsuleRotDir();

        // Position
        if (m_Input.MoveInput.x != 0.0f || m_Input.MoveInput.z != 0.0f)
        {
            if (m_Rb.velocity.magnitude < m_MaxMoveSpeed)
            {
                m_Rb.AddRelativeForce((m_ForwardForce + m_StrafeForce + m_RunForce + m_DashForce), ForceMode.Force);
            }
        }
    }


    private void LateUpdate()
    {
        m_Input = m_PlayerCtrlScr.GetBasicInput();
        if (m_Input.MoveInput.x != 0.0f && m_Input.MoveInput.z != 0.0f)
        {
            m_Input.MoveInput /= Mathf.Sqrt(m_Input.MoveInput.x * m_Input.MoveInput.x + m_Input.MoveInput.z * m_Input.MoveInput.z);
        }

        StateUpdate();
        RunState();
    }
}
