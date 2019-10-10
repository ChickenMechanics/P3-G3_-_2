using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerBehaviour : DefaultGroundEnemyBehaviour, IController
{
    public float damageAmount;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;
    public float attackAngle;
    
    private EnemyCrawlerAnimation m_Anims;
    private NavMeshAgent agent;
    private bool m_HasDoneDamage;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.MOVE;
        m_Anims = gameObject.GetComponent<EnemyCrawlerAnimation>();
        HP = health;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case  State.ATTACK: StartCoroutine(Attack()); break;
            case  State.DEATH:                 Death();   break;
            case  State.MOVE:                  Move();    break;
            default: /*State.IDLE*/                 Idle();    break;
        }
    }
    
    private IEnumerator Attack()
    {
        var playerPos = PlayerManager.GetInstance.transform.position;
        
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.MELEE);

        if (Mathf.Abs(Vector3.Angle(position, playerPos) - 90f) < attackAngle && m_HasDoneDamage == false)
        {
            PlayerManager.GetInstance.DecreaseHealth(damageAmount);
            m_HasDoneDamage = true;
        }

        yield return new WaitForSeconds(attackDuration);

        m_HasDoneDamage = false;
        currentState = State.MOVE;
    }

    private void Death()
    {
        ScoreManager.GetInstance.AddComboPoints(scoreAmount);

        Debug.Log(scoreAmount);

        Destroy(gameObject);
    }

    private void Move()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

        MoveTowardsPlayer(transform, agent);

        if (distanceToPlayer < attackRange)
            currentState = (int) State.ATTACK;
    }

    private void Idle()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.IDLE);
    }
}