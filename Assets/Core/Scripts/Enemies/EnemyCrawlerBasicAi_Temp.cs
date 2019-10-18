using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyCrawlerBasicAi_Temp : MonoBehaviour
{
    public float m_OnSpawnAiDelayTime = 2.0f;

    private NavMeshAgent m_Agent;
    private Transform m_tFormPlayer;
    private EnemyCrawlerAnimation m_AnimScr;
    private BoxCollider m_MeleeHitBox;
    private ECrawlerState m_CurrentSate;
    private float m_WalkTriggerMinDist;
    private float m_StateDelayTimer;
    private bool m_StateUpdateDelay;

    private enum ECrawlerState
    {
        WALK,
        MELEE,
        IDLE,
        SIZE
    }


    //----------------------------------------------------------------------------------------------------


    private void StateUpdate()
    {
        switch ((int)m_CurrentSate)
        {
            case (int)ECrawlerState.IDLE:  IdleState();    break;
            case (int)ECrawlerState.WALK:  WalkState();    break;
            case (int)ECrawlerState.MELEE: MeleeState();   break;
        }
    }


    private void IdleState()
    {
        Vector3 currentPlayerPos = m_tFormPlayer.position;

        float dist = (currentPlayerPos - transform.position).magnitude;
        if (dist < m_WalkTriggerMinDist)
        {
            m_CurrentSate = ECrawlerState.MELEE;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.ATTACK);

            return;
        }
       
        m_CurrentSate = ECrawlerState.WALK;
        m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

        m_Agent.isStopped = false;
    }


    private void WalkState()
    {
        float dist = Vector3.Distance(m_tFormPlayer.position, transform.position);
        if (dist > m_WalkTriggerMinDist)
        {
            m_Agent.destination = m_tFormPlayer.position;
        }
        else
        {
            m_CurrentSate = ECrawlerState.MELEE;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.ATTACK);

            m_Agent.isStopped = true;
        }
    }


    private void MeleeState()
    {
        float dist = Vector3.Distance(m_tFormPlayer.position, transform.position);
        if (dist > m_WalkTriggerMinDist)
        {
            m_CurrentSate = ECrawlerState.WALK;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

            m_Agent.isStopped = false;
        }
    }


    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.enabled = false;

        if (PlayerManager.GetInstance != null)
        {
            m_tFormPlayer = PlayerManager.GetInstance.GetPlayer.transform.Find("Move").transform;
        }

        m_AnimScr = GetComponent<EnemyCrawlerAnimation>();

        m_CurrentSate = ECrawlerState.WALK;

        m_WalkTriggerMinDist = 4.0f;
        m_StateDelayTimer = m_OnSpawnAiDelayTime;

        m_StateUpdateDelay = true;
    }


    private void Update()
    {
        if (m_StateUpdateDelay != false)
        {
            m_StateDelayTimer -= Time.deltaTime;
            if (m_StateDelayTimer > 0.0f)
            {
                return;
            }

            m_StateUpdateDelay = false;

            m_Agent.enabled = true;
            m_Agent.isStopped = false;
        }

        if(m_tFormPlayer != null)
        {
            StateUpdate();
        }
    }
}
