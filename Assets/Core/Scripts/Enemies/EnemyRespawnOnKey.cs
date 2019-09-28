using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRespawnOnKey : MonoBehaviour
{
    [Header("Press 'U' for respawn")]

    public GameObject m_EnemyToSpawn;

    private MeshRenderer m_MeshRenderer;
    private GameObject m_EnemyInstance;


    private void Awake()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
        m_EnemyInstance.transform.position = transform.position - new Vector3(0.0f, 1.0f, 0.0f);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (m_EnemyInstance != null)
            {
                return; 
            }

            m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
            m_EnemyInstance.transform.position = transform.position - new Vector3(0.0f, 1.0f, 0.0f);
        }
    }
}
