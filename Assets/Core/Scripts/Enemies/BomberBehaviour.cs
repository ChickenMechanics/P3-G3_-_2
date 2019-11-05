using UnityEngine;

public class BomberBehaviour : EnemyBehaviour
{
    public float playerDamage;
    public float enemyDamage;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;

    private float m_TimeToNextAttack;

    //private EnemyBomberAnimation m_Anims;

    // Start is called before the first frame update
    private void Start()
    {
        currentState = State.MOVE;
        m_TimeToNextAttack = attackDuration;
        //m_Anims = gameObject.GetComponent<EnemyBomberAnimation>();
        hp = health;

        //SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.ENEMY_SPAWN, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case State.ATTACK: Attack(); break;
            case State.DEATH:  Death();  break;
            case State.MOVE:   Move();   break;
        }
    }

    private void Attack()
    {
        //m_Anims.SetAnim(EnemyBomberAnimation.EAnimBomber.EXPLODE);

        agent.isStopped = true;

        if (m_TimeToNextAttack <= 0)
        {
            UpdateDistanceToPlayer();

            if (distanceToPlayer <= attackRange)
                PlayerManager.GetInstance.DecreaseHealth(playerDamage);

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (Mathf.Abs(enemy.transform.position.magnitude - transform.position.magnitude) < attackRange)
                    enemy.GetComponent<EnemyBehaviour>().TakeDamage(playerDamage);
            }

            Destroy(transform.gameObject);
        }
        else
            m_TimeToNextAttack -= Time.deltaTime;
    }

    private void Death()
    {
        ScoreManager.GetInstance.AddComboPoints(scoreAmount);

#if DEBUG
        if (SoundManager.GetInstance != null)
#endif
        {
            SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_DEATH, transform.position);
        }

        //m_Anims.SetAnim(EnemyBomberAnimation.EAnimBomber.EXPLODE);

        Destroy(transform.gameObject);
    }

    private void Move()
    {
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