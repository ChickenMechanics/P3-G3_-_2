using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Model")]
    public Vector3 m_PositionOffset;

    [Header("Properties")]
    public int m_RoundsPerMinute = 500;
    public int m_MagazineSize = 30;
    public float m_ReloadTimeInSec = 1.0f;

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
    [HideInInspector]
    public float GetCurrentReloadTime;
    [HideInInspector]
    public int GetCurrentMagSize { private set; get; }
    private bool m_IsFiring;
    [HideInInspector]
    public bool GetIsReloading { private set; get; }

    private EGunState m_CurrentGunState;
    private Transform m_CameraPoint;

    // Test
    private bool m_bADS;
    // Test

    private enum EGunState
    {
        READY = 0,
        SHOOTING,
        RELOADING,
        SIZE
    }


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

        GetCurrentReloadTime = m_ReloadTimeInSec;
        GetCurrentMagSize = m_MagazineSize;

        m_IsFiring = false;
        GetIsReloading = false;

        m_CurrentGunState = EGunState.READY;
        m_CameraPoint = null;
    }


    private void GunState()
    {
        switch ((int)m_CurrentGunState)
        {
            case (int)EGunState.READY:      GunReady();     break;
            case (int)EGunState.SHOOTING:   GunFiring();    break;
            case (int)EGunState.RELOADING:  GunReloading(); break;
        }
    }


    private void GunReady()
    {
        if (m_IsFiring == true)
        {
            m_CurrentGunState = EGunState.SHOOTING;

            m_IsFiring = false;
        }
    }


    private void GunFiring()
    {
        if (m_TimePastSinceLastFire >= m_Rpm)
        {
            Ray ray = new Ray(m_CameraPoint.position, m_CameraPoint.forward);
            Vector3 raycastedDir = m_CameraPoint.forward;
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
            --GetCurrentMagSize;
            m_IsFiring = false;

            bulletScr.Fire(m_BulletSpawnPoint, raycastedDir);
            m_CurrentGunState = EGunState.READY;
        }
    }


    private void GunReloading()
    {
        if (GetCurrentReloadTime < 0.0f)
        {
            GetIsReloading = false;
            m_TimePastSinceLastFire = 0.0f;
            GetCurrentMagSize = m_MagazineSize;
            GetCurrentReloadTime = m_ReloadTimeInSec;
            m_CurrentGunState = EGunState.READY;
        }

        //if(Input.GetMouseButton(0))
        //{
        //    m_TimePastSinceLastFire = 0.0f;
        //    m_IsFiring = false;

        //    m_IsReloading = false;
        //    GetCurrentReloadTime = m_ReloadTimeInSec;
        //    m_CurrentGunState = EGunState.READY;
        //}
    }


    private void UpdateMagazine()
    {
        // Rate of fire
        if (m_TimePastSinceLastFire < m_Rpm)
        {
            m_TimePastSinceLastFire += Time.deltaTime;
        }

        // Empty clip
        if (GetCurrentMagSize <= 0.0f)
        {
            Reload();
        }

        // Reloading time
        if(GetIsReloading == true)
        {
            GetCurrentReloadTime -= Time.deltaTime;  // Put this here because C# switches
        }
    }


    public void Fire(Transform cameraPoint)
    {
        if (GetIsReloading == false)
        {
            m_IsFiring = true;
            m_CameraPoint = cameraPoint;

            // TODO: Fix this cheat for a recoil
            Vector3 lastPos = GunManager.GetInstance.ActiveGun.transform.position;
            Vector3 nextpos = new Vector3(
                Random.Range(lastPos.x - 0.002f, lastPos.x + 0.002f),
                Random.Range(lastPos.y - 0.002f, lastPos.y + 0.002f),
                Random.Range(lastPos.z - 0.02f, lastPos.z + 0.02f));

            GunManager.GetInstance.ActiveGun.transform.position = Vector3.Lerp(lastPos, nextpos, 0.75f);
        }
    }


    public void Reload()
    {
        if(GetIsReloading == false)
        {
            if(GetCurrentMagSize < m_MagazineSize)
            {
                GetIsReloading = true;
                GetCurrentReloadTime = m_ReloadTimeInSec;
                GetCurrentMagSize = 0;
                m_CurrentGunState = EGunState.RELOADING;
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
        GunState();
        UpdateMagazine();

        // Test ADS
        {
            if (Input.GetMouseButton(1) == true && GetIsReloading == false)
            {
                Vector3 forward = transform.parent.forward * 0.2f;
                Vector3 down = transform.parent.up * -0.4f;

                transform.position = Vector3.Lerp(transform.position, (transform.parent.position + down + forward), 0.6f);
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 54.0f, 0.3f);

                transform.parent.transform.Find("Canvas").transform.Find("CrosshairImage").gameObject.SetActive(false);
            }
        }

        if (Input.GetMouseButton(1) == false || GetIsReloading == true)
        {
            Vector3 offsetPos = (transform.right * m_PositionOffset.x) +
                                (transform.up * m_PositionOffset.y) +
                                (transform.forward * m_PositionOffset.z);

            transform.position = Vector3.Lerp(transform.position, transform.parent.transform.position + offsetPos, 0.2f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60.0f, 0.1f);

            transform.parent.transform.Find("Canvas").transform.Find("CrosshairImage").gameObject.SetActive(true);
        }
        // Test ADS
    }
}
