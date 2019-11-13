using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunManager : MonoBehaviour
{
    public static GunManager GetInstance { private set; get; }

    #region design vars
    [Header("Gun Locker")]
    public int m_DefaultGun;
    public GameObject[] m_arrGunPrefab;
    #endregion

    private GameObject[] m_arrGunPrefabClone;
    [HideInInspector]
    public GunTemplate[] m_arrGunTemplateScr;
    [HideInInspector]
    public GameObject ActiveGun { private set; get; }
    private GunTemplate m_ActiveGunScr;
    private Transform m_tParent;
    private int m_ActiveGunIdx;
    private int m_NowGunIdx;
    private int m_PrevGunIdx;
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
        m_NowGunIdx = m_ActiveGunIdx;
        m_PrevGunIdx = m_ActiveGunIdx;
        ActiveGun = m_arrGunPrefabClone[m_ActiveGunIdx];
        //ActiveGun.SetActive(true);
        m_ActiveGunScr = ActiveGun.GetComponent<GunTemplate>();

        for (int i = 0; i < m_arrGunTemplateScr.Length; ++i)
        {
            if (i != m_ActiveGunIdx)
            {
                m_arrGunTemplateScr[i].DisableGun();
            }
        }

        //m_NumOfGuns = -1;

        //CreateGunInstances();

        //m_ActiveGunIdx = m_DefaultGun;
        //m_NowGunIdx = m_ActiveGunIdx;
        //m_PrevGunIdx = m_ActiveGunIdx;
        //ActiveGun = m_arrGunPrefabClone[m_ActiveGunIdx];
        //ActiveGun.SetActive(true);
        //m_ActiveGunScr = ActiveGun.GetComponent<GunTemplate>();

        //for (int i = 0; i < m_arrGunTemplateScr.Length; ++i)
        //{
        //    if (i != m_ActiveGunIdx)
        //    {
        //        m_arrGunTemplateScr[i].DisableGun();
        //    }
        //}
    }


    public void SetActiveGun(int idx)
    {
        if (idx != m_ActiveGunIdx)
        {
            m_ActiveGunIdx = idx;

            m_arrGunTemplateScr[m_PrevGunIdx].DisableGun();
            ActiveGun = m_arrGunPrefabClone[m_ActiveGunIdx];
            m_arrGunTemplateScr[m_ActiveGunIdx].EnableGun();

            //ActiveGun.SetActive(false);
            //ActiveGun = m_arrGunPrefabClone[m_ActiveGunIdx];
            //ActiveGun.SetActive(true);

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
        if(PlayerManager.GetInstance.GetIsAlive == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                m_PrevGunIdx = m_NowGunIdx;
                m_NowGunIdx = (m_NowGunIdx == 1) ? 0 : 1;
                SetActiveGun(m_NowGunIdx);
            }

            float wheelDir = Input.GetAxisRaw("Mouse ScrollWheel");
            if (wheelDir != 0.0f)
            {
                if (wheelDir == 0.1f)
                {
                    //++m_CurrentGunIdx;
                    //if (m_CurrentGunIdx > m_NumOfGuns)
                    //    m_CurrentGunIdx = 0;

                    m_PrevGunIdx = m_NowGunIdx;
                    m_NowGunIdx = 0;
                }
                else
                {
                    //--m_CurrentGunIdx;
                    //if (m_CurrentGunIdx < 0)
                    //    m_CurrentGunIdx = m_NumOfGuns;

                    m_PrevGunIdx = m_NowGunIdx;
                    m_NowGunIdx = 1;
                }

                SetActiveGun(m_NowGunIdx);
            }
        }
    }


    private void CreateGunInstances()
    {
        int size = m_arrGunPrefab.Length;
        m_arrGunPrefabClone = new GameObject[size];
        m_arrGunTemplateScr = new GunTemplate[size];

        m_tParent = PlayerManager.GetInstance.GetPlayer.transform.Find("Look");
        for (int i = 0; i < m_arrGunPrefab.Length; ++i)
        {
            m_arrGunPrefabClone[i] = Instantiate(m_arrGunPrefab[i], Vector3.zero, Quaternion.identity);
            //m_arrGunPrefabClone[i].SetActive(false);
            m_arrGunTemplateScr[i] = m_arrGunPrefabClone[i].GetComponent<GunTemplate>();

            Transform tForm = m_arrGunPrefabClone[i].transform;
            m_arrGunPrefabClone[i].transform.position = m_tParent.transform.position + tForm.position;

            m_arrGunPrefabClone[i].transform.SetParent(m_tParent);
            m_arrGunTemplateScr[i].InitGun();

            ++m_NumOfGuns;
        }

        //int size = m_arrGunPrefab.Length;
        //m_arrGunPrefabClone = new GameObject[size];
        //m_arrGunTemplateScr = new GunTemplate[size];

        //m_tParent = PlayerManager.GetInstance.GetPlayer.transform.Find("Look");
        //for (int i = 0; i < m_arrGunPrefab.Length; ++i)
        //{
        //    m_arrGunPrefabClone[i] = Instantiate(m_arrGunPrefab[i], Vector3.zero, Quaternion.identity);
        //    m_arrGunPrefabClone[i].SetActive(false);
        //    m_arrGunTemplateScr[i] = m_arrGunPrefabClone[i].GetComponent<GunTemplate>();

        //    Transform tForm = m_arrGunPrefabClone[i].transform;
        //    m_arrGunPrefabClone[i].transform.position = m_tParent.transform.position + tForm.position;

        //    m_arrGunPrefabClone[i].transform.SetParent(m_tParent);
        //    m_arrGunTemplateScr[i].InitGun();

        //    ++m_NumOfGuns;
        //}
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
