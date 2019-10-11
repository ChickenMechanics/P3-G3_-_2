﻿using UnityEngine;
using UnityEngine.AI;

public class DefaultGroundEnemyBehaviour : MonoBehaviour
{
    #region Header
    protected enum State { ATTACK, DEATH, IDLE, MOVE }
    protected State currentState;
    protected Vector3 position;
    protected float distanceToPlayer;
    protected float HP;

    private Quaternion m_LookRotation;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
    }

    protected void MoveTowardsPlayer(Transform agentTransform, NavMeshAgent agent)
    {
        var playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        position = agentTransform.position;
        var rotation = agentTransform.rotation;

        var lookPosition = playerPos - position;

        lookPosition.y = 0;

        if (lookPosition != Vector3.zero)
            m_LookRotation = Quaternion.LookRotation(lookPosition);

        distanceToPlayer = lookPosition.magnitude;

        agentTransform.rotation = Quaternion.Slerp(rotation, m_LookRotation, 0.1f);

        agent.SetDestination(playerPos);
    }

    private void OnTriggerEnter(Component other)
    {
        if (other.CompareTag("BulletBasic"))
        {
            TakeDamage(other.GetComponent<BulletBehaviour>().m_DamageValue);
            Destroy(other.gameObject);
        }
    }
    
    public void TakeDamage(float damageValue)
    {
        HP -= damageValue;

        if (HP <= 0)
            currentState = State.DEATH;
    }
}