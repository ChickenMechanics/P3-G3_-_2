using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    #region Header
    protected enum State { ATTACK, DEATH, MOVE }
    protected State currentState;
    protected float distanceToPlayer;
    protected float hp;
    protected Vector3 playerPos;
    protected Vector3 lookPosition;

    private Quaternion m_LookRotation;
    #endregion

    private GameObject m_PupilGO;
    private float m_TargetEyeFlasherTime;
    private float m_NowEyeFlasherTime;
    private float m_EyeFlasherFunctionOnOff;
    private float m_EyeFlasherFunctionOnOffTime;
    private bool m_EyeOnOff;
    private bool m_IsEyeFlash;


    private void Awake()
    {
        m_PupilGO = transform.GetChild(0).transform.GetChild(0).Find("Pupil").gameObject;
        m_TargetEyeFlasherTime = 0.1f;
        m_NowEyeFlasherTime = 0.0f;
        m_EyeFlasherFunctionOnOffTime = 1.0f;
        m_EyeFlasherFunctionOnOff = 0.0f;
        m_EyeOnOff = false;
        m_IsEyeFlash = false;
    }


    private void Update()
    {
        if(m_IsEyeFlash == true)
        {
            m_EyeFlasherFunctionOnOff += Time.deltaTime;
            if(m_EyeFlasherFunctionOnOff > m_EyeFlasherFunctionOnOffTime)
            {
                m_EyeFlasherFunctionOnOff = 0.0f;
                m_IsEyeFlash = false;
                m_EyeOnOff = false;
                m_PupilGO.SetActive(true);
            }

            EyeFlasherLogic();
        }
    }


    private void EyeFlasherLogic()
    {
        m_NowEyeFlasherTime += Time.deltaTime;
        if(m_NowEyeFlasherTime > m_TargetEyeFlasherTime)
        {
            m_NowEyeFlasherTime = 0.0f;
            m_EyeOnOff = !m_EyeOnOff;
            m_PupilGO.SetActive(m_EyeOnOff);
        }
    }


    protected void MoveTowardsPlayer(Transform agentTransform, NavMeshAgent agent)
    {
        transform.position = agentTransform.position;
        var rotation = agentTransform.rotation;

        UpdateDistanceToPlayer();

        if (lookPosition != Vector3.zero)
            m_LookRotation = Quaternion.LookRotation(lookPosition);

        agentTransform.rotation = Quaternion.Slerp(rotation, m_LookRotation, 0.1f);

        agent.SetDestination(playerPos);
    }

    protected void UpdatePlayerPos()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    protected void UpdateDistanceToPlayer()
    {
        UpdatePlayerPos();

        lookPosition = playerPos - transform.position;
        
        distanceToPlayer = lookPosition.magnitude;

        lookPosition.y = 0;
    }

    private void OnTriggerEnter(Component other)
    {
        if (other.gameObject.layer == 13)   // == projectile
        {
            if (other.gameObject.GetComponent<BulletBehaviour>() != null)
            {
                TakeDamage(other.GetComponent<BulletBehaviour>().GetDmgValue());
#if DEBUG
                if (SoundManager.GetInstance != null)
#endif
                {
                    SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_HURT, other.transform.position);
                }

                m_IsEyeFlash = true;
            }

            //TakeDamage(other.GetComponent<BulletBehaviour>().m_DamageValue);
        }
    }

    private void OnTriggerStay(Component other)
    {
        if (other.gameObject.layer == 13)   // == projectile
        {
            if (other.gameObject.GetComponent<BulletGrenadeAOE>() != null)
            {
                TakeDamage(other.GetComponent<BulletGrenadeAOE>().GetDmgValue());
#if DEBUG
                if (SoundManager.GetInstance != null)
#endif
                {
                    SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.CRAWLER_HURT, other.transform.position);
                }
            }

            //TakeDamage(other.GetComponent<BulletBehaviour>().m_DamageValue);
        }
    }

    public void TakeDamage(float damageValue)
    {
        hp -= damageValue;

        if (hp <= 0)
            currentState = State.DEATH;
    }
}