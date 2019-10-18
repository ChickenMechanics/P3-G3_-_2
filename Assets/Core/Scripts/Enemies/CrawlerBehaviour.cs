using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerBehaviour : DefaultGroundEnemyBehaviour
{
    public float damageAmount;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;
    public float attackAngle;
    
    private EnemyCrawlerAnimation m_Anims;
    private NavMeshAgent m_Agent;

    // Start is called before the first frame update
    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        currentState = State.MOVE;
        m_Anims = gameObject.GetComponent<EnemyCrawlerAnimation>();
        hp = health;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case  State.ATTACK: StartCoroutine(Attack()); break;
            case  State.DEATH:                 Death();   break;
            case  State.MOVE:                  Move();    break;
        }
    }
    
    private IEnumerator Attack()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.ATTACK);

        yield return new WaitForSeconds(attackDuration);

        UpdatePlayerPos();

        UpdateDistanceToPlayer();

        if (Mathf.Abs(Vector3.Angle(position, playerPos) - 90f) < attackAngle &&
            distanceToPlayer <= attackRange)
        {
            PlayerManager.GetInstance.DecreaseHealth(damageAmount);
        }

        currentState = State.MOVE;
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

        Destroy(transform.gameObject);
    }

    private void Move()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

        MoveTowardsPlayer(transform, m_Agent);

        if (distanceToPlayer < attackRange)
            currentState = State.ATTACK;
    }
}