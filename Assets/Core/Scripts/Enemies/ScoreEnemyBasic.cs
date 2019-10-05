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

    private ScoreManager m_ScoreManager;
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
        ScoreManager scoreMan = FindObjectOfType<ScoreManager>();
        if (scoreMan != null)
        {
            m_ScoreManager = scoreMan;
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
        // Self destructed
        if(other.gameObject.layer == 8)     // 8 == Player
        {
            if (m_ScoreManager != null)
            {
                m_ScoreManager.AddComboPoints(m_ScoreValue);
            }

            PlayerManager.GetInstance.DecreaseHealth(m_ExplosiveDamage);

            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (GetCurrentHealth <= 0.0f)
        {
            if(m_ScoreManager != null)
            {
                m_ScoreManager.AddComboPoints(m_ScoreValue);
            }

            Destroy(gameObject);
        }
    }
}
