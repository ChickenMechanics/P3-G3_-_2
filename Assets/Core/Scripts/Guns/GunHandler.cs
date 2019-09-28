using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunHandler : MonoBehaviour
{
    #region design vars
    [Header("Gun Locker")]
    public int m_DefaultGun;
    public GameObject[] m_GunPrefab;
    #endregion

    private GameObject[] m_GunPrefabClone;
    private GameObject m_ActiveGun;
    private GunTemplate m_ActiveGunScr;
    private int m_ActiveGunIdx;
    private int m_NumOfGuns;


    //----------------------------------------------------------------------------------------------------


    public GameObject GetActiveGun()
    {
        return m_ActiveGun;
    }


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


    private void CreateGunInstances()
    {
        m_GunPrefabClone = new GameObject[m_GunPrefab.Length];
        Transform parent = GameObject.Find("Camera Point").transform;
        for (int i = 0; i < m_GunPrefab.Length; ++i)
        {
            m_GunPrefabClone[i] = Instantiate(m_GunPrefab[i], Vector3.zero, Quaternion.identity);
            m_GunPrefabClone[i].SetActive(false);

            Transform tForm = m_GunPrefabClone[i].transform;
            m_GunPrefabClone[i].transform.position = parent.transform.position + tForm.position;

            m_GunPrefabClone[i].transform.SetParent(parent);
            m_GunPrefabClone[i].GetComponent<GunTemplate>().InitGun();

            ++m_NumOfGuns;
        }
    }


    public void Fire(Transform cameraPoint)   // Dir equals player camera transform forward
    {
        m_ActiveGunScr.Fire(cameraPoint);
    }
}
