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
    private Vector3 m_LastPlayerPos;
    private ECrawlerState m_CurrentSate;
    private float m_WalkTriggerMinDist;
    private float m_StateDelayTimer;
    private bool m_StateUpdateDelay;

    private enum ECrawlerState
    {
        IDLE = 0,
        WALK,
        MELEE,
        SIZE
    }


    private void StateUpdate()
    {
        switch((int)m_CurrentSate)
        {
            case (int)ECrawlerState.IDLE:  IdleState();    break;
            case (int)ECrawlerState.WALK:  WalkState();    break;
            case (int)ECrawlerState.MELEE: MeleeState();   break;
        }
    }


    private void IdleState()
    {
        float dist = (m_tFormPlayer.position - transform.position).magnitude;
        if (dist < m_WalkTriggerMinDist)
        {
            m_CurrentSate = ECrawlerState.MELEE;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.MELEE);

            return;
        }

        Vector3 currentPlayerPos = m_tFormPlayer.position;
        if (currentPlayerPos != m_LastPlayerPos)
        {
            m_CurrentSate = ECrawlerState.WALK;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

            m_Agent.isStopped = false;


        }
    }


    private void WalkState()
    {
        float dist = (m_tFormPlayer.position - transform.position).magnitude;
        if (dist > m_WalkTriggerMinDist)
        {
            m_Agent.destination = m_tFormPlayer.position;

            return;
        }
        else
        {
            m_CurrentSate = ECrawlerState.MELEE;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.MELEE);

            m_Agent.isStopped = true;

        }
    }


    private void MeleeState()
    {
        float dist = (m_tFormPlayer.position - transform.position).magnitude;
        if (dist > m_WalkTriggerMinDist)
        {
            m_CurrentSate = ECrawlerState.WALK;
            m_AnimScr.SetAnim(EnemyCrawlerAnimation.EAnimCrawler.WALK);

            m_Agent.isStopped = false;

            return;
        }
    }


    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();

        m_tFormPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        m_LastPlayerPos = m_tFormPlayer.position;

        m_AnimScr = GetComponent<EnemyCrawlerAnimation>();

        m_CurrentSate = ECrawlerState.IDLE;

        m_WalkTriggerMinDist = 4.0f;
        m_StateDelayTimer = m_OnSpawnAiDelayTime;

        m_StateUpdateDelay = true;
    }


    private void Update()
    {
        if (m_StateUpdateDelay == true)
        {
            m_StateDelayTimer -= Time.deltaTime;
            if (m_StateDelayTimer > 0.0f)
            {
                return;
            }

            m_StateUpdateDelay = false;
        }

        StateUpdate();
    }
}
