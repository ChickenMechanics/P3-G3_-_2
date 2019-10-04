using System;
using UnityEngine;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    public static HUDManager GetInstance { get; private set; }

    #region scr ref
    private PlayerManager m_PlayerMan;
    private GunManager m_GunMan;
    private ScoreManager m_ScoreMan;
    #endregion

    private Image m_HealthBarImg;
    private Image m_ComboMeterImg;
    private Image m_WaveMeterImg;
    private Text m_ScoreTxt;
    private Text m_ChainTxt;
    private Text m_Multiplier;
    private Text m_GunBulletText;
    private float m_PrevHealth;
    private float m_PrevComboMeter;
    private int m_ScoreComboDecimalPoints;

    private float m_MagEmptyBlinkTime;
    private float m_FlasherThingTimer;
    private bool m_bFlasherThing;

    // things that probably goes to endscreen
    private Text m_SpareChainTimeTxt;
    private Text m_TotalChainsTxt;
    private Text m_LongestChain;

    // guns / bulllets
    public enum EPlayerText
    {
        HEALTH = 0,
        SIZE
    }


    //----------------------------------------------------------------------------------------------------


    private void Init()
    {
        Transform canvas = transform.Find("HUDCanvas");

        // player status
        m_PlayerMan = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        m_HealthBarImg = canvas.transform.Find("PlayerStatus").transform.Find("HealthSliderImage").GetComponent<Image>();
        m_PrevHealth = m_PlayerMan.GetCurrentHealth;

        // guns / bulllets
        m_GunMan = GameObject.Find("GunManager").GetComponent<GunManager>();
        m_GunBulletText = canvas.transform.Find("GunBullet").transform.Find("BulletCounter").GetComponent<Text>();

        // score / combo
        m_ScoreMan = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        m_ScoreTxt = canvas.transform.Find("ScoreCombo").transform.Find("Score").GetComponent<Text>();
        m_ScoreTxt.text = " ";

        m_ComboMeterImg = canvas.transform.Find("ScoreCombo").transform.Find("ComboMeter").GetComponent<Image>();
        m_ComboMeterImg.fillAmount = m_ScoreMan.GetChainTimeLeft;
        m_PrevComboMeter = 0.0f;

        m_ChainTxt = canvas.transform.Find("ScoreCombo").transform.Find("CurrentChain").GetComponent<Text>();
        m_ChainTxt.text = "0";

        m_Multiplier = canvas.transform.Find("ScoreCombo").transform.Find("ComboMultiplier").GetComponent<Text>();
        m_Multiplier.text = "0x";

        {
            // things that probably goes to endscreen
            m_SpareChainTimeTxt = canvas.transform.Find("ScoreCombo").transform.Find("EndScreen").transform.Find("SpareChainTimeNum").GetComponent<Text>();
            m_SpareChainTimeTxt.text = " ";
            m_TotalChainsTxt = canvas.transform.Find("ScoreCombo").transform.Find("EndScreen").transform.Find("TotalChainsNum").GetComponent<Text>();
            m_TotalChainsTxt.text = " ";
            m_LongestChain = canvas.transform.Find("ScoreCombo").transform.Find("EndScreen").transform.Find("LongestChainNum").GetComponent<Text>();
            m_LongestChain.text = " ";
        }


        // waves
        m_WaveMeterImg = canvas.transform.Find("Waves").transform.Find("WaveSliderImage").GetComponent<Image>();
        m_WaveMeterImg.fillAmount = 1.0f;

        // randoms
        m_ScoreComboDecimalPoints = 2;
        m_MagEmptyBlinkTime = 0.15f;
        m_FlasherThingTimer = m_MagEmptyBlinkTime;
        m_bFlasherThing = true;
    }


    private float TruncateFloat(float value, int nDecimalPoints)
    {
        return (float)(Math.Round(value, nDecimalPoints, MidpointRounding.ToEven));
    }


    private float GetZeroToOneRange(float valueToMod, float baseValue)
    {
        return valueToMod / baseValue;
    }


    private bool FlasherThing(float flashInterval)
    {
        if (m_FlasherThingTimer <= flashInterval)
        {
            m_FlasherThingTimer -= Time.deltaTime;
            if (m_FlasherThingTimer < 0.0f)
            {
                m_FlasherThingTimer = flashInterval;
                m_bFlasherThing = !m_bFlasherThing;
            }
        }

        return m_bFlasherThing;
    }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        DontDestroyOnLoad(gameObject);

        Init();
    }


    private void Update()
    {
        // health
        float nextHealth = Mathf.Lerp(m_PrevHealth, GetZeroToOneRange(m_PlayerMan.GetCurrentHealth, m_PlayerMan.GetBaseHealth), 0.2f);
        m_HealthBarImg.fillAmount = nextHealth;
        m_PrevHealth = nextHealth;

        // guns / bulllets
        bool isReloading = m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetIsReloading;
        if (isReloading == false)
        {
            m_GunBulletText.text = m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentMagSize.ToString();
        }

        if(isReloading == true)
        {
            m_GunBulletText.text = (FlasherThing(m_MagEmptyBlinkTime) == false) ? "OUT" : " ";
        }

        // score / combo
        int score = (int)m_ScoreMan.GetPlayerScore;
        //if(score > 0)
        {
            m_ScoreTxt.text = score.ToString();
        }

        float translatedToRange = GetZeroToOneRange(m_ScoreMan.GetChainTimeLeft, m_ScoreMan.GetBaseChainTime);
        if(translatedToRange < 0.98f && translatedToRange > 0.97f)
        {
            m_ComboMeterImg.enabled = false;
            m_ComboMeterImg.fillAmount = 1.0f;
            m_ComboMeterImg.enabled = true;
        }
        if (translatedToRange < 1.0f && translatedToRange >= 0.0f)
        {
            float nextComboMeter = Mathf.Lerp(m_PrevComboMeter, translatedToRange, 0.2f);
            m_ComboMeterImg.fillAmount = nextComboMeter;
            m_PrevComboMeter = nextComboMeter;
        }

        m_ChainTxt.text = m_ScoreMan.GetCurrentChain.ToString();

        string scaleSymbol = "x ";
        float multi = TruncateFloat(m_ScoreMan.GetCurrentComboMultiplier, m_ScoreComboDecimalPoints);
        m_Multiplier.text = string.Concat(scaleSymbol, multi.ToString());

        {
            // things that probably goes to endscreen
            float timeLeft = TruncateFloat(m_ScoreMan.GetChainTimeLeft, m_ScoreComboDecimalPoints);
            m_SpareChainTimeTxt.text = timeLeft.ToString();
            float totalChains = TruncateFloat(m_ScoreMan.GetTotalChains, m_ScoreComboDecimalPoints);
            m_TotalChainsTxt.text = totalChains.ToString();
            float longestChain = TruncateFloat(m_ScoreMan.GetLongestChain, m_ScoreComboDecimalPoints);
            m_LongestChain.text = longestChain.ToString();
        }

        // waves
        {


        }
    }
}
