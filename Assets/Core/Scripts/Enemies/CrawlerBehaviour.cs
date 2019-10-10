using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerBehaviour : DefaultGroundEnemyBehaviour, IController
{
    public NavMeshAgent agent;
    public float damageAmount;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;
    public float attackAngle;

    public enum State { ATTACK, DEATH, IDLE, MOVE }

    private EnemyCrawlerAnimation m_Anims;
    private int m_CurrentState = (int)State.MOVE;
    private bool m_HasDoneDamage;

    // Start is called before the first frame update
    private void Start()
    {
        m_Anims = gameObject.GetComponent<EnemyCrawlerAnimation>();
        SetHealth(health);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (m_CurrentState)
        {
            case  (int)State.ATTACK: StartCoroutine(Attack()); break;
            case  (int)State.DEATH:                 Death();   break;
            case  (int)State.MOVE:                  Move();    break;
            default: /*State.IDLE*/                 Idle();    break;
        }
    }
    
    private IEnumerator Attack()
    {
        var position = GetPosition();
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
        ScoreManager.GetInstance.AddComboPoints(scoreAmount);

        Debug.Log(scoreAmount);

        Destroy(gameObject);
    }

    private void Move()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

        MoveTowardsPlayer(transform, agent);

        if (GetDistanceToPlayer() < attackRange)
            m_CurrentState = (int) State.ATTACK;
    }

    private void Idle()
    {
        m_Anims.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.IDLE);
    }
}