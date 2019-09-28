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
    public float m_DamageValue = 25.0f;
    public float m_Speed = 50.0f;
    public float m_DropOff = 0.0f;
    public float m_MaxLifetimeInSec = 5.0f;
    #endregion

    #region vfx
    private BulletBehaviour m_BulletBehaviour;
    private ParticleSystem m_WallClash;
    private ParticleSystem m_Glow;
    private ParticleSystem m_Body;
    private TrailRenderer m_Trail;
    #endregion

    private Rigidbody m_Rb;
    private Vector3 m_Force;
    private float m_CurrentLifeTime;


    public void InitBullet()
    {
        m_Rb = GetComponent<Rigidbody>();
        //if (m_IsPhysicsBased == true)
        //{
        //    m_Rb.useGravity = true;
        //    m_Rb.isKinematic = false;
        //    m_Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //}
        //else
        {
            m_Rb.isKinematic = true;
            m_Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        #region vfx
        if (m_WallClashParticle != null)
        {
            m_WallClash = Instantiate(m_WallClashParticle.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_WallClash.Stop();
            m_WallClash.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_WallClash.transform.localScale = new Vector3(m_WallClashScale, m_WallClashScale, m_WallClashScale);
            m_WallClash.transform.parent = transform;
        }

        if (m_GlowParticle != null)
        {
            m_Glow = Instantiate(m_GlowParticle.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_Glow.Stop();
            m_Glow.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_Glow.transform.localScale = new Vector3(m_GlowScale, m_GlowScale, m_GlowScale);
            m_Glow.transform.parent = transform;
        }

        if (m_BodyParticle != null)
        {
            m_Body = Instantiate(m_BodyParticle.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_Body.Stop();
            m_Body.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_Body.transform.localScale = new Vector3(m_BodyScale, m_BodyScale, m_BodyScale);
            m_Body.transform.parent = transform;
        }

        if (m_TrailRender != null)
        {
            m_Trail = Instantiate(m_TrailRender.GetComponent<TrailRenderer>(), transform.position, Quaternion.identity);
            m_Trail.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_Trail.transform.localScale = new Vector3(m_TrailScale, m_TrailScale, m_TrailScale);
            m_Trail.transform.parent = transform;
        }
        #endregion

        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform bulletSpawnPoint, Vector3 dir)
    {
        transform.position = bulletSpawnPoint.position;

        transform.forward = dir;
        m_Force = (dir * m_Speed) + new Vector3(0.0f, m_DropOff, 0.0f);

        #region vfx
        if(m_WallClash != null)
        {
            m_WallClash.transform.rotation = Camera.main.transform.rotation;
            //m_WallClash.transform.position = vfxSpawnPoint;
        }

        if (m_Glow != null)
        {
            m_Glow.transform.position = transform.position;
            m_Glow.Play();
        }

        if (m_Body != null)
        {
            m_Body.transform.position = transform.position;
            m_Body.Play();
        }

        if (m_Trail != null)
        {
            m_Trail.transform.position = transform.position;
        }
        #endregion

        gameObject.SetActive(true);
    }


    private void OnEnable()
    {
        gameObject.SetActive(true);
    }


    private void OnDisable()
    {
        gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        Destroy(this);
    }


    private void OnTriggerEnter(Collider other)
    {
        // TODO: If time, move vfx things to it's own script
        if (m_WallClashParticle != null)
        {
            m_WallClash.transform.parent = null;
            m_WallClash.transform.position = transform.position;
            m_WallClash.Play();
        }


        // Projectiles might pass thru some type of force fileds or whatever so some conditional are needed
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<ScoreEnemyBasic>().TakeDmg(m_DamageValue);
            Destroy(this);  // Change this to gameObject as I'm unsure if this destroys the whole object orjust the script component
        }
        else if (other.CompareTag("DestroyBullet"))
        {
            Destroy(this);
        }
    }


    private void Update()
    {
        m_CurrentLifeTime += Time.deltaTime;

        //if (m_IsPhysicsBased == false)
        {
            transform.position += m_Force * Time.deltaTime;
            if (m_CurrentLifeTime > m_MaxLifetimeInSec)
            {
                Destroy(this);
            }
        }
    }


    // Saved if we wan't physics based projectiles
    //private void FixedUpdate()
    //{
    //    if (m_IsPhysicsBased == true)
    //    {
    //        transform.position += m_Force * Time.fixedTime;
    //        if (m_CurrentLifeTime > m_MaxLifetimeInSec)
    //        {
    //            Destroy(this);
    //        }
    //    }
    //}
}
