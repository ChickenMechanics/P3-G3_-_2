using UnityEngine;
using UnityEngine.AI;

public class DefaultGroundEnemyBehaviour : MonoBehaviour
{
    #region Header
    protected enum State { ATTACK, DEATH, MOVE }
    protected State currentState;
    protected Vector3 position;
    protected float distanceToPlayer;
    protected float hp;
    protected Vector3 playerPos;
    protected Vector3 lookPosition;

    private Quaternion m_LookRotation;
    #endregion

    protected void MoveTowardsPlayer(Transform agentTransform, NavMeshAgent agent)
    {
        UpdatePlayerPos();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        position = agentTransform.position;
        var rotation = agentTransform.rotation;

        UpdateDistanceToPlayer();

        if (lookPosition != Vector3.zero)
            m_LookRotation = Quaternion.LookRotation(lookPosition);

        agentTransform.rotation = Quaternion.Slerp(rotation, m_LookRotation, 0.1f);

        agent.SetDestination(playerPos);
    }

    protected void UpdatePlayerPos()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    protected void UpdateDistanceToPlayer()
    {
        UpdatePlayerPos();

        lookPosition = playerPos - position;

        lookPosition.y = 0;

        distanceToPlayer = lookPosition.magnitude;
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