using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Model")]
    public Vector3 m_PositionOffset;

    [Header("Properties")]
    //public bool m_AutoFire;
    public int m_RoundsPerMinute;
    public int m_MagazineSize;

    [Header("Bullet Prefab")]
    public GameObject m_BulletModelPrefab;
    #endregion

    // Gun things
    private Transform m_BulletSpawnPoint;
    private LayerMask m_AimRayLayerMask;
    // Ammunition things
    private GameObject m_BulletFolder;
    private RaycastHit m_RaycastHit;
    private Vector3 m_SecretSpot;
    private float m_RayMaxDist;
    private float m_Rpm;
    private float m_TimePastSinceLastFire;
    private int m_CurrentMagSize;
    private bool m_IsReload;

    // Test
    private bool m_ADS;
    // Test


    //----------------------------------------------------------------------------------------------------


    public void InitGun()
    {
        m_BulletFolder = new GameObject("bullets");
        m_BulletFolder.transform.position = new Vector3(5.0f, -10.0f, 0.0f);

        m_RaycastHit = new RaycastHit();

        m_SecretSpot = new Vector3(0.0f, -10.0f, 0.0f);

        m_RayMaxDist = 1000.0f;
        m_Rpm = 60.0f / m_RoundsPerMinute;
        m_TimePastSinceLastFire = m_Rpm;

        m_BulletSpawnPoint = transform.GetChild(0);
        m_AimRayLayerMask = LayerMask.GetMask("Level_Ground", "Level_Wall", "Enemy");

        m_CurrentMagSize = m_MagazineSize;

        m_IsReload = false;
    }


    private void UpdateMagazine()
    {
        // Time is resetted? in fire
        if (m_TimePastSinceLastFire < m_Rpm)
        {
            m_TimePastSinceLastFire += Time.deltaTime;
        }

        if(m_CurrentMagSize <= 0.0f)
        {
            m_IsReload = true;
        }

        if(m_IsReload == true)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                m_CurrentMagSize = m_MagazineSize;
                m_IsReload = false;
            }
        }
    }


    private void UpdateTransform()
    {
        if(transform.parent != null)
        {
            Vector3 offsetPos = (transform.right * m_PositionOffset.x) +
                                (transform.up * m_PositionOffset.y) +
                                (transform.forward * m_PositionOffset.z);

            transform.position = transform.parent.transform.position + offsetPos;
        }
    }


    public void Fire(Transform cameraPoint)     // Is currently called from FixedUpdate(), so most code, except raycast, should be moved to Update()
    {
        if(m_IsReload != true)
        {
            if (m_TimePastSinceLastFire >= m_Rpm)
            {
                Ray ray = new Ray(cameraPoint.position, cameraPoint.forward);
                Vector3 raycastedDir = cameraPoint.forward;
                if (Physics.Raycast(ray, out m_RaycastHit, m_RayMaxDist, m_AimRayLayerMask))
                {
                    raycastedDir = (m_RaycastHit.point - m_BulletSpawnPoint.position).normalized;
                }

                Transform tForm = transform.GetChild(0).transform;
                Vector3 spawnPos = tForm.position;
                Quaternion spawnRot = tForm.rotation;

                GameObject bulletClone = Instantiate(m_BulletModelPrefab, spawnPos, spawnRot);
                BulletBehaviour bulletScr = bulletClone.GetComponent<BulletBehaviour>();

                bulletScr.InitBullet();
                bulletClone.SetActive(false);
                bulletClone.transform.SetParent(m_BulletFolder.transform);

                m_TimePastSinceLastFire = 0.0f;
                --m_CurrentMagSize;

                bulletScr.Fire(m_BulletSpawnPoint, raycastedDir);
            }
        }
    }


    private void OnEnable()
    {
        UpdateTransform();
    }


    private void OnDisable()
    {
        transform.position = m_SecretSpot;
    }


    private void Update()
    {
        UpdateMagazine();

        // Test
        if (Input.GetMouseButton(1) == true)
        {
            Vector3 forward = transform.parent.forward * 0.2f;
            Vector3 down = transform.parent.up * -0.4f;

            transform.position = Vector3.Lerp(transform.position, (transform.parent.position + down + forward), 0.6f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 54.0f, 0.3f);
        }

        if (Input.GetMouseButton(1) == false)
        {
            Vector3 offsetPos = (transform.right * m_PositionOffset.x) +
                                (transform.up * m_PositionOffset.y) +
                                (transform.forward * m_PositionOffset.z);

            transform.position = Vector3.Lerp(transform.position, transform.parent.transform.position + offsetPos, 0.2f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60.0f, 0.15f);
        }
        // Test
    }
}
