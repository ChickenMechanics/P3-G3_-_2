using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletBehaviour : MonoBehaviour
{
    #region design vars
    [Header("Vfx")]
    public GameObject m_WallClashParticle;
    public float m_WallClashScale = 1.0f;
    public GameObject m_GlowParticle;
    public float m_GlowScale = 1.0f;
    public GameObject m_BodyParticle;
    public float m_BodyScale = 1.0f;
    public GameObject m_TrailRender;
    public float m_TrailScale = 1.0f;
    [Header("Properties")]
    public string m_ProjectileName;
    public float m_DamageValue = 25.0f;
    public float m_Speed;
    public float m_MaxLifetimeInSec = 5.0f;
    public bool m_IsPhysicsBased = false;
    public GameObject m_GrenadeImpactGO;
    public float m_GrenadeImpactLifetime;
    #endregion

    #region vfx
    private BulletBehaviour m_BulletBehaviour;
    private ParticleSystem m_Glow;
    private ParticleSystem m_Body;
    private TrailRenderer m_Trail;
    #endregion

    private Vector3 m_WallVfxScaleVec;
    private Rigidbody m_Rb;
    private Vector3 m_Force;
    private Vector3 m_ImpactSpot;
    private float m_CurrentLifeTime;


    // ----------------------------------------------------------------------------------------------------


    public float GetDmgValue()
    {
        return m_DamageValue;
    }


    public void InitBullet()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_WallVfxScaleVec = new Vector3(m_WallClashScale, m_WallClashScale, m_WallClashScale);

        if (m_IsPhysicsBased == true)
        {
            m_Rb.useGravity = true;
            m_Rb.isKinematic = false;
            m_Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        else
        {
            m_Rb.isKinematic = true;
            m_Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        if (m_GlowParticle != null)
        {
            m_Glow = Instantiate(m_GlowParticle.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_Glow.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_Glow.transform.parent = transform;
        }

        if (m_BodyParticle != null)
        {
            m_Body = Instantiate(m_BodyParticle.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            
            m_Body.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_Body.transform.parent = transform;
        }

        if (m_TrailRender != null)
        {
            m_Trail = Instantiate(m_TrailRender.GetComponent<TrailRenderer>(), transform.position, Quaternion.identity);
            m_Trail.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_Trail.transform.parent = transform;
        }

        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform bulletSpawnPoint, Vector3 dir)
    {
        InitBullet();

        transform.position = bulletSpawnPoint.position;

        transform.forward = dir;
        m_Force = dir * m_Speed;

#region vfx
        if (m_Glow != null)
        {
            m_Glow.transform.position = transform.position;
        }

        if (m_Body != null)
        {
            m_Body.transform.position = transform.position;
        }

        if (m_Trail != null)
        {
            m_Trail.transform.position = transform.position;
        }
#endregion

        gameObject.SetActive(true);
    }


    private void ParticlVfxDestructionAnStuff()
    {
        if (m_WallClashParticle != null)
        {
            ParticleSystem part = Instantiate(m_WallClashParticle.GetComponent<ParticleSystem>());
            if (part != null)
            {
                part.transform.forward = Camera.main.transform.forward * -1.0f;
                part.transform.position = transform.position;
                part.transform.localScale = m_WallVfxScaleVec;
                Destroy(part.gameObject, 1.5f);
            }
        }
    }


    private void OnDisable()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        ParticlVfxDestructionAnStuff();

        if(m_ProjectileName == "Grenade")
        {
            SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.ROCKET_IMPACT, transform.position);

            GameObject go = Instantiate(m_GrenadeImpactGO, transform.position, Quaternion.identity);
            BulletGrenadeAOE scr = go.GetComponent<BulletGrenadeAOE>();
            scr.m_TargetScale = go.transform.localScale;
            scr.m_DamageValue = m_DamageValue;
            scr.m_GrenadeImpactLifetime = m_GrenadeImpactLifetime;

        }

        Destroy(gameObject);
    }


    private void Update()
    {
        m_CurrentLifeTime += Time.deltaTime;

        if (m_IsPhysicsBased == false)
        {
            transform.position += m_Force * Time.deltaTime;

            if (m_CurrentLifeTime > m_MaxLifetimeInSec)
            {
                ParticlVfxDestructionAnStuff();
                Destroy(gameObject);
            }
        }
    }


    // Saved if we wan't physics based projectiles
    private void FixedUpdate()
    {
        if (m_IsPhysicsBased == true)
        {
            transform.position += m_Force * Time.fixedDeltaTime;
            if (m_CurrentLifeTime > m_MaxLifetimeInSec)
            {
                ParticlVfxDestructionAnStuff();
                Destroy(gameObject);
            }
        }
    }
}
