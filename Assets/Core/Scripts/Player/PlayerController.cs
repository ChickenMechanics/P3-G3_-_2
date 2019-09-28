using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    #region design vars
    [Header("Movement")]
    public float m_MoveAcceleration = 100.0f;
    public float m_MaxMoveSpeed = 10.0f;

    [Header("Look")]
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
    private Rigidbody m_Rb;
    private GameObject m_PlayerEyePoint;
    private GameObject m_EyePoint;

    private Vector3 m_MoveDir;
    private Vector3 m_ForwardForce;
    private Vector3 m_StrafeForce;

    private Vector2 m_NextLookRotation;
    private Vector2 m_CurrentLookRotation;

    private float m_ForwardAccel;
    private float m_StrafeAccel;
    private float m_AccelScaler;
    private float m_EyePointOffsetZ;

    private float m_CurrentAccel;

    // Lazy gun
    private GunHandler m_Gunhandler;
    private int m_CurrentGunIdx;


    void Awake()
    {
        // Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Camera
        CameraSetup();

        // Rb
        m_Rb = GetComponent<Rigidbody>();

        // Transform
        m_MoveDir = Vector3.zero;
        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;

        m_NextLookRotation = Vector2.zero;
        m_CurrentLookRotation = Vector2.zero;

        m_AccelScaler = 50.0f;
        m_ForwardAccel = m_MoveAcceleration * m_AccelScaler;
        m_StrafeAccel = m_MoveAcceleration * m_AccelScaler;

        // Temp gun
        m_Gunhandler = GetComponent<GunHandler>();
        m_Gunhandler.Init();
        m_CurrentGunIdx = m_Gunhandler.GetActiveGunIdx();
    }


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

        if(m_MainCam == null)
        {
            m_MainCam = Camera.main;
            m_MainCam.nearClipPlane = 0.01f;
            m_MainCam.farClipPlane = 800.0f;
            m_MainCam.depth = -1.0f;

            m_MainCam.transform.rotation = m_PlayerEyePoint.transform.rotation;
            m_MainCam.transform.position = m_PlayerEyePoint.transform.position;
            m_MainCam.transform.SetParent(m_PlayerEyePoint.transform);
        }

        if(m_FPSCam == null)
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


    private void FixedMove()
    {
        if (m_MoveDir.x != 0.0f || m_MoveDir.z != 0.0f)
        {
            if (m_Rb.velocity.magnitude < m_MaxMoveSpeed)
            {
                m_Rb.AddForce((m_ForwardForce + m_StrafeForce), ForceMode.Force);
            }
        }
    }


    private void  Move()
    {
        m_MoveDir.x = Input.GetAxisRaw("Horizontal");
        m_MoveDir.z = Input.GetAxisRaw("Vertical");

        if (m_MoveDir.x != 0.0f || m_MoveDir.z != 0.0f)
        {
            if (m_MoveDir.x != 0.0f && m_MoveDir.z != 0.0f)
            {
                m_MoveDir /= Mathf.Sqrt(m_MoveDir.x * m_MoveDir.x + m_MoveDir.z * m_MoveDir.z);
            }

            m_ForwardForce = transform.forward;
            m_ForwardForce *= m_ForwardAccel * m_MoveDir.z * Time.deltaTime;

            m_StrafeForce = transform.right;
            m_StrafeForce *= m_StrafeAccel * m_MoveDir.x * Time.deltaTime;
        }
    }


    private void OnEnable()
    {
        CameraSetup();
    }


    void Update()
    {
        Look();
        Move();

        // Temp gun
        float wheelDir = Input.GetAxisRaw("Mouse ScrollWheel");
        if (wheelDir != 0.0f)
        {
            if (wheelDir != -0.1f)
            {
                ++m_CurrentGunIdx;
                if (m_CurrentGunIdx > m_Gunhandler.GetNumOfGuns() - 1)
                    m_CurrentGunIdx = 0;
            }
            else
            {
                --m_CurrentGunIdx;
                if (m_CurrentGunIdx < 0)
                    m_CurrentGunIdx = m_Gunhandler.GetNumOfGuns() - 1;
            }

            m_Gunhandler.SetActiveGun(m_CurrentGunIdx);
        }

        if (Input.GetMouseButton(0))
        {
            m_Gunhandler.Fire(m_PlayerEyePoint.transform);
        }
    }


    private void FixedUpdate()
    {
        FixedMove();
    }


    // TODO: Move this to a better place
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            // Temp
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.MAIN_MENU);
            // Temp
        }
    }
}
