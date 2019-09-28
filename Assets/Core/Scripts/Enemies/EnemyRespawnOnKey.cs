using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRespawnOnKey : MonoBehaviour
{
    [Header("Press 'R' for respawn")]

    public GameObject m_EnemyToSpawn;

    private MeshRenderer m_MeshRenderer;
    private GameObject m_EnemyInstance;


    private void Awake()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (m_EnemyInstance != null)
            {
                return;
            }

            m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
        }
    }
}
