using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    #region Header
    protected enum State { ATTACK, DEATH, MOVE }
    protected State currentState;
    protected float distanceToPlayer;
    protected float hp;
    protected Vector3 playerPos;
    protected Vector3 lookPosition;
    protected NavMeshAgent agent;
    protected bool GetHasAttacked() { return m_HasAttacked; }
    protected void SetHasAttacked(bool hasAttacked) { m_HasAttacked = hasAttacked; }

    private bool m_HasAttacked;
    private Quaternion m_LookRotation;
    private GameObject m_PupilGO;
    private float m_TargetEyeFlasherTime;
    private float m_NowEyeFlasherTime;
    private float m_EyeFlasherFunctionOnOff;
    private float m_EyeFlasherFunctionOnOffTime;
    private bool m_EyeOnOff;
    private bool m_IsEyeFlash;
    private float m_CanTakeGrenadeDmgTime;
    private float m_CanTakeGrenadeDmgTimeNow;
    private bool m_IsTakingGrenade;
    private float m_FootSoundFreq;
    private float m_NowFootSoundTime;
    private bool m_TriggerFootSound;
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (transform.childCount > 1)    // special case for flying enemy as that object hierarchy is different then the rest
        {
            m_PupilGO = transform.GetChild(0).transform.GetChild(0).Find("Pupil").gameObject;
            m_TargetEyeFlasherTime = 0.075f;
            m_NowEyeFlasherTime = 0.0f;
            m_EyeFlasherFunctionOnOffTime = 1.0f;
            m_EyeFlasherFunctionOnOff = 0.0f;
            m_EyeOnOff = false;
            m_IsEyeFlash = false;

            m_CanTakeGrenadeDmgTime = 0.75f;
            m_CanTakeGrenadeDmgTimeNow = m_CanTakeGrenadeDmgTime;
            m_IsTakingGrenade = false;

            m_FootSoundFreq = 0.35f;
            m_NowFootSoundTime = m_FootSoundFreq;
            m_TriggerFootSound = true;
        }
    }

    private void Update()
    {
        if (m_IsTakingGrenade)
            CanTakeGrenadeDmgCounter();

        if (m_PupilGO != null && m_IsEyeFlash)
        {
            m_EyeFlasherFunctionOnOff += Time.deltaTime;
            if (m_EyeFlasherFunctionOnOff > m_EyeFlasherFunctionOnOffTime)
            {
                m_EyeFlasherFunctionOnOff = 0.0f;
                m_IsEyeFlash = false;
                m_EyeOnOff = false;
                m_PupilGO.SetActive(true);
            }

            EyeFlasherLogic();
        }

        if (m_TriggerFootSound == true)
        {
            m_TriggerFootSound = false;
            SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.ENEMY_FOOTSTEPS, transform.position);
        }
        else
        {
            FootStepSoundTimer();
        }
    }

    private void FootStepSoundTimer()
    {
        m_NowFootSoundTime -= Time.deltaTime;
        if (m_NowFootSoundTime < 0.0f)
        {
            m_NowFootSoundTime = m_FootSoundFreq;
            m_TriggerFootSound = true;
        }
    }

    private void CanTakeGrenadeDmgCounter()
    {
        m_CanTakeGrenadeDmgTimeNow -= Time.deltaTime;
        if (m_CanTakeGrenadeDmgTimeNow < 0.0f)
        {
            m_CanTakeGrenadeDmgTimeNow = m_CanTakeGrenadeDmgTime;
            m_IsTakingGrenade = false;
        }
    }

    private void EyeFlasherLogic()
    {
        m_NowEyeFlasherTime += Time.deltaTime;
        if (m_NowEyeFlasherTime > m_TargetEyeFlasherTime)
        {
            m_NowEyeFlasherTime = 0.0f;
            m_EyeOnOff = !m_EyeOnOff;
            m_PupilGO.SetActive(m_EyeOnOff);
        }
    }

    protected void MoveTowardsPlayer()
    {
        UpdateDistanceToPlayer();

        if (lookPosition != Vector3.zero)
            m_LookRotation = Quaternion.LookRotation(lookPosition);

        transform.rotation = Quaternion.Slerp(
            transform.rotation, m_LookRotation, 0.1f);

        agent.SetDestination(playerPos);
    }

    protected void UpdateDistanceToPlayer()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        lookPosition = playerPos - transform.position;

        distanceToPlayer = lookPosition.magnitude;

        lookPosition.y = 0;
    }

    private void OnTriggerEnter(Component other)
    {
        if (other.gameObject.layer == 13 /*projectile*/ &&
            other.gameObject.GetComponent<BulletBehaviour>() != null)
        {
            TakeDamage(other.GetComponent<BulletBehaviour>().GetDmgValue());
#if DEBUG
            if (SoundManager.GetInstance != null)
#endif
            {
                SoundManager.GetInstance.PlaySoundClip(
                    SoundManager.ESoundClip.CRAWLER_HURT,
                    other.transform.position);
            }

            if (m_PupilGO != null)
                m_IsEyeFlash = true;
        }
    }

    private void OnTriggerStay(Component other)
    {
        //if (m_IsTakingGrenade == false &&
        //    other.gameObject.layer == 17 /*grenade*/ &&
        //    other.gameObject.GetComponent<BulletGrenadeAOE>() != null)
        if (other.gameObject.layer == 17 /*grenade*/ &&
            other.gameObject.GetComponent<BulletGrenadeAOE>() != null)
        {
            TakeDamage(other.GetComponent<BulletGrenadeAOE>().GetDmgValue());

#if DEBUG
            if (SoundManager.GetInstance != null)
#endif
            {
                SoundManager.GetInstance.PlaySoundClip(
                    SoundManager.ESoundClip.CRAWLER_HURT,
                    other.transform.position);
            }

            if (m_PupilGO != null)
                m_IsEyeFlash = true;
        }
    }

    public void TakeDamage(float damageValue)
    {
        hp -= damageValue;
        if (hp <= 0)
            currentState = State.DEATH;
    }
}