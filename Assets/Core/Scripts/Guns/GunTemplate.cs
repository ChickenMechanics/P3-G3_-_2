﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Model")]
    public string m_GunName;
    public Vector3 m_PositionOffset;

    [Header("Properties")]
    public int m_RoundsPerMinute = 500;
    public int m_MagSizeTotal = 30;
    [Range(0.0f, 0.5f)]
    public float m_AdsSpread = 0.01f;
    [Range(0.0f, 0.5f)]
    public float m_HipSpread = 0.04f;
    [Header("MuzzleFlash Vfx")]
    public GameObject m_MuzzleFlashVfx;
    [Header("Sound")]
    public SoundManager.ESoundClip m_EFiringSound;

    [Header("Bullet Prefab")]
    public GameObject m_BulletModelPrefab;
    #endregion

    // gun things
    private Transform m_BulletSpawnPoint;
    private Transform m_RayOriginPoint;
    private LayerMask m_AimRayLayerMask;
    // animation
    [HideInInspector] public Animator m_GetAnimator { private set; get; }
    // ammunition things
    private GameObject m_BulletFolder;
    private RaycastHit m_RaycastHit;
    private Vector3 m_SecretSpot;
    private float m_RayMaxDist;
    private float m_Rpm;
    private float m_TimePastSinceLastFire;
    [HideInInspector] public float GetCurrentReloadTime;
    [HideInInspector] public int GetCurrentMagSize { private set; get; }
    private bool m_IsFiring;
    [HideInInspector] public bool GetIsReloading { private set; get; }
    [HideInInspector] public bool GetIsADS { private set; get; }

    private EGunState m_CurrentGunState;
    private Transform m_CameraPoint;
    private Vector2 m_BulletSpreadDirs;
    private GameObject m_CrossHairObj;
    private GameObject m_MuzzleFlash;
    private Transform m_MuzzlePoint;
    private float m_ReloadAnimTime;
    [HideInInspector] public bool m_IsPaused { set; get; }

    // greande delay to sync with animation
    private float m_GrenadeDelayTargetT;
    private float m_NowGrenadeDelayT;

    private enum EGunState
    {
        READY = 0,
        FIRING,
        RELOADING,
        SIZE
    }


    //----------------------------------------------------------------------------------------------------


    public void InitGun()
    {
        m_BulletFolder = new GameObject("BulletFolder");
        m_BulletFolder.transform.parent = GunManager.GetInstance.transform;
        m_BulletFolder.transform.position = new Vector3(5.0f, -10.0f, 0.0f);

        m_RaycastHit = new RaycastHit();

        m_SecretSpot = new Vector3(0.0f, -10.0f, 0.0f);

        m_RayMaxDist = 10000.0f;
        m_Rpm = 60.0f / m_RoundsPerMinute;
        m_TimePastSinceLastFire = m_Rpm;

        m_BulletSpawnPoint = transform.GetChild(0);
        m_RayOriginPoint = transform.GetChild(1);
        m_AimRayLayerMask = LayerMask.GetMask("Level_Ground", "Level_Wall", "Enemy");

        m_GetAnimator = transform.GetChild(3).gameObject.GetComponent<Animator>();
        m_ReloadAnimTime = m_GetAnimator.runtimeAnimatorController.animationClips[0].length;

        GetCurrentReloadTime = m_ReloadAnimTime;
        GetCurrentMagSize = m_MagSizeTotal;

        if (m_MuzzleFlashVfx != null)
        {
            m_MuzzlePoint = transform.GetChild(2);
            m_MuzzleFlash = Instantiate(m_MuzzleFlashVfx, m_MuzzlePoint.position, Quaternion.identity, m_MuzzlePoint.transform);
            m_MuzzleFlash.GetComponent<ParticleSystem>().Stop();
        }

        m_IsFiring = false;
        GetIsReloading = false;
        GetIsADS = false;

        m_CurrentGunState = EGunState.READY;
        m_CameraPoint = null;
        m_BulletSpreadDirs = Vector2.zero;
        m_CrossHairObj = transform.parent.transform.Find("Canvas").transform.Find("CrosshairImage").gameObject;

        m_IsPaused = false;

        if(m_GunName == "Grenade")
        {
            m_GrenadeDelayTargetT = 0.35f;
            m_NowGrenadeDelayT = m_GrenadeDelayTargetT;
        }
    }


    private void GunStateUpdate()
    {
        switch ((int)m_CurrentGunState)
        {
            case (int)EGunState.READY:      GunReadyState();     break;
            case (int)EGunState.FIRING:     GunFiringState();    break;
            case (int)EGunState.RELOADING:  GunReloadingState(); break;
        }
    }


    private void GunReadyState()
    {
        if(m_GetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fire") ||
            m_GetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Overheat"))
        {
            m_GetAnimator.SetBool("Fire", false);
            m_GetAnimator.SetBool("Overheat", false);
            m_GetAnimator.SetBool("Idle", true);
        }

        if (m_IsFiring == true)
        {
            m_GetAnimator.SetBool("Idle", false);
            m_GetAnimator.SetBool("Overheat", false);
            m_GetAnimator.SetBool("Fire", true);

            // delay for grenade shot so it syncs with animation
            if (m_GunName == "Grenade")
            {
                m_NowGrenadeDelayT -= Time.deltaTime;
                if (m_NowGrenadeDelayT < 0.0f)
                {
                    m_NowGrenadeDelayT = m_GrenadeDelayTargetT;
                    m_CurrentGunState = EGunState.FIRING;
                }
            }
            else
            {
                m_CurrentGunState = EGunState.FIRING;
            }
        }
    }


    private void GunFiringState()
    {
        if (m_TimePastSinceLastFire >= m_Rpm)
        {
            Ray ray = new Ray(m_RayOriginPoint.position, m_RayOriginPoint.forward);
            Vector3 raycastedDir = m_RayOriginPoint.forward;

             if (Physics.Raycast(ray, out m_RaycastHit, m_RayMaxDist, m_AimRayLayerMask))
            {
                if(m_RaycastHit.distance > 2.0f)    // some protection from keeping the bullet model rotation to spawn at super akward angles when bullet target point is very close to bullet spawn point... eh
                {
                    raycastedDir = (m_RaycastHit.point - m_BulletSpawnPoint.position).normalized;
                }
            }

            Transform tForm = transform.GetChild(0).transform;
            Vector3 spawnPos = tForm.position;
            Quaternion spawnRot = tForm.rotation;

            GameObject bulletClone = Instantiate(m_BulletModelPrefab, spawnPos, spawnRot);
            bulletClone.SetActive(false);
            bulletClone.transform.SetParent(m_BulletFolder.transform);
            BulletBehaviour bulletScr = bulletClone.GetComponent<BulletBehaviour>();

            m_TimePastSinceLastFire = 0.0f;
            --GetCurrentMagSize;
            m_IsFiring = false;

            raycastedDir += new Vector3(m_BulletSpreadDirs.x, m_BulletSpreadDirs.y, 0.0f);

#if DEBUG
            if(SoundManager.GetInstance != null)
#endif
            {
                SoundManager.GetInstance.PlaySoundClip(m_EFiringSound, transform.position);
            }
            
            bulletScr.Fire(m_BulletSpawnPoint, raycastedDir);

#if DEBUG
            if (m_MuzzleFlash != null)
#endif
            {
                m_MuzzleFlash.GetComponent<ParticleSystem>().Play();
                m_MuzzleFlash.GetComponent<ParticleSystem>().Clear();
            }

            m_CurrentGunState = EGunState.READY;
        }
    }


    private void GunReloadingState()
    {
        if (GetCurrentReloadTime < 0.0f)
        {
            GetIsReloading = false;
            m_TimePastSinceLastFire = 0.0f;
            GetCurrentMagSize = m_MagSizeTotal;
            GetCurrentReloadTime = m_ReloadAnimTime;
            m_CurrentGunState = EGunState.READY;

            m_GetAnimator.SetBool("Fire", false);
            m_GetAnimator.SetBool("Overheat", false);
            m_GetAnimator.SetBool("Idle", true);
        }
    }


    private void MagUpdate()
    {
        // rate of fire
        if (m_TimePastSinceLastFire < m_Rpm)
        {
            m_TimePastSinceLastFire += Time.deltaTime;
        }

        // empty clip
        if (GetCurrentMagSize <= 0.0f)
        {
            Reload();
        }

        // reloading time
        if(GetIsReloading == true)
        {
            GetCurrentReloadTime -= Time.deltaTime;  // put this here because something with c# switches
        }
    }


    private void AimPosUpdate()
    {
        // ads
        if (Input.GetMouseButton(1) == true &&
            GetIsReloading == false)
        {
            GetIsADS = true;

            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 52.0f, 0.4f);

            m_BulletSpreadDirs.x = Random.Range(-m_AdsSpread, m_AdsSpread);
            m_BulletSpreadDirs.y = Random.Range(-m_AdsSpread, m_AdsSpread);

            Vector3 forward = transform.parent.forward * 1.45f;
            Vector3 down = transform.parent.up * -0.272f;

            transform.position = Vector3.Lerp(transform.position, (transform.parent.position + down + forward), 0.6f);

            m_CrossHairObj.SetActive(false);
        }

        // hip
        if (Input.GetMouseButton(1) == false ||
            GetIsReloading == true)
        {
            GetIsADS = false;

            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60.0f, 0.075f);

            m_BulletSpreadDirs.x = Random.Range(-m_HipSpread, m_HipSpread);
            m_BulletSpreadDirs.y = Random.Range(-m_HipSpread, m_HipSpread);

            Vector3 offsetPos = (transform.right * m_PositionOffset.x) +
                                (transform.up * m_PositionOffset.y) +
                                (transform.forward * m_PositionOffset.z);

            transform.position = Vector3.Lerp(transform.position, transform.parent.transform.position + offsetPos, 0.175f);

            m_CrossHairObj.SetActive(true);
        }
    }


    public void Fire(Transform cameraPoint)
    {
        if (GetIsReloading == false)
        {
            m_IsFiring = true;
            m_CameraPoint = cameraPoint;

            //Time.timeScale = 0.5f;
        }
    }


    public void Reload()
    {
        if(GetIsReloading == false)
        {
            if(GetCurrentMagSize < m_MagSizeTotal)
            {
                if(m_GunName == "Plasma")
                {
                    SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.GUN_RELOAD_1, transform.position);
                }
                else if (m_GunName == "Grenade")
                {
                    SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.GUN_RELOAD_2, transform.position);
                }

                GetIsReloading = true;
                GetCurrentReloadTime = m_ReloadAnimTime;
                m_CurrentGunState = EGunState.RELOADING;

                m_GetAnimator.SetBool("Idle", false);
                m_GetAnimator.SetBool("Fire", false);
                m_GetAnimator.SetBool("Overheat", true);
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
#if DEBUG
        Debug.DrawLine(m_RayOriginPoint.position, m_RayOriginPoint.position + (m_RayOriginPoint.forward * 100.0f), Color.green);
#endif

        if(m_IsPaused == false)
        {
            GunStateUpdate();
            MagUpdate();
            AimPosUpdate();
        }
    }
}
