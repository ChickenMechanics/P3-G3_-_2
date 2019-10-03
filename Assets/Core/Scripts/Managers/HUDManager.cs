using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class HUDManager : MonoBehaviour
{
    public static HUDManager GetInstance { get; private set; }

    private PlayerManager m_PlayerMan;
    private GunManager m_GunMan;
    private ScoreManager m_ScoreMan;
    private int m_ScoreComboDecimalPoints;

    // player status
    private Text[] m_arrPlayerStatusText;

    // guns / bulllets
    private Text[] m_arrGunsBulletText;

    // score / combo
    private Text[] m_arrScoreComboText;


    //----------------------------------------------------------------------------------------------------


    private void Init()
    {
        Transform canvas = transform.Find("HUDCanvas");

        // player status
        m_PlayerMan = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        m_arrPlayerStatusText = new Text[(int)PlayerManager.EPlayerText.SIZE];
        m_arrPlayerStatusText[(int)PlayerManager.EPlayerText.HEALTH] = canvas.transform.Find("PlayerStatus").transform.Find("HealthCounter").GetComponent<Text>();

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

        // decimals
        m_ScoreComboDecimalPoints = 2;
    }


    private float TruncateFloat(float value, int nDecimalPoints)
    {
        return (float)(Math.Round(value, nDecimalPoints, MidpointRounding.ToEven));
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
        m_arrPlayerStatusText[(int)PlayerManager.EPlayerText.HEALTH].text = m_PlayerMan.GetHealth.ToString();

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
        int score = (int)m_ScoreMan.PlayerScore;
        m_arrScoreComboText[(int)ScoreManager.EScoreText.SCORE].text = score.ToString();
        m_arrScoreComboText[(int)ScoreManager.EScoreText.TOTAL_CHAINS].text = m_ScoreMan.TotalChains.ToString();
        m_arrScoreComboText[(int)ScoreManager.EScoreText.LONGEST_CHAIN].text = m_ScoreMan.LongestChain.ToString();

        // truncated
        float chainTimeLeft = TruncateFloat(m_ScoreMan.ChainTimeLeft, m_ScoreComboDecimalPoints);
        m_arrScoreComboText[(int)ScoreManager.EScoreText.CHAIN_TIME_LEFT].text = chainTimeLeft.ToString();

        // truncated
        float spareChainTime = TruncateFloat(m_ScoreMan.SpareChainTime, m_ScoreComboDecimalPoints);
        m_arrScoreComboText[(int)ScoreManager.EScoreText.SPARE_CHAIN_TIME].text = spareChainTime.ToString();

        m_arrScoreComboText[(int)ScoreManager.EScoreText.CURRENT_CHAIN].text = m_ScoreMan.CurrentChain.ToString();

        // truncated
        float comboMulti = TruncateFloat(m_ScoreMan.CurrentComboMultiplier, m_ScoreComboDecimalPoints);
        m_arrScoreComboText[(int)ScoreManager.EScoreText.CURRENT_MULTI].text = comboMulti.ToString();
    }
}
