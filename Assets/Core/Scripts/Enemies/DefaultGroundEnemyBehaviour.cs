﻿using UnityEngine;
using UnityEngine.AI;

public class DefaultGroundEnemyBehaviour : MonoBehaviour
{
    #region Header
    protected enum State { ATTACK, DEATH, MOVE }
    protected State currentState;
    protected Vector3 position;
    protected float distanceToPlayer;
    protected float hp;

    private Quaternion m_LookRotation;
    #endregion

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
        if (other.gameObject.layer == 13)   // == projectile
        {
            TakeDamage(other.GetComponent<BulletBehaviour>().m_DamageValue);
#if DEBUG
            if (SoundManager.GetInstance != null)
#endif
            {
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_AR_DAMAGE, other.transform.position);
            }
        }
    }
    
    public void TakeDamage(float damageValue)
    {
        hp -= damageValue;

        if (hp <= 0)
            currentState = State.DEATH;
    }
}