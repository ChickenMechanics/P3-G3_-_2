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

        m_ScoreTxt = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("Score").GetComponent<Text>();
        m_ScoreTxt.text = " ";

        m_ComboMeterImg = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("ComboMeter").GetComponent<Image>();
        m_ComboMeterImg.fillAmount = m_ScoreMan.GetChainTimeLeft;
        m_PrevComboMeter = 0.0f;

        m_ChainTxt = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("CurrentChain").GetComponent<Text>();
        m_ChainTxt.text = "0";

        m_Multiplier = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("ComboMultiplier").GetComponent<Text>();
        m_Multiplier.text = "0x";

        {
            // things that probably goes to endscreen
            m_SpareChainTimeTxt = canvas.transform.Find("ScoreCombo").transform.Find("ScoreTitles").transform.Find("SpareChainTimeNum").GetComponent<Text>();
            m_SpareChainTimeTxt.text = " ";
            m_TotalChainsTxt = canvas.transform.Find("ScoreCombo").transform.Find("ScoreTitles").transform.Find("TotalChainsNum").GetComponent<Text>();
            m_TotalChainsTxt.text = " ";
            m_LongestChain = canvas.transform.Find("ScoreCombo").transform.Find("ScoreTitles").transform.Find("LongestChainNum").GetComponent<Text>();
            m_LongestChain.text = " ";
        }


        // waves
        m_WaveMeterImg = canvas.transform.Find("Waves").transform.Find("WaveSliderImage").GetComponent<Image>();
        m_WaveMeterImg.fillAmount = 1.0f;

        // decimals
        m_ScoreComboDecimalPoints = 2;
    }


    private float TruncateFloat(float value, int nDecimalPoints)
    {
        return (float)(Math.Round(value, nDecimalPoints, MidpointRounding.ToEven));
    }


    private float GetZeroToOneRange(float valueToMod, float baseValue)
    {
        return valueToMod / baseValue;
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
        int currentMag = m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentMagSize;
        if(currentMag > 0)
        {
            m_GunBulletText.text = m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentMagSize.ToString();
        }
        else
        {
            float reloadTime = TruncateFloat(m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentReloadTime, m_ScoreComboDecimalPoints);
            m_GunBulletText.text = reloadTime.ToString();
        }

        // score / combo
        int score = (int)m_ScoreMan.GetPlayerScore;
        if(score > 0)
        {
            m_ScoreTxt.text = score.ToString();
        }

        float nextComboMeter = Mathf.Lerp(m_PrevComboMeter, GetZeroToOneRange(m_ScoreMan.GetChainTimeLeft, m_ScoreMan.GetBaseChainTime), 0.2f);
        m_ComboMeterImg.fillAmount = nextComboMeter;
        m_PrevComboMeter = nextComboMeter;

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
