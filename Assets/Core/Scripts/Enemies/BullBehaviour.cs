using System;
using UnityEngine;
using UnityEngine.AI;

public class BullBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;

    public float rushDistance;
    public float startRushDistance;

    public float distanceToEndPoint;

    private DefaultGroundEnemyBehaviour m_DefaultGroundEnemyBehaviour;
    private bool m_LockOn = false;

    private Vector3 m_RushDirection;
    private Vector3 m_StartRushPosition;
    private Vector3 m_EndRushPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_DefaultGroundEnemyBehaviour = gameObject.AddComponent<DefaultGroundEnemyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LockOn)
        {
            distanceToEndPoint = m_EndRushPosition.magnitude - m_DefaultGroundEnemyBehaviour.GetPosition().magnitude;

            agent.SetDestination(m_EndRushPosition);

            if (Math.Abs(distanceToEndPoint) < 0.1)
                m_LockOn = false;
        }
        else
        {
            m_DefaultGroundEnemyBehaviour.MoveTowardsPlayer(transform, agent);

            if (m_DefaultGroundEnemyBehaviour.GetDistanceToPlayer() > startRushDistance) return;

            m_RushDirection = m_DefaultGroundEnemyBehaviour.transform.forward;
            m_StartRushPosition = m_DefaultGroundEnemyBehaviour.GetPosition();
            m_EndRushPosition = m_StartRushPosition + rushDistance * m_RushDirection;
            m_LockOn = true;
        }
    }
}