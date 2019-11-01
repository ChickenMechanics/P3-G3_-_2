using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    #region design vars
    [Header("Movement")]
    public float m_ForwardAcceleration;
    public float m_BackwardAcceleration;
    public float m_MaxForwardSpeed;
    public float m_MaxBacwardSpeed;
    [HideInInspector]
    public float m_StrafeAcceleration;
    [HideInInspector]
    public float m_RunAcceleration = 500.0f;
    public float m_DashAcceleration;
    [HideInInspector]
    public float m_DashActiveTime = 0.25f;
    public float m_DashCooldown;
    public float m_PlayerGravity;
    #endregion

    private PlayerCtrl m_PlayerCtrlScr;
    private PlayerLook m_PlayerLookScr;
    private GameObject m_MoveObj;
    private Rigidbody m_Rb;
    private PlayerCtrl.BasicInput m_Input;
    private Vector3 m_ForwardBackwardForce;
    private Vector3 m_StrafeForce;
    private Vector3 m_RunForce;
    private Vector3 m_DashForce;
    private Vector3 m_DashDir;
    private LayerMask m_GroundRayLayerMask;
    private Transform m_GroundRayTFormPoint;
    private float m_ForwardAccel;
    private float m_BackwardAccel;
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
            // remove below conditional when all states are known as it won't be needed
            if (m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.IDLE ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.WALK ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.RUN ||
                m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx == (int)PlayerCtrl.EPlayerState.DASH)
            {
                m_CurrentState = m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx;

                m_ForwardBackwardForce = Vector3.zero;
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
        m_ForwardBackwardForce = transform.forward;

        m_ForwardBackwardForce *= (m_Input.MoveInput.z > 0.0f) ? m_ForwardAccel : (m_BackwardAccel * -1.0f);
        m_ForwardBackwardForce *= Time.fixedDeltaTime;

        m_StrafeForce = transform.right;
        m_StrafeForce *= m_StrafeAccel * m_Input.MoveInput.x * Time.fixedDeltaTime;

        //Debug.Log("Walk");
    }


    private void Run()
    {
        m_RunForce = transform.forward;
        m_RunForce *= m_RunAccel * m_Input.MoveInput.z * Time.fixedDeltaTime;

        m_StrafeForce = transform.right;
        m_StrafeForce *= m_StrafeAccel * m_Input.MoveInput.x * Time.fixedDeltaTime;
    }


    private void Dash()
    {
        m_DashForce = m_DashDir * m_DashAccel * Time.fixedDeltaTime;

        //Debug.Log("Dash");
    }


    private void Awake()
    {
        m_MoveObj = transform.Find("Move").gameObject;
        m_Rb = m_MoveObj.GetComponent<Rigidbody>();
        m_PlayerCtrlScr = GetComponent<PlayerCtrl>();
        m_PlayerLookScr = GetComponent<PlayerLook>();

        m_Input = m_PlayerCtrlScr.GetBasicInput;

        m_ForwardBackwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;
        m_DashForce = Vector3.zero;

        m_AccelScaler = 50.0f;
        m_ForwardAccel = m_ForwardAcceleration * m_AccelScaler;
        m_BackwardAccel = m_BackwardAcceleration * m_AccelScaler;
        //m_StrafeAccel = m_StrafeAcceleration * m_AccelScaler;
        m_StrafeAccel = m_ForwardAcceleration * m_AccelScaler;
        m_RunAccel = m_RunAcceleration * m_AccelScaler;
        m_DashAccel = m_DashAcceleration * m_AccelScaler;

        m_GroundRayLayerMask = LayerMask.GetMask("Level_Ground");
        m_GroundRayTFormPoint = transform.GetChild(1);

        StateUpdate();
    }


    private void FixedUpdate()
    {
        // Rotation
        m_MoveObj.transform.eulerAngles = m_PlayerLookScr.GetPlayerCapsuleRotDir();

        // Position
        if (m_Input.MoveInput.x != 0.0f || m_Input.MoveInput.z != 0.0f)
        {
            if(m_Input.MoveInput.z > 0.0f)
            {
                if (m_Rb.velocity.magnitude < m_MaxForwardSpeed)
                {
                    m_Rb.AddRelativeForce((m_ForwardBackwardForce + m_StrafeForce + m_RunForce + m_DashForce), ForceMode.Force);
                }
            }
            else if(m_Input.MoveInput.z < 0.0f)
            {
                if (m_Rb.velocity.magnitude < m_MaxBacwardSpeed)
                {
                    m_Rb.AddRelativeForce((m_ForwardBackwardForce + m_StrafeForce + m_RunForce + m_DashForce), ForceMode.Force);
                }
            }
            else
            {
                if (m_Rb.velocity.magnitude < m_MaxForwardSpeed)
                {
                    m_Rb.AddRelativeForce((m_StrafeForce + m_RunForce + m_DashForce), ForceMode.Force);
                }
            }
        }

        // bs gravity, seems to work but eh...
        if(m_PlayerCtrlScr.GetFSM.GetCurrentStateIdx != (int)PlayerCtrl.EPlayerState.DASH)
        {
            Ray ray = new Ray(m_GroundRayTFormPoint.position + new Vector3(0.0f, -1.0f, 0.0f), -m_GroundRayTFormPoint.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000.0f, m_GroundRayLayerMask))
            {
                if (hit.distance > 0.1f)
                {
                    m_Rb.AddRelativeForce(new Vector3(0.0f, m_PlayerGravity, 0.0f), ForceMode.Force);
                }
            }
        }

        //Debug.DrawLine(ray.origin, ray.origin + (ray.direction * 1000.0f), Color.green);
    }


    private void Update()
    {
        m_Input = m_PlayerCtrlScr.GetBasicInput;
        if (m_Input.MoveInput.x != 0.0f && m_Input.MoveInput.z != 0.0f)
        {
            m_Input.MoveInput /= Mathf.Sqrt(m_Input.MoveInput.x * m_Input.MoveInput.x + m_Input.MoveInput.z * m_Input.MoveInput.z);
        }

        StateUpdate();
        RunState();
    }
}
