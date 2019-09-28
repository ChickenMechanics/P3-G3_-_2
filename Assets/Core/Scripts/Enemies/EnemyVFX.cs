using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVFX : MonoBehaviour
{
    #region design vars
    [Header("Vfx")]
    public GameObject m_ExplosionFireVfx;
    public GameObject m_ExplosionSmokeVfx;
    public float m_ExplosionFireScale= 1.0f;
    public float m_ExplosionSmokeScale = 1.0f;
    #endregion

    private ParticleSystem m_ExplosionFireParticle;
    private ParticleSystem m_ExplosionSmokeParticle;


    private void Awake()
    {
        GameObject parent = new GameObject("VfxEnemyDeathFolder");
        parent.transform.rotation = transform.rotation;
        parent.transform.position = transform.position;

        if (m_ExplosionFireVfx != null)
        {
            m_ExplosionFireParticle = Instantiate(m_ExplosionFireVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_ExplosionFireParticle.Stop();
            m_ExplosionFireParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_ExplosionFireParticle.transform.localScale = new Vector3(m_ExplosionFireScale, m_ExplosionFireScale, m_ExplosionFireScale);

            m_ExplosionFireParticle.transform.parent = parent.transform;
        }

        if (m_ExplosionSmokeVfx != null)
        {
            m_ExplosionSmokeParticle = Instantiate(m_ExplosionSmokeVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_ExplosionSmokeParticle.Stop();
            m_ExplosionSmokeParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_ExplosionSmokeParticle.transform.localScale = new Vector3(m_ExplosionSmokeScale, m_ExplosionSmokeScale, m_ExplosionSmokeScale);

            m_ExplosionSmokeParticle.transform.parent = parent.transform;
        }
    }


    private void OnDestroy()
    {
        if (m_ExplosionFireParticle != null)
        {
            m_ExplosionFireParticle.transform.position = transform.position;
            m_ExplosionFireParticle.Play();
        }

        if (m_ExplosionSmokeParticle != null)
        {
            m_ExplosionSmokeParticle.transform.position = transform.position;
            m_ExplosionSmokeParticle.Play();
        }
    }
}
