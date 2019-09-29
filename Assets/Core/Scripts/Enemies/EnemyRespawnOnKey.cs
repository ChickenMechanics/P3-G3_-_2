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
    private bool m_IsRotaterElevator;


    public void Elevator()
    {
        if (m_EnemyInstance.transform.position.y > transform.position.y)
        {
            m_EnemyInstance.transform.position = transform.position;

            m_IsRotaterElevator = false;
            return;
        }

        Vector3 lastPos = m_EnemyInstance.transform.position;
        m_EnemyInstance.transform.position = Vector3.Lerp(lastPos, new Vector3(lastPos.x, transform.position.y - 0.8f, lastPos.z), 0.9f * m_ElevSpeed * Time.deltaTime);
    }


    private void Awake()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        m_ElevSpeed = 15.0f;

        m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
        m_EnemyInstance.transform.position = transform.position - new Vector3(0.0f, 1.0f, 0.0f);

        m_IsRotaterElevator = true;
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
            float boundsSize = m_EnemyInstance.GetComponent<BoxCollider>().bounds.size.magnitude;
            m_EnemyInstance.transform.position = transform.position - new Vector3(0.0f, boundsSize + 1.0f, 0.0f);

            m_IsRotaterElevator = true;
        }

        if (m_EnemyInstance != null)
        {
            if (m_IsRotaterElevator == true)
            {
                Elevator();
            }
        }
    }
}
