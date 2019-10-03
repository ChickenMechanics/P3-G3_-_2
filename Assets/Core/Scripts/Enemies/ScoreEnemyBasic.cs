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


    //----------------------------------------------------------------------------------------------------


    public float GetHealth()
    {
        return m_Health;
    }


    public void DecreaseHealth(float value)
    {
        m_Health -= value;
    }


    private void FlipMaterialColorOnDmgTaken()
    {

    }   


    private void Awake()
    {
        ScoreManager scoreMan = FindObjectOfType<ScoreManager>();
        if (scoreMan != null)
        {
            m_ScoreManager = scoreMan;
        }
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
        // Killed
        if (m_Health <= 0.0f)
        {
            if(m_ScoreManager != null)
            {
                m_ScoreManager.AddComboPoints(m_ScoreValue);
            }

            Destroy(gameObject);
        }
    }
}
