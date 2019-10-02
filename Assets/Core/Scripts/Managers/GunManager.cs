using System.Collections;
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
    private GameObject m_ActiveGun;
    private GunTemplate m_ActiveGunScr;
    private int m_ActiveGunIdx;
    private int m_NumOfGuns;
    private int m_CurrentGunIdx;


    public int GetNumOfGuns()
    {
        return m_NumOfGuns;
    }


    public int GetActiveGunIdx()
    {
        return m_ActiveGunIdx;
    }


    public void Init()
    {
        m_NumOfGuns = 0;

        CreateGunInstances();

        m_ActiveGunIdx = m_DefaultGun;
        m_ActiveGun = m_GunPrefabClone[m_ActiveGunIdx];
        m_ActiveGun.SetActive(true);
        m_ActiveGunScr = m_ActiveGun.GetComponent<GunTemplate>();
    }


    public void SetActiveGun(int idx)
    {
        if (idx != m_ActiveGunIdx)
        {
            m_ActiveGunIdx = idx;

            m_ActiveGun.SetActive(false);
            m_ActiveGun = m_GunPrefabClone[m_ActiveGunIdx];
            m_ActiveGun.SetActive(true);
            m_ActiveGunScr = m_ActiveGun.GetComponent<GunTemplate>();
        }
    }


    public void Fire(Transform cameraPoint)   // Dir equals player camera transform forward
    {
        m_ActiveGunScr.Fire(cameraPoint);
    }


    public void Reload()   // Dir equals player camera transform forward
    {
        m_ActiveGunScr.Reload();
    }


    private void ScrollWeapons()
    {
        float wheelDir = Input.GetAxisRaw("Mouse ScrollWheel");
        if (wheelDir != 0.0f)
        {
            if (wheelDir != -0.1f)
            {
                ++m_CurrentGunIdx;
                if (m_CurrentGunIdx > m_NumOfGuns - 1)
                    m_CurrentGunIdx = 0;
            }
            else
            {
                --m_CurrentGunIdx;
                if (m_CurrentGunIdx < 0)
                    m_CurrentGunIdx = m_NumOfGuns - 1;
            }

            SetActiveGun(m_CurrentGunIdx);
        }
    }


    private void CreateGunInstances()
    {
        int size = m_GunPrefab.Length;
        m_GunPrefabClone = new GameObject[size];
        m_GunTempScrs = new GunTemplate[size];

        Transform parent = PlayerManager.GetInstance.Player.transform.Find("Look");
        //Transform parent = GameObject.Find("Camera Point").transform;     // Before there was a gun manager and a player manager
        for (int i = 0; i < m_GunPrefab.Length; ++i)
        {
            m_GunPrefabClone[i] = Instantiate(m_GunPrefab[i], Vector3.zero, Quaternion.identity);
            m_GunPrefabClone[i].SetActive(false);
            m_GunTempScrs[i] = m_GunPrefabClone[i].GetComponent<GunTemplate>();

            Transform tForm = m_GunPrefabClone[i].transform;
            m_GunPrefabClone[i].transform.position = parent.transform.position + tForm.position;

            m_GunPrefabClone[i].transform.SetParent(parent);
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
