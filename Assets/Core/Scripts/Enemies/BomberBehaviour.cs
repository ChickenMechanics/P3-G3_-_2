using System.Collections;
using UnityEngine;

public class BomberBehaviour : EnemyBehaviour
{
    public float playerDamage;
    public float enemyDamage;
    public float scoreAmount;
    public float attackDuration;
    public float attackRange;
    public float health;

    // Start is called before the first frame update
    private void Start()
    {
        currentState = State.MOVE;
        hp = health;
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

            case State.DEATH:  Death();  break;
            case State.MOVE:   Move();   break;
        }
    }

    private IEnumerator Attack()
    {
        SetHasAttacked(true);

        agent.isStopped = true;

        yield return new WaitForSeconds(attackDuration);
        
        UpdateDistanceToPlayer();

        if (distanceToPlayer <= attackRange)
            PlayerManager.GetInstance.DecreaseHealth(playerDamage);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Mathf.Abs(enemy.transform.position.magnitude - transform.position.magnitude) < attackRange)
                enemy.GetComponent<EnemyBehaviour>().TakeDamage(enemyDamage);
        }

        Destroy(transform.gameObject);
    }

    private void Death()
    {
        ScoreManager.GetInstance.AddComboPoints(scoreAmount);

        if (SoundManager.GetInstance != null)
            SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_DEATH, transform.position);

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