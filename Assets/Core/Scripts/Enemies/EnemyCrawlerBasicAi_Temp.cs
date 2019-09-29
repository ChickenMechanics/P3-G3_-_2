using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyCrawlerBasicAi_Temp : MonoBehaviour
{
    NavMeshAgent m_Agent;
    Transform m_tFormPlayer;
    Vector3 m_LastPlayerPos;


    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();

        m_tFormPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        m_LastPlayerPos = m_tFormPlayer.position;
    }


    private void Start()
    {
        m_Agent.SetDestination(m_tFormPlayer.position);
    }


    private void Update()
    {
        Vector3 currentPlayerPos = m_tFormPlayer.position;
        if(currentPlayerPos != m_LastPlayerPos)
        {
            m_Agent.SetDestination(m_tFormPlayer.position);
        }
    }
}
