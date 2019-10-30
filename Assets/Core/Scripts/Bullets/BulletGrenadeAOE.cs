using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeAOE : MonoBehaviour
{
    [HideInInspector] public float m_DamageValue;
    [HideInInspector] public float m_GrenadeImpactLifetime;

    public float GetDmgValue()
    {
        return m_DamageValue;
    }

    private void Update()
    {
        Destroy(gameObject, m_GrenadeImpactLifetime);
    }
}
