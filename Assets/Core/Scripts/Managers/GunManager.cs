﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunManager : MonoBehaviour
{
    public static GunManager GetInstance { private set; get; }

    #region design vars
    [Header("Gun Locker")]
    public int m_DefaultGun;
    public GameObject[] m_GunPrefab;
    #endregion

    private GameObject[] m_GunPrefabClone;
    private GunTemplate[] m_GunTempScrs;
    [HideInInspector]
    public GameObject ActiveGun { private set; get; }
    private GunTemplate m_ActiveGunScr;
    private Transform m_tParent;
    private int m_ActiveGunIdx;
    private int m_CurrentGunIdx;
    private int m_NumOfGuns;

    private GameObject m_RumbleObj;
    private FxRumbleTransform m_RumbleScr;


    //----------------------------------------------------------------------------------------------------


    public int GetNumOfGuns()
    {
        return m_NumOfGuns;
    }


    public void Init()
    {
        m_NumOfGuns = -1;

        CreateGunInstances();

        m_ActiveGunIdx = m_DefaultGun;
        m_CurrentGunIdx = m_ActiveGunIdx;
        ActiveGun = m_GunPrefabClone[m_ActiveGunIdx];
        ActiveGun.SetActive(true);
        m_ActiveGunScr = ActiveGun.GetComponent<GunTemplate>();
    }


    public void SetActiveGun(int idx)
    {
        if (idx != m_ActiveGunIdx)
        {
            m_ActiveGunIdx = idx;

            ActiveGun.SetActive(false);

            ActiveGun = m_GunPrefabClone[m_ActiveGunIdx];
            ActiveGun.SetActive(true);

            m_ActiveGunScr.m_IsPaused = true;

            m_ActiveGunScr = ActiveGun.GetComponent<GunTemplate>();

            m_ActiveGunScr.m_IsPaused = false;
        }
    }

    public void Fire()   // Dir equals player camera transform forward
    {
        if(Input.GetMouseButton(0))
        {
            m_ActiveGunScr.Fire(m_tParent);
        }
    }


    public void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_ActiveGunScr.Reload();
        }
    }


    public void AdsHip()
    {
        m_ActiveGunScr.AimPosUpdate();
    }


    public void ScrollWeapons()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            m_CurrentGunIdx = (m_CurrentGunIdx == 1) ? 0 : 1;
            SetActiveGun(m_CurrentGunIdx);
        }

        float wheelDir = Input.GetAxisRaw("Mouse ScrollWheel");
        if (wheelDir != 0.0f)
        {
            if (wheelDir == 0.1f)
            {
                //++m_CurrentGunIdx;
                //if (m_CurrentGunIdx > m_NumOfGuns)
                //    m_CurrentGunIdx = 0;

                m_CurrentGunIdx = 0;
            }
            else
            {
                //--m_CurrentGunIdx;
                //if (m_CurrentGunIdx < 0)
                //    m_CurrentGunIdx = m_NumOfGuns;

                m_CurrentGunIdx = 1;
            }

            SetActiveGun(m_CurrentGunIdx);
        }
    }


    private void CreateGunInstances()
    {
        int size = m_GunPrefab.Length;
        m_GunPrefabClone = new GameObject[size];
        m_GunTempScrs = new GunTemplate[size];

        m_tParent = PlayerManager.GetInstance.GetPlayer.transform.Find("Look");
        for (int i = 0; i < m_GunPrefab.Length; ++i)
        {
            m_GunPrefabClone[i] = Instantiate(m_GunPrefab[i], Vector3.zero, Quaternion.identity);
            m_GunPrefabClone[i].SetActive(false);
            m_GunTempScrs[i] = m_GunPrefabClone[i].GetComponent<GunTemplate>();

            Transform tForm = m_GunPrefabClone[i].transform;
            m_GunPrefabClone[i].transform.position = m_tParent.transform.position + tForm.position;

            m_GunPrefabClone[i].transform.SetParent(m_tParent);
            m_GunTempScrs[i].InitGun();

            ++m_NumOfGuns;
        }
    }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        Init();
    }
}
