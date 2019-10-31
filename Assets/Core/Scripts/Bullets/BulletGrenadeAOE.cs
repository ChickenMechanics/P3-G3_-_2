using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeAOE : MonoBehaviour
{
    public float m_CollisionScaleIncreaser;
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

    }


    public float GetDmgValue()
    {
        return m_DamageValue;
    }


    private void Start()
    {
        m_ScaleIncreaser = new Vector3(m_CollisionScaleIncreaser, m_CollisionScaleIncreaser, m_CollisionScaleIncreaser);
        gameObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject effect1 = Instantiate(m_Vfx1, transform.position, Quaternion.identity);
        GameObject effect2 = Instantiate(m_Vfx2, transform.position, Quaternion.identity);
        Destroy(effect1, effect1.GetComponent<ParticleSystem>().main.duration);
        Destroy(effect2, effect2.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject, m_GrenadeImpactLifetime);
    }


    private void Update()
    {
        StateUpdate();
    }
}
