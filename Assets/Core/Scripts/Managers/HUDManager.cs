using System;
using UnityEngine;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    public static HUDManager GetInstance { get; private set; }

    private PlayerManager m_PlayerMan;
    private GunManager m_GunMan;
    private ScoreManager m_ScoreMan;
    private int m_ScoreComboDecimalPoints;

    // player status
    private Text[] m_arrPlayerStatusText;
    private Image m_HealthBar;
    private float m_PrevHealth;
    private Image m_ComboMeter;
    private float m_PrevComboMeter;
    private Text m_CurrentChain;
    private Text m_CurrentComboMultiplier;

    // guns / bulllets
    private Text[] m_arrGunsBulletText;

    // score / combo
    private Text[] m_arrScoreComboText;

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
        m_arrPlayerStatusText = new Text[(int)EPlayerText.SIZE];
        m_arrPlayerStatusText[(int)EPlayerText.HEALTH] = canvas.transform.Find("PlayerStatus").transform.Find("HealthCounter").GetComponent<Text>();

        {
            // new
            m_HealthBar = canvas.transform.Find("PlayerStatus").transform.Find("HealthSliderImage").GetComponent<Image>();
            m_PrevHealth = m_PlayerMan.GetCurrentHealth;
        }

        // guns / bulllets
        m_GunMan = GameObject.Find("GunManager").GetComponent<GunManager>();
        m_arrGunsBulletText = new Text[1];
        m_arrGunsBulletText[0] = canvas.transform.Find("GunBullet").transform.Find("BulletCounter").GetComponent<Text>();

        // score / combo
        m_ScoreMan = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        Transform score = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual");
        int size = (int)ScoreManager.EScoreText.SIZE;
        m_arrScoreComboText = new Text[size];
        for (int i = 0; i < size; ++i)
        {
            m_arrScoreComboText[i] = score.GetChild(i).GetComponent<Text>();
        }

        {
            // new
            m_ComboMeter = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("ComboMeter").GetComponent<Image>();
            m_ComboMeter.fillAmount = m_ScoreMan.GetChainTimeLeft;
            m_PrevComboMeter = 0.0f;

            m_CurrentChain = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("CurrentChain").GetComponent<Text>();
            m_CurrentChain.text = "0";

            m_CurrentComboMultiplier = canvas.transform.Find("ScoreCombo").transform.Find("ScoreActual").transform.Find("CurrentComboMultiplier").GetComponent<Text>();
            m_CurrentComboMultiplier.text = "0x";
        }

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
        // player status
        m_arrPlayerStatusText[(int)EPlayerText.HEALTH].text = m_PlayerMan.GetCurrentHealth.ToString();

        {
            // new
            float nextHealth = Mathf.Lerp(m_PrevHealth, GetZeroToOneRange(m_PlayerMan.GetCurrentHealth, m_PlayerMan.GetBaseHealth), 0.2f);
            m_HealthBar.fillAmount = nextHealth;
            m_PrevHealth = nextHealth;
        }

        // guns / bulllets
        int currentMag = m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentMagSize;
        if(currentMag > 0)
        {
            m_arrGunsBulletText[0].text = m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentMagSize.ToString();
        }
        else
        {
            float reloadTime = TruncateFloat(m_GunMan.ActiveGun.GetComponent<GunTemplate>().GetCurrentReloadTime, m_ScoreComboDecimalPoints);
            m_arrGunsBulletText[0].text = reloadTime.ToString();
        }

        // score / combo
        int score = (int)m_ScoreMan.GetPlayerScore;
        m_arrScoreComboText[(int)ScoreManager.EScoreText.SCORE].text = score.ToString();
        m_arrScoreComboText[(int)ScoreManager.EScoreText.TOTAL_CHAINS].text = m_ScoreMan.GetTotalChains.ToString();
        m_arrScoreComboText[(int)ScoreManager.EScoreText.LONGEST_CHAIN].text = m_ScoreMan.GetLongestChain.ToString();

        {
            // new
            float nextComboMeter = Mathf.Lerp(m_PrevComboMeter, GetZeroToOneRange(m_ScoreMan.GetChainTimeLeft, m_ScoreMan.GetBaseChainTime), 0.2f);
            m_ComboMeter.fillAmount = nextComboMeter;
            m_PrevComboMeter = nextComboMeter;

            m_CurrentChain.text = m_ScoreMan.GetCurrentChain.ToString();

            string scaleSymbol = "x";
            float multi = TruncateFloat(m_ScoreMan.GetCurrentComboMultiplier, m_ScoreComboDecimalPoints);
            m_CurrentComboMultiplier.text = string.Concat(scaleSymbol, multi.ToString());
        }

        // truncated
        float chainTimeLeft = TruncateFloat(m_ScoreMan.GetChainTimeLeft, m_ScoreComboDecimalPoints);
        m_arrScoreComboText[(int)ScoreManager.EScoreText.CHAIN_TIME_LEFT].text = chainTimeLeft.ToString();

        // truncated
        float spareChainTime = TruncateFloat(m_ScoreMan.GetSpareChainTime, m_ScoreComboDecimalPoints);
        m_arrScoreComboText[(int)ScoreManager.EScoreText.SPARE_CHAIN_TIME].text = spareChainTime.ToString();

        m_arrScoreComboText[(int)ScoreManager.EScoreText.CURRENT_CHAIN].text = m_ScoreMan.GetCurrentChain.ToString();

        // truncated
        float comboMulti = TruncateFloat(m_ScoreMan.GetCurrentComboMultiplier, m_ScoreComboDecimalPoints);
        m_arrScoreComboText[(int)ScoreManager.EScoreText.CURRENT_MULTI].text = comboMulti.ToString();
    }
}
