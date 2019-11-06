using System.Collections;
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
    
    // Start is called before the first frame update
    private void Start()
    {
        currentState = State.MOVE;
        m_Anims = gameObject.GetComponent<EnemyCrawlerAnimation>();
        hp = health;

        SoundManager.GetInstance.PlaySoundClip(
            SoundManager.ESoundClip.ENEMY_SPAWN, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case State.ATTACK:
                if (GetHasAttacked() == false)
                    StartCoroutine(Attack());
                break;
            case  State.DEATH: Death(); break;
            case  State.MOVE:  Move();  break;
        }
    }
    
    private IEnumerator Attack()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.ATTACK);

        SetHasAttacked(true);

        agent.isStopped = true;

        yield return new WaitForSeconds(attackDuration);

        UpdateDistanceToPlayer();

        if (Mathf.Abs(Vector3.Angle(transform.position, playerPos) - 90f) < attackAngle &&
            distanceToPlayer <= attackRange)
        {
            PlayerManager.GetInstance.DecreaseHealth(damageAmount);
        }

        SetHasAttacked(false);

        currentState = State.MOVE;
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