using System.Collections;
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

    private Camera m_MainCam;
    private Camera m_FPSCam;
    private GameObject m_PlayerEyePoint;
    private GameObject m_EyePoint;

    private Vector2 m_NextLookRotation;
    private Vector2 m_CurrentLookRotation;

    private float m_EyePointOffsetZ;


    private void CameraSetup()
    {
        if (m_PlayerEyePoint == null)
        {
            m_EyePointOffsetZ = 0.4f;
            m_PlayerEyePoint = new GameObject("Camera Point");
            m_PlayerEyePoint.transform.rotation = transform.rotation;
            m_PlayerEyePoint.transform.position = transform.position + new Vector3(0.0f, m_EyeHeight, m_EyePointOffsetZ);
            m_PlayerEyePoint.transform.SetParent(transform);
        }

        if (m_MainCam == null)
        {
            m_MainCam = Camera.main;
            m_MainCam.nearClipPlane = 0.01f;
            m_MainCam.farClipPlane = 800.0f;
            m_MainCam.depth = -1.0f;

            m_MainCam.transform.rotation = m_PlayerEyePoint.transform.rotation;
            m_MainCam.transform.position = m_PlayerEyePoint.transform.position;
            m_MainCam.transform.SetParent(m_PlayerEyePoint.transform);
        }

        if (m_FPSCam == null)
        {
            m_FPSCam = GameObject.Find("FPS Camera").GetComponent<Camera>();
            m_FPSCam.transform.rotation = m_PlayerEyePoint.transform.rotation;
            m_FPSCam.transform.position = m_PlayerEyePoint.transform.position;
            m_FPSCam.transform.SetParent(m_PlayerEyePoint.transform);
        }
    }


    private void Look()
    {
        Vector2 mouseLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * m_LookSensitivity;  // X is yaw, Y is pitch
        m_NextLookRotation = Vector2.Lerp(m_NextLookRotation, mouseLook, m_LookSmooth);
        m_CurrentLookRotation += m_NextLookRotation;

        if (m_CurrentLookRotation.x > 360.0f) m_CurrentLookRotation.x = 0.0f;
        else if (m_CurrentLookRotation.x < -360.0f) m_CurrentLookRotation.x = 0.0f;

        m_CurrentLookRotation.y = Mathf.Clamp(m_CurrentLookRotation.y, m_LookPitchMax, m_LookPitchMin);

        // PlayerCapsule
        transform.eulerAngles = new Vector3(0.0f, m_CurrentLookRotation.x, 0.0f);

        // Camera // Works, but I don't really know...
        m_PlayerEyePoint.transform.localRotation = Quaternion.AngleAxis(-m_CurrentLookRotation.y, Vector3.right);
        m_PlayerEyePoint.transform.localRotation = Quaternion.AngleAxis(m_CurrentLookRotation.x, Vector3.up);
        m_PlayerEyePoint.transform.eulerAngles = new Vector3(-m_CurrentLookRotation.y, m_CurrentLookRotation.x, 0.0f);
    }


    private void Position()
    {

    }


    private void Awake()
    {
        // Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Camera
        CameraSetup();

        m_NextLookRotation = Vector2.zero;
        m_CurrentLookRotation = Vector2.zero;
    }


    private void LateUpdate()
    {
        Look();
    }
}
