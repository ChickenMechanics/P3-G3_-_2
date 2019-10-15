using System;
using UnityEngine;
using UnityEngine.AI;

public class BullBehaviour : DefaultGroundEnemyBehaviour
{
    public float rushDistance;
    public float startRushDistance;
    public float distanceToEndPoint;

    private bool m_LockOn;
    private NavMeshAgent m_Agent;
    private Vector3 m_RushDirection;
    private Vector3 m_StartRushPosition;
    private Vector3 m_EndRushPosition;

    // Start is called before the first frame update
    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_LockOn)
        {
            distanceToEndPoint = m_EndRushPosition.magnitude - position.magnitude;

            m_Agent.SetDestination(m_EndRushPosition);

            if (Math.Abs(distanceToEndPoint) < 0.1)
                m_LockOn = false;
        }
        else
        {
            MoveTowardsPlayer(transform, m_Agent);

            if (distanceToPlayer > startRushDistance) return;

            m_RushDirection = transform.forward;
            m_StartRushPosition = position;
            m_EndRushPosition = m_StartRushPosition + rushDistance * m_RushDirection;
            m_LockOn = true;
        }
    }
}