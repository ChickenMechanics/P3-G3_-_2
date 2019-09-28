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
    // Ammunition things
    private List<GameObject> m_BulletPrefabClones;
    private List<BulletBehaviour> m_BulletBehaviourScripts;
    private GameObject m_BulletFolder;
    private RaycastHit m_RaycastHit;
    private Vector3 m_SecretSpot;
    private float m_RayMaxDist;
    private float m_Rpm;
    private float m_TimePastSinceLastFire;
    private int m_NextFreeBullet;


    //----------------------------------------------------------------------------------------------------


    public void InitGun()
    {
        m_BulletPrefabClones = new List<GameObject>();
        m_BulletBehaviourScripts = new List<BulletBehaviour>();

        m_BulletFolder = new GameObject("Bullets");
        m_BulletFolder.transform.position = new Vector3(5.0f, -10.0f, 0.0f);

        m_RaycastHit = new RaycastHit();

        m_SecretSpot = new Vector3(0.0f, -10.0f, 0.0f);

        m_RayMaxDist = 1000.0f;
        m_Rpm = 60.0f / m_RoundsPerMinute;
        m_TimePastSinceLastFire = m_Rpm;

        m_BulletSpawnPoint = transform.GetChild(0);
        
        InitMagazine();
    }


    private void InitMagazine()
    {
        // TODO: Reusing spent bullets is an idea

        if (m_BulletBehaviourScripts.Count > 0) m_BulletBehaviourScripts.Clear();
        if (m_BulletPrefabClones.Count > 0) m_BulletPrefabClones.Clear();

        Transform tForm = transform.GetChild(0).transform;
        Vector3 spawnPos = tForm.position;
        Quaternion spawnRot = tForm.rotation;

        for (int i = 0; i < m_MagazineSize; ++i)
        {
            GameObject bulletClone = Instantiate(m_BulletModelPrefab, spawnPos, spawnRot);
            BulletBehaviour bulletScr = bulletClone.GetComponent<BulletBehaviour>();

            bulletScr.InitBullet();
            bulletClone.SetActive(false);
            bulletClone.transform.SetParent(m_BulletFolder.transform);

            m_BulletPrefabClones.Add(bulletClone);
            m_BulletBehaviourScripts.Add(bulletScr);
        }
        
        m_NextFreeBullet = m_MagazineSize - 1;
    }


    private void UpdateMagazine()
    {
        // Time is resetted in fire

        if (m_TimePastSinceLastFire < m_Rpm)
            m_TimePastSinceLastFire += Time.deltaTime;
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


    public void Fire(Transform cameraPoint)
    {
#if DEBUG
        if (m_BulletBehaviourScripts.Count == 0)
        {
            Debug.LogWarning("GunTemplate::Fire(): No bollit in clip!");
            return;
        }
#endif

        if (m_TimePastSinceLastFire >= m_Rpm)
        {
            Ray ray = new Ray(cameraPoint.position, cameraPoint.forward);
            Vector3 raycastedDir = cameraPoint.forward;
            if (Physics.Raycast(ray, out m_RaycastHit, m_RayMaxDist))
            {
                raycastedDir = (m_RaycastHit.point - m_BulletSpawnPoint.position).normalized;
            }

            BulletBehaviour bulletScr = m_BulletBehaviourScripts[m_NextFreeBullet];
            GameObject bulletClone = m_BulletPrefabClones[m_NextFreeBullet];

            bulletScr.Fire(m_BulletSpawnPoint, raycastedDir);
            m_BulletBehaviourScripts.Remove(bulletScr);
            m_BulletPrefabClones.Remove(bulletClone);

            if (m_NextFreeBullet == 0) return;

            --m_NextFreeBullet;
            m_TimePastSinceLastFire = 0.0f;
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
    }
}
