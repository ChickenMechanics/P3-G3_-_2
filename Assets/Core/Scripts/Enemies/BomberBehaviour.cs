using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BomberBehaviour : DefaultGroundEnemyBehaviour
{
    public float damageAmount;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;

    //private EnemyBomberAnimation m_Anims;
    private NavMeshAgent m_Agent;

    // Start is called before the first frame update
    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        currentState = State.MOVE;
        //m_Anims = gameObject.GetComponent<EnemyBomberAnimation>();
        hp = health;

        //SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.ENEMY_SPAWN, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case State.ATTACK: StartCoroutine(Attack()); break;
            case State.DEATH: Death(); break;
            case State.MOVE: Move(); break;
        }
    }

    private IEnumerator Attack()
    {
        //m_Anims.SetAnim(EnemyBomberAnimation.EAnimBomber.EXPLODE);

        yield return new WaitForSeconds(attackDuration);

        UpdatePlayerPos();

        UpdateDistanceToPlayer();
        
        if (distanceToPlayer <= attackRange)
            PlayerManager.GetInstance.DecreaseHealth(damageAmount);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Mathf.Abs(enemy.transform.position.magnitude - transform.position.magnitude) < attackRange)
                enemy.GetComponent<DefaultGroundEnemyBehaviour>().TakeDamage(damageAmount);
        }

        Destroy(transform.gameObject);
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

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Mathf.Abs(enemy.transform.position.magnitude - transform.position.magnitude) < attackRange)
                enemy.GetComponent<DefaultGroundEnemyBehaviour>().TakeDamage(damageAmount);
        }

        Destroy(transform.gameObject);
    }

    private void Move()
    {
        //m_Anims.SetAnim(EnemyBomberAnimation.EAnimBomber.WALK);

        MoveTowardsPlayer(transform, m_Agent);

        if (distanceToPlayer < attackRange)
            currentState = State.ATTACK;
    }
}