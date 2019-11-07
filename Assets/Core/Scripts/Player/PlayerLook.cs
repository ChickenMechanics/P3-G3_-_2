﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLook : MonoBehaviour
{
    #region design vars
    public float m_EyeHeight = 0.5f;
    public float m_LookSensitivity = 4;
    [Range(0.0f, 1.0f)]
    public float m_LookSmooth = 0.4f;
    [Range(0.0f, 100.0f)]
    public float m_LookPitchMin = 98.0f;
    [Range(-100.0f, 0.0f)]
    public float m_LookPitchMax = -98.0f;
    #endregion

    private PlayerCtrl m_PlayerCtrlScr;
    private Transform m_tPlayerLook;
    private Transform m_tPlayerMove;
    private Camera m_MainCam;
    private Camera m_FPSCam;
    private GameObject m_PlayerEyePoint;
    private GameObject m_EyePoint;
    private Vector3 m_PrevPos;
    private Vector2 m_NextLookRotation;
    private Vector2 m_CurrentLookRotation;
    private float m_EyePointOffsetZ;


    // ----------------------------------------------------------------------------------------------------


    public Vector3 GetPlayerCapsuleRotDir()
    {
        return new Vector3(0.0f, m_CurrentLookRotation.x, 0.0f);
    }


    private void CameraSetup()
    {
        m_EyePointOffsetZ = 0.5f;

        if (m_MainCam == null)
        {
            m_MainCam = Camera.main;
            m_MainCam.nearClipPlane = 0.01f;
            m_MainCam.farClipPlane = 800.0f;
            m_MainCam.depth = -1.0f;

            m_MainCam.transform.rotation = m_tPlayerLook.transform.rotation;
            m_MainCam.transform.position = m_tPlayerLook.transform.position;
            m_MainCam.transform.SetParent(m_tPlayerLook.transform);
        }

        if (m_FPSCam == null)
        {
            m_FPSCam = transform.Find("Look").transform.Find("FPS Camera").GetComponent<Camera>();
            m_FPSCam.transform.rotation = m_tPlayerLook.transform.rotation;
            m_FPSCam.transform.position = m_tPlayerLook.transform.position;
            m_FPSCam.transform.SetParent(m_tPlayerLook.transform);
        }
    }


    private void Look()
    {
        Vector2 lookInput = m_PlayerCtrlScr.GetBasicInput.LookInput * m_LookSensitivity;
        m_NextLookRotation = Vector2.Lerp(m_NextLookRotation, lookInput, m_LookSmooth);
        m_CurrentLookRotation += m_NextLookRotation;

        if (m_CurrentLookRotation.x > 360.0f) m_CurrentLookRotation.x = 0.0f;
        else if (m_CurrentLookRotation.x < -360.0f) m_CurrentLookRotation.x = 0.0f;

        m_CurrentLookRotation.y = Mathf.Clamp(m_CurrentLookRotation.y, m_LookPitchMax, m_LookPitchMin);

        m_tPlayerLook.eulerAngles = new Vector3(0.0f, m_CurrentLookRotation.x, 0.0f);    // Rotate root point

        m_tPlayerLook.transform.localRotation = Quaternion.AngleAxis(m_CurrentLookRotation.y, Vector3.left);
        m_tPlayerLook.transform.localRotation *= Quaternion.AngleAxis(m_CurrentLookRotation.x, Vector3.up);
        m_tPlayerLook.transform.eulerAngles = new Vector3(-m_CurrentLookRotation.y, m_CurrentLookRotation.x, 0.0f); // fix rotation from bugging out
    }


    private void Awake()
    {
        // Scr
        m_PlayerCtrlScr = GetComponent<PlayerCtrl>();
        m_tPlayerLook = transform.Find("Look").transform;
        m_tPlayerMove = transform.Find("Move").transform;

        // Camera
        CameraSetup();

        m_NextLookRotation = Vector2.zero;
        m_CurrentLookRotation = Vector2.zero;

        // set start roatation by playermanager roatation
        m_CurrentLookRotation.x = PlayerManager.GetInstance.transform.rotation.eulerAngles.y;

        m_PrevPos = m_tPlayerMove.transform.position;
    }


    private void Update()
    {
        Vector3 posOffset = m_tPlayerLook.up * m_EyeHeight + m_tPlayerLook.forward * m_EyePointOffsetZ;
        Vector3 nxtPos = m_tPlayerMove.position + posOffset;

        m_tPlayerLook.position = Vector3.Lerp(m_PrevPos, nxtPos, Time.deltaTime);

        m_PrevPos = nxtPos;

        Look();


        //Vector3 posOffset = m_tPlayerLook.up * m_EyeHeight + m_tPlayerLook.forward * m_EyePointOffsetZ;

        //m_tPlayerLook.transform.position =
        //    Vector3.Lerp(m_tPlayerLook.transform.position,
        //    m_tPlayerMove.position + posOffset,
        //    0.9f);

        //Look();
    }
}
