using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeAOE : MonoBehaviour
{
    public float m_IncreaseValue;
    public GameObject m_Vfx1;
    public GameObject m_Vfx2;
    public GameObject m_Vfx3;

    [HideInInspector] public Vector3 m_TargetScale;
    [HideInInspector] public float m_DamageValue;
    [HideInInspector] public float m_GrenadeImpactLifetime;

    private Vector3 m_ScaleIncreaser;


    private enum EState
    {
        INCREASE,
        NORMAL_MODE
    }
    EState m_NowState = EState.INCREASE;


    private void StateUpdate()
    {
        switch(m_NowState)
        {
            case EState.INCREASE:       Increase();     break;
            case EState.NORMAL_MODE:    NormalMode();   break;
        }
    }


    private void Increase()
    {
        transform.localScale += m_ScaleIncreaser * Time.deltaTime;

        if (transform.localScale.x > m_TargetScale.x)
        {
            transform.localScale = m_TargetScale;
            m_NowState = EState.NORMAL_MODE;
        }
    }


    private void NormalMode()
    {
        int hej = 7;
    }


    public float GetDmgValue()
    {
        return m_DamageValue;
    }


    private void Start()
    {
        m_ScaleIncreaser = new Vector3(m_IncreaseValue, m_IncreaseValue, m_IncreaseValue);
        gameObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject go1 = Instantiate(m_Vfx1, transform.position, Quaternion.identity);
        Destroy(go1, m_GrenadeImpactLifetime);
        Destroy(gameObject, m_GrenadeImpactLifetime);
    }


    private void Update()
    {
        StateUpdate();
    }
}
