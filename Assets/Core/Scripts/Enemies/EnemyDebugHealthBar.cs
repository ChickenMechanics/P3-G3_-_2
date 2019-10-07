using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyDebugHealthBar : MonoBehaviour
{
    private ScoreEnemyBasic m_ScoreEnemyBasicScr;
    private Image m_HealthBar;
    private Vector3 m_PosOffset;
    

    private void Awake()
    {
        transform.Find("HealthCanvas").GetComponent<Canvas>().worldCamera = Camera.main;
        m_ScoreEnemyBasicScr = transform.parent.GetComponent<ScoreEnemyBasic>();
        m_HealthBar = transform.Find("HealthCanvas").transform.Find("HealthImage").GetComponent<Image>();

        m_HealthBar.transform.forward = transform.parent.forward;
        m_PosOffset = new Vector3(0.0f, 1.75f, 0.0f);
        m_HealthBar.transform.position += m_PosOffset;
    }


    private void Update()
    {
        m_HealthBar.fillAmount = m_ScoreEnemyBasicScr.GetCurrentHealth / m_ScoreEnemyBasicScr.GetBaseHealth;
    }
}
