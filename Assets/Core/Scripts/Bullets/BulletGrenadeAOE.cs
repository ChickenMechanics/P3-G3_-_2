using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeAOE : MonoBehaviour
{
    public GameObject m_Vfx1;
    public GameObject m_Vfx2;
    public GameObject m_Vfx3;

    [HideInInspector] public float m_DamageValue;
    [HideInInspector] public float m_GrenadeImpactLifetime;


    public float GetDmgValue()
    {
        return m_DamageValue;
    }


    private void Start()
    {
        
    }


    private void Update()
    {
        Destroy(gameObject, m_GrenadeImpactLifetime);
    }
}
