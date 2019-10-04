using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRespawnOnKey : MonoBehaviour
{
    [Header("Press 'U' for respawn")]

    public GameObject m_EnemyToSpawn;
    private MeshRenderer m_MeshRenderer;
    private GameObject m_EnemyInstance;
    private float m_ElevSpeed;
    float m_CurrentSpawnDelayT;
    private bool m_IsElevator;
    private bool m_SpawnDelayLocked;


    public void Elevator()
    {
        if (m_EnemyInstance.transform.position.y > transform.position.y)
        {
            m_EnemyInstance.transform.position = transform.position;

            m_IsElevator = false;
            return;
        }

        Vector3 lastPos = m_EnemyInstance.transform.position;
        m_EnemyInstance.transform.position = Vector3.Lerp(lastPos, new Vector3(lastPos.x, transform.position.y - 0.8f, lastPos.z), 0.9f * m_ElevSpeed * Time.deltaTime);
    }


    private void Awake()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        m_ElevSpeed = 18.0f;

        //m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
        //float boundsSize = m_EnemyInstance.GetComponent<BoxCollider>().bounds.size.magnitude;
        //m_EnemyInstance.transform.position = transform.position - new Vector3(0.0f, boundsSize + 1.0f, 0.0f);

        m_CurrentSpawnDelayT = 0.0f;
        m_IsElevator = true;
        m_SpawnDelayLocked = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (m_EnemyInstance != null)
            {
                return; 
            }

            if(m_SpawnDelayLocked == false)
            {
                m_CurrentSpawnDelayT = Random.Range(0.0f, 0.2f);
                m_SpawnDelayLocked = true;
            }
        }

        m_CurrentSpawnDelayT -= Time.deltaTime;
        if (m_SpawnDelayLocked == true &&
            m_CurrentSpawnDelayT <= 0.0f)
        {
            m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
            float boundsSize = m_EnemyInstance.GetComponent<BoxCollider>().bounds.size.magnitude;
            m_EnemyInstance.transform.position = transform.position - new Vector3(0.0f, boundsSize + 1.0f, 0.0f);

            m_IsElevator = true;
            m_SpawnDelayLocked = false;
        }

        if (m_EnemyInstance != null)
        {
            if (m_IsElevator == true)
            {
                Elevator();
            }
        }
    }
}
