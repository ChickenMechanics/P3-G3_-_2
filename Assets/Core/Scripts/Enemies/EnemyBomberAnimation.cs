using System;
using UnityEngine;


// Should update this with a manager or something
public class EnemyBomberAnimation : MonoBehaviour
{
    [HideInInspector]
    public Animator animator { private set; get; }
    private readonly string[] m_ClipNames = { "Walk", "Explode" };
    private int m_CurrentAnimId;

    public enum EAnimBomber
    {
        WALK,
        EXPLODE
    }
    
    public void SetAnim(EAnimBomber anim)
    {
        var iAnim = (int)anim;
        if(iAnim == m_CurrentAnimId) return;

        animator.SetBool(m_ClipNames[m_CurrentAnimId], false);
        animator.SetBool(m_ClipNames[iAnim], true);

        m_CurrentAnimId = iAnim;
    }

    public void ResetAnim()
    {
        var size = Enum.GetNames(typeof(EAnimBomber)).Length;

        for (var i = 0; i < size; ++i)
            animator.SetBool(m_ClipNames[i], false);

        m_CurrentAnimId = (int)EAnimBomber.WALK;
        animator.SetBool(m_ClipNames[m_CurrentAnimId], true);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        m_CurrentAnimId = (int)EAnimBomber.WALK;

        ResetAnim();
    }
}
