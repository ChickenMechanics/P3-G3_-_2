using UnityEngine;
using UnityEngine.AI;

public class CrawlerBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;
    public float damageAmount;
    public float scoreAmount;
    public float attackRange;
    public float health;

    private readonly ScoreManager m_ScoreManager;

    private DefaultGroundEnemyBehaviour m_DefaultGroundEnemyBehaviour;

    // Start is called before the first frame update
    private void Start()
    {
        m_DefaultGroundEnemyBehaviour = gameObject.GetComponent<DefaultGroundEnemyBehaviour>();
        m_DefaultGroundEnemyBehaviour.SetHealth(health);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_DefaultGroundEnemyBehaviour.GetHealth() > 0)
            BaseState();
        else
            DeathState();
    }

    private void BaseState()
    {
        m_DefaultGroundEnemyBehaviour.MoveTowardsPlayer(transform, agent);

        if (m_DefaultGroundEnemyBehaviour.GetDistanceToPlayer() < attackRange)
            PlayerManager.GetInstance.DecreaseHealth(damageAmount);
    }

    private void DeathState()
    {
        if (m_ScoreManager != null)
            m_ScoreManager.AddComboPoints(scoreAmount);

        Destroy(gameObject);
    }
}