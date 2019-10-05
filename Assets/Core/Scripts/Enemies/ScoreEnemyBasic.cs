using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEnemyBasic : MonoBehaviour
{
    #region design vars
    public float m_ScoreValue = 100.0f;
    public float m_Health = 100.0f;
    public float m_ExplosiveDamage = 25.0f;
    #endregion

    private PlayerManager m_PlayerManScr;
    private ScoreManager m_ScoreManScr;
    private SoundManager m_SoundManScr;
    private GameObject m_DebugHealthBar;
    [HideInInspector]
    public float GetBaseHealth { private set; get; }
    [HideInInspector]
    public float GetCurrentHealth { private set; get; }


    //----------------------------------------------------------------------------------------------------


    public void DecreaseHealth(float value)
    {
        GetCurrentHealth -= value;
    } 


    private void Awake()
    {
        PlayerManager playerMan = PlayerManager.GetInstance;
        if (playerMan != null)
        {
            m_PlayerManScr = playerMan;
        }

        ScoreManager scoreMan = ScoreManager.GetInstance;
        if (scoreMan != null)
        {
            m_ScoreManScr = scoreMan;
        }

        SoundManager soundMan = SoundManager.GetInstance;
        if (soundMan != null)
        {
            m_SoundManScr = soundMan;
        }

        GetBaseHealth = m_Health;
        GetCurrentHealth = GetBaseHealth;

#if DEBUG
        GameObject resource = (GameObject)Resources.Load("Prefabs/DebugEnemyHealth");
        m_DebugHealthBar = Instantiate(resource, transform.position, Quaternion.identity, transform);
#endif
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)    // == projectile
        {
#if DEBUG
            if(m_SoundManScr != null)
#endif
            {
                m_SoundManScr.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_BULLET_DAMAGE, other.transform.position);
            }
        }

        // Self destructed
        if (other.gameObject.layer == 8)     // 8 == player
        {
//#if DEBUG
//            if (m_ScoreManScr != null)
//#endif
//            {
//                m_ScoreManScr.AddComboPoints(m_ScoreValue);
//            }
#if DEBUG
            if (m_PlayerManScr != null)
#endif
            {
                m_PlayerManScr.DecreaseHealth(m_ExplosiveDamage);
            }

            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (GetCurrentHealth <= 0.0f)
        {
#if DEBUG
            if (m_ScoreManScr != null)
#endif
            {
                m_ScoreManScr.AddComboPoints(m_ScoreValue);
            }

#if DEBUG
            if (m_SoundManScr != null)
#endif
            {
                m_SoundManScr.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_DEATH, transform.position);
                m_SoundManScr.PlaySoundClip(SoundManager.ESoundClip.SCORE_POINTS_BASIC, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
