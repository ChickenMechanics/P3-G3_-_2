using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyCrawlerAnimation : MonoBehaviour
{
    private Animator m_Animator;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Walk", false);

            m_Animator.SetBool("Idle", true);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Idle", false);

            m_Animator.SetBool("Walk", true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Idle", false);

            m_Animator.SetBool("Attack", true);
        }
    }
}
