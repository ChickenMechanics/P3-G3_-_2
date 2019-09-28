using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager GetInstance { get; private set; }

    #region design vars
    public float m_ComboTimeInSecMax = 1.0f;
    public float m_ComboScaler = 1.5f;
    #endregion

    // TODO: Changed all that is named combo to chain
    #region get / set
    [HideInInspector]
    public float ChainTimeLeft { get; private set; }
    [HideInInspector]
    public float SpareChainTime{ get; private set; }
    [HideInInspector]
    public float PlayerScore { get; private set; }
    [HideInInspector]
    public int CurrentChain { get; private set; }
    [HideInInspector]
    public float CurrentComboMultiplier { get; private set; }
    [HideInInspector]
    public int TotalChains { get; private set; }
    [HideInInspector]
    public int LongestChain { get; private set; }
    #endregion

    private bool m_ComboAlive;

    public enum EText
    {
        SCORE = 0,
        TOTAL_CHAINS,
        LONGEST_CHAIN,
        CHAIN_TIME_LEFT,
        SPARE_CHAIN_TIME,
        CURRENT_CHAIN,
        CURRENT_MULTI,
        SIZE
    }


    public float GetComboTimeMax()
    {
        return m_ComboTimeInSecMax;
    }


    public void AddComboPoints(float value)    //  Call this for everything included in the combo points system
    {
        ++CurrentChain;
        if (CurrentChain == 1)
        {
            m_ComboAlive = true;
        }

        ComboEvaluator(value);
    }


    public void AddVanillaPoints(float value)   //  Call this for vanilla points
    {
        PlayerScore += value;
    }


    private void UpdatePoints()
    {
        if (m_ComboAlive == true)
        {
            ChainTimeLeft -= Time.deltaTime;

            ComboUpdater();
        }
    }


    private void ComboEvaluator(float value)
    {
        if (CurrentChain > 1)  // Each kill that is chained in a combo is worth more then the previous
        {
            SpareChainTime = ChainTimeLeft;
            CurrentComboMultiplier *= m_ComboScaler;
        }

        if (CurrentChain < 2)     // Normalizes the multiplier if it's the first enemy killed
        {
            CurrentComboMultiplier /= CurrentComboMultiplier;
        }

        PlayerScore += value * CurrentComboMultiplier;  // TODO: If time bonus or whatever exists, implement here

        // Combo alive
        ChainTimeLeft = m_ComboTimeInSecMax;
    }


    private void ComboUpdater()
    {
        // Combo dead
        if (ChainTimeLeft < 0.0f)
        {
            if (CurrentChain > LongestChain)
            {
                LongestChain = CurrentChain;
            }

            if(CurrentChain > 1)
            {
                ++TotalChains;
            }

            ChainTimeLeft = 0.0f;
            SpareChainTime = 0.0f;
            CurrentComboMultiplier = 1.0f;
            CurrentChain = 0;
            m_ComboAlive = false;
        }
    }


    public void ResetPlayer()
    {
        ChainTimeLeft = 0.0f;
        SpareChainTime = 0.0f;
        PlayerScore = 0.0f;
        CurrentComboMultiplier = 1.0f;
        CurrentChain = 0;
        TotalChains = 0;
        LongestChain = 0;
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

        ResetPlayer();
    }


    private void Awake()
    {
        Init();
    }


    private void Update()
    {
        UpdatePoints();
    }
}
