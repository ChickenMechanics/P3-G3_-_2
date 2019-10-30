using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeAOE : MonoBehaviour
{
    [HideInInspector] public float m_DamageValue;

    public float GetDmgValue()
    {
        return m_DamageValue;
    }
}
