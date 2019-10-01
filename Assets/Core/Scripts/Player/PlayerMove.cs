using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region design vars
    [Header("Movement")]
    public float m_MoveAcceleration = 100.0f;
    public float m_MaxMoveSpeed = 10.0f;
    #endregion

    private PlayerCtrl m_PlayerCtrlScr;
    private GameObject m_MoveObj;
    private Rigidbody m_Rb;
    private Vector3 m_ForwardForce;
    private Vector3 m_StrafeForce;
    private float m_ForwardAccel;
    private float m_StrafeAccel;
    private float m_AccelScaler;


    private void Awake()
    {
        m_MoveObj = transform.Find("Move").gameObject;
        m_Rb = m_MoveObj.GetComponent<Rigidbody>();
        m_PlayerCtrlScr = GetComponent<PlayerCtrl>();

        m_AccelScaler = 50.0f;
        m_ForwardAccel = m_MoveAcceleration * m_AccelScaler;
        m_StrafeAccel = m_MoveAcceleration * m_AccelScaler;
    }


    private void FixedUpdate()
    {
        if (m_PlayerCtrlScr.GetBasicInput().MoveInput.x != 0.0f ||
            m_PlayerCtrlScr.GetBasicInput().MoveInput.z != 0.0f)
        {
            if (m_Rb.velocity.magnitude < m_MaxMoveSpeed)
            {
                m_Rb.AddForce((m_ForwardForce + m_StrafeForce), ForceMode.Force);
            }
        }
    }


    private void LateUpdate()
    {
        Vector3 currentInput = m_PlayerCtrlScr.GetBasicInput().MoveInput;

        if (m_PlayerCtrlScr.GetBasicInput().MoveInput.x != 0.0f ||
            m_PlayerCtrlScr.GetBasicInput().MoveInput.z != 0.0f)
        {
            if (currentInput.x != 0.0f && currentInput.z != 0.0f)
            {
                currentInput /= Mathf.Sqrt(currentInput.x * currentInput.x + currentInput.z * currentInput.z);
                
            }

            m_ForwardForce = transform.forward;
            m_ForwardForce *= m_ForwardAccel * currentInput.z * Time.deltaTime;

            m_StrafeForce = transform.right;
            m_StrafeForce *= m_StrafeAccel * currentInput.x * Time.deltaTime;
        }
    }
}
