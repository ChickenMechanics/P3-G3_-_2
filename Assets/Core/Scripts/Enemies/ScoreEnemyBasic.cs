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
            DecreaseHealth(other.GetComponent<BulletBehaviour>().GetDamageValue());
#if DEBUG
            if(SoundManager.GetInstance != null)
#endif
            {
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_BULLET_DAMAGE, other.transform.position);
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
            if (PlayerManager.GetInstance != null)
#endif
            {
                PlayerManager.GetInstance.DecreaseHealth(m_ExplosiveDamage);
            }

            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (GetCurrentHealth <= 0.0f)
        {
#if DEBUG
            if (ScoreManager.GetInstance != null)
#endif
            {
                ScoreManager.GetInstance.AddComboPoints(m_ScoreValue);
            }

#if DEBUG
            if (SoundManager.GetInstance != null)
#endif
            {
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_DEATH, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
