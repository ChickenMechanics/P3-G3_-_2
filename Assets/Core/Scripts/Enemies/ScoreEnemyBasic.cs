using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEnemyBasic : MonoBehaviour
{
    #region design vars
    public float m_ScoreValue = 100.0f;
    public float m_Health = 100.0f;
    #endregion

    private ScoreManager m_ScoreManager;


    public float GetHealth()
    {
        return m_Health;
    }


    public void TakeDmg(float value)
    {
        m_Health -= value;
    }


    private void FlipMaterialColorOnDmgTaken()
    {

    }   


    private void Awake()
    {
        m_ScoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "BulletBasic")
        //{
            // TODO: Fix health - bullet dmg
            //m_Health -= 100.0f;
            //if(m_Health >= 0.0f)
            //{
            //    m_ScoreManager.AddComboPoints(m_ScoreValue);
            //}
        //}
    }


    private void Update()
    {
        if (m_Health <= 0.0f)
        {
            if(m_ScoreManager != null)
            {
                m_ScoreManager.AddComboPoints(m_ScoreValue);
                Destroy(gameObject);
            }
        }
    }
}
