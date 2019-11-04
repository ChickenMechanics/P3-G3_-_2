using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager GetInstance { get; private set; }

    #region design vars
    public float m_ComboTimeInSecMax = 1.0f;
    public float m_ComboScaler;
    public float m_BulletTimeActiveTime;
    public float m_BulletTimeScale;
    public float m_BulletTimeFov;
    #endregion

    #region get / set
    [HideInInspector]
    public float GetBaseChainTime { get; private set; }
    [HideInInspector]
    public float GetChainTimeLeft { get; private set; }
    [HideInInspector]
    public float GetSpareChainTime{ get; private set; }
    [HideInInspector]
    public float GetPlayerScore { get; private set; }
    [HideInInspector]
    public int GetCurrentChain { get; private set; }
    [HideInInspector]
    public float GetCurrentComboMultiplier { get; private set; }
    [HideInInspector]
    public int GetTotalChains { get; private set; }
    [HideInInspector]
    public int GetLongestChain { get; private set; }
    #endregion

    private bool m_ComboAlive;

    //private float m_BulletTimeFreq;
    private float m_NowBulletTime;
    private bool m_TriggerBulletTime;
    private int m_PreviousComboMulti;


    //----------------------------------------------------------------------------------------------------


    public float GetComboTimeMax()
    {
        return m_ComboTimeInSecMax;
    }


    public void AddComboPoints(float value)    //  Call this for everything included in the combo points system
    {
        ++GetCurrentChain;
        if (GetCurrentChain == 1)
        {
            m_ComboAlive = true;
        }

        ComboEvaluator(value);
    }


    public void AddVanillaPoints(float value)   //  Call this for vanilla points
    {
        GetPlayerScore += value;
    }


    private void UpdatePoints()
    {
        if (m_ComboAlive == true)
        {
            GetChainTimeLeft -= Time.deltaTime;

            ComboUpdater();
        }
    }


    private void ComboEvaluator(float value)
    {
        if (GetCurrentChain > 1)  // Each kill that is chained in a combo is worth more then the previous
        {
            GetSpareChainTime = GetChainTimeLeft;
            GetCurrentComboMultiplier *= m_ComboScaler;
        }

        if (GetCurrentChain < 2)     // Normalizes the multiplier if it's the first enemy killed
        {
            GetCurrentComboMultiplier /= GetCurrentComboMultiplier;
        }

        GetPlayerScore += value * GetCurrentComboMultiplier;  // TODO: If time bonus or whatever exists, implement here

        // Combo alive
        GetChainTimeLeft = m_ComboTimeInSecMax;
    }


    private void ComboUpdater()
    {
        // Combo dead
        if (GetChainTimeLeft < 0.0f)
        {
            if (GetCurrentChain > GetLongestChain)
            {
                GetLongestChain = GetCurrentChain;
            }

            if(GetCurrentChain > 1)
            {
                ++GetTotalChains;
            }

            GetChainTimeLeft = 0.0f;
            GetSpareChainTime = 0.0f;
            GetCurrentComboMultiplier = 1.0f;
            GetCurrentChain = 0;
            m_ComboAlive = false;
        }
    }


    public void ResetPlayerStats()
    {
        GetBaseChainTime = m_ComboTimeInSecMax;
        GetChainTimeLeft = 0.0f;
        GetSpareChainTime = 0.0f;
        GetPlayerScore = 0.0f;
        GetCurrentComboMultiplier = 1.0f;
        GetCurrentChain = 0;
        GetTotalChains = 0;
        GetLongestChain = 0;
        m_ComboAlive = false;
    }


    private void Init()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        DontDestroyOnLoad(gameObject);

        if (m_ComboScaler < 0.0f)
        {
            m_ComboScaler = 0.0f;
        }

        // fuckery
        m_BulletTimeActiveTime = 0.25f;
        m_NowBulletTime = m_BulletTimeActiveTime;
        m_TriggerBulletTime = false;
        m_PreviousComboMulti = (int)GetCurrentComboMultiplier;

        ResetPlayerStats();
    }


    private void Awake()
    {
        Init();
    }


    private void Update()
    {
        UpdatePoints();
        BulletTime();
    }


    private void BulletTime()
    {
        if (m_TriggerBulletTime == true)
        {
            BulletTimeOutTimer();
        }
        else if (m_PreviousComboMulti < (int)GetCurrentComboMultiplier &&
            m_TriggerBulletTime == false)
        {
            m_TriggerBulletTime = true;
        }

        // timescale & fov hacks
        if (m_TriggerBulletTime == true &&
            Camera.main.fieldOfView > m_BulletTimeFov)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, m_BulletTimeScale, 0.5f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, m_BulletTimeFov, 0.05f);
        }
        else if (m_TriggerBulletTime == false &&
            Camera.main.fieldOfView != 60.0f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, 0.5f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60.0f, 0.2f);
            if(Camera.main.fieldOfView > 60.0f)
            {
                Camera.main.fieldOfView = 60.0f;
            }
        }

        m_PreviousComboMulti = (int)GetCurrentComboMultiplier;
    }


    private void BulletTimeOutTimer()
    {
        m_NowBulletTime -= Time.deltaTime;
        if (m_NowBulletTime < 0.0f)
        {
            m_NowBulletTime = m_BulletTimeActiveTime;
            m_TriggerBulletTime = false;
        }
    }
}
