using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerBehaviour : MonoBehaviour, IController
{
    public NavMeshAgent agent;
    public float damageAmount;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;
    public float attackAngle;

    public enum State { ATTACK, DEATH, IDLE, MOVE }

    private readonly ScoreManager m_ScoreManager;
    private EnemyCrawlerAnimation m_Anims;
    private DefaultGroundEnemyBehaviour m_DefaultGroundEnemyBehaviour;
    private int m_CurrentState = (int)State.MOVE;
    private bool m_HasDoneDamage;

    // Start is called before the first frame update
    private void Start()
    {
        m_Anims = gameObject.GetComponent<EnemyCrawlerAnimation>();
        m_DefaultGroundEnemyBehaviour = gameObject.GetComponent<DefaultGroundEnemyBehaviour>();
        m_DefaultGroundEnemyBehaviour.SetHealth(health);
    }

    // Update is called once per frame
    private void Update()
    {
        RunState();

        //CrawlerMoveState();
        //if (m_DefaultGroundEnemyBehaviour.GetHealth() < 0)
        //    State = State.DEATH;

        // CrawlerDeathState();

        //var position = m_DefaultGroundEnemyBehaviour.GetPosition();
        //var playerPos = PlayerManager.GetInstance.transform.position;

        //if (Vector3.Angle(position, playerPos) < attackAngle &&
        //    m_DefaultGroundEnemyBehaviour.GetDistanceToPlayer() < attackRange)

        //if (m_DefaultGroundEnemyBehaviour.GetDistanceToPlayer() < attackRange)
        //{
        //    var attackState = CrawlerAttackState();
        //}
        //else
        //    CrawlerMoveState();

        // getFSM.Update();
    }

    private void RunState()
    {
        switch (m_CurrentState)
        {
            case (int) State.ATTACK: StartCoroutine(Attack()); break;
            case (int) State.DEATH:                 Death();   break;
            case (int) State.MOVE:                  Move();    break;
            default: /*State.IDLE*/                 Idle();    break;
        }
    }

    private IEnumerator Attack()
    {
        var position = m_DefaultGroundEnemyBehaviour.GetPosition();
        var playerPos = PlayerManager.GetInstance.transform.position;

        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.MELEE);

        if (Mathf.Abs(Vector3.Angle(position, playerPos) - 90f) < attackAngle && m_HasDoneDamage == false)
        {
            PlayerManager.GetInstance.DecreaseHealth(damageAmount);
            m_HasDoneDamage = true;
        }

        yield return new WaitForSeconds(attackDuration);

        m_HasDoneDamage = false;
        m_CurrentState = (int) State.MOVE;
    }

    private void Death()
    {
        if (m_ScoreManager != null)
            m_ScoreManager.AddComboPoints(scoreAmount);

        Destroy(gameObject);
    }

    private void Move()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

        m_DefaultGroundEnemyBehaviour.MoveTowardsPlayer(transform, agent);

        if (m_DefaultGroundEnemyBehaviour.GetDistanceToPlayer() < attackRange)
            m_CurrentState = (int) State.ATTACK;
    }

    private void Idle()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.IDLE);
    }
}