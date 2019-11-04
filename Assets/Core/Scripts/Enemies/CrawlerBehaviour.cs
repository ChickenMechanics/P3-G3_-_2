using UnityEngine;

public class CrawlerBehaviour : EnemyBehaviour
{
    public float damageAmount;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;
    public float attackAngle;
    
    private EnemyCrawlerAnimation m_Anims;
    private float m_TimeToNextAttack;
    
    // Start is called before the first frame update
    private void Start()
    {
        currentState = State.MOVE;
        m_Anims = gameObject.GetComponent<EnemyCrawlerAnimation>();
        hp = health;
        m_TimeToNextAttack = attackDuration;

        SoundManager.GetInstance.PlaySoundClip(
            SoundManager.ESoundClip.ENEMY_SPAWN, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case  State.ATTACK: Attack(); break;
            case  State.DEATH:  Death();  break;
            case  State.MOVE:   Move();   break;
        }
    }
    
    private void Attack()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.ATTACK);

        agent.isStopped = true;

        if (m_TimeToNextAttack <= 0)
        {
            UpdateDistanceToPlayer();

            if (Mathf.Abs(Vector3.Angle(transform.position, playerPos) - 90f) < attackAngle &&
                distanceToPlayer <= attackRange)
            {
                PlayerManager.GetInstance.DecreaseHealth(damageAmount);
            }

            m_TimeToNextAttack = attackDuration;
            currentState = State.MOVE;
        }
        else
        {
            m_TimeToNextAttack -= Time.deltaTime;
            currentState = State.MOVE;
        }
    }

    private void Death()
    {
        ScoreManager.GetInstance.AddComboPoints(scoreAmount);

#if DEBUG
        if (SoundManager.GetInstance != null)
#endif
        {
            SoundManager.GetInstance.PlaySoundClip(
                SoundManager.ESoundClip.CRAWLER_DEATH, transform.position);
        }

        Destroy(transform.gameObject);
    }

    private void Move()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

        UpdateDistanceToPlayer();

        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false;
            MoveTowardsPlayer();
        }
        else
            currentState = State.ATTACK;
    }
}