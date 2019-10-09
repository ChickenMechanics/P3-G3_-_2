﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.AI.NavMeshAgent;

public class DefaultGroundEnemyBehaviour : MonoBehaviour
{
    #region Header

    private Transform m_Player;
    private Quaternion m_LookRotation;

    private Vector3 m_Position;
    public Vector3 GetPosition() { return m_Position; }

    private float m_Health;
    public float GetHealth() { return m_Health; }
    public void SetHealth(float health) { m_Health = health; }

    private float m_DistanceToPlayer;
    public float GetDistanceToPlayer() { return m_DistanceToPlayer; }

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    public void MoveTowardsPlayer(Transform agentTransform, NavMeshAgent agent)
    {
        var playerPos = m_Player.position;
        m_Position = agentTransform.position;
        var rotation = agentTransform.rotation;

        var lookPosition = playerPos - m_Position;

        lookPosition.y = 0;

        if (lookPosition != Vector3.zero)
            m_LookRotation = Quaternion.LookRotation(lookPosition);

        m_DistanceToPlayer = lookPosition.magnitude;

        agentTransform.rotation = Quaternion.Slerp(rotation, m_LookRotation, 0.1f);

        agent.SetDestination(playerPos);
    }

    public void TakeDamage(float damageValue, Object obj)
    {
        m_Health -= damageValue;

        if (m_Health <= 0)
            Destroy(obj);
    }
}