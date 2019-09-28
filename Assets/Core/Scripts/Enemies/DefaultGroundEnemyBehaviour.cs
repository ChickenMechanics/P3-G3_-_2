using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.AI.NavMeshAgent;

public class DefaultGroundEnemyBehaviour : MonoBehaviour
{
    private Transform player;
    //public NavMeshAgent agent;

    private Vector3 m_Position;

    public Vector3 GetPosition() { return m_Position; }

    private float m_DistanceToPlayer;
    public float GetDistanceToPlayer() { return m_DistanceToPlayer; }

    private Quaternion m_LookRotation;
    public Quaternion GetLookDirection() { return m_LookRotation; }
    
    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
    }

    // Update is called once per frame
    public void MoveTowardsPlayer(Transform transform_TODO_RENAME, NavMeshAgent agent)
    {
        var playerPos = player.position;
        m_Position = transform_TODO_RENAME.position;
        var rotation = transform_TODO_RENAME.rotation;

        var lookPosition = playerPos - m_Position;

        lookPosition.y = 0;

        m_LookRotation = Quaternion.LookRotation(lookPosition);

        m_DistanceToPlayer = lookPosition.magnitude;

        transform_TODO_RENAME.rotation = Quaternion.Slerp(rotation, m_LookRotation, 0.1f);

        agent.SetDestination(playerPos);
    }
}