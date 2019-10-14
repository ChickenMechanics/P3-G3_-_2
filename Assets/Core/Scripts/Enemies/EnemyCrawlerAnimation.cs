using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Should update this with a manager or something
public class EnemyCrawlerAnimation : MonoBehaviour
{
    [HideInInspector]
    public Animator m_Animator { private set; get; }
    private string[] m_ClipNames = { "Walk", "Attack" };
    private int m_CurrentAnimId;

    public enum EAnimCrawler
    {
        WALK,
        ATTACK,
        SIZE
    }


    public void SetAnim(EAnimCrawler anim)
    {
        int iAnim = (int)anim;
        if(iAnim == m_CurrentAnimId)
        {
            return;
        }

        m_Animator.SetBool(m_ClipNames[m_CurrentAnimId], false);
        m_Animator.SetBool(m_ClipNames[iAnim], true);

        m_CurrentAnimId = iAnim;
    }


    public void ResetAnim()
    {
        int size = (int)EAnimCrawler.SIZE;
        for (int i = 0; i < size; ++i)
        {
            m_Animator.SetBool(m_ClipNames[i], false);
        }

        m_CurrentAnimId = (int)EAnimCrawler.WALK;
        m_Animator.SetBool(m_ClipNames[m_CurrentAnimId], true);
    }


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_CurrentAnimId = (int)EAnimCrawler.WALK;

        ResetAnim();
    }


    private void Update()
    {
        #region debug animation on keypress
#if DEBUG
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    SetAnim(EAnimCrawler.IDLE);
        //}

        if (Input.GetKeyDown(KeyCode.O))
        {
            SetAnim(EAnimCrawler.WALK);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetAnim(EAnimCrawler.ATTACK);
        }
#endif
        #endregion
    }
}
