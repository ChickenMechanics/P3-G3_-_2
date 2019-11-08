using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class HUDManager : MonoBehaviour
{
    #region design vars
    [Header("Score FX")]
    [Range(0.0f, 10.0f)]
    public float m_ScoreRumbleValue;
    [Range(0.0f, 0.5f)]
    public float m_ScoreRumbleTime;
    [Range(1.0f, 5.0f)]
    public float m_ScoreBounceTargetScale;
    [Range(0.0f, 10.0f)]
    public float m_ScoreBounceScaleMultiUp;
    [Range(0.0f, 10.0f)]
    public float m_ScoreBounceScaleMultiDown;
    public Color m_ScoreColorChange;

    [Header("Score Chain FX")]
    [Range(0.0f, 10.0f)]
    public float m_ChainRumbleValue;
    [Range(0.0f, 0.5f)]
    public float m_ChainRumbleTime;
    [Range(1.0f, 5.0f)]
    public float m_ChainBounceTargetScale;
    [Range(0.0f, 10.0f)]
    public float m_ChainBounceScaleMultiUp;
    [Range(0.0f, 10.0f)]
    public float m_ChainBounceScaleMultiDown;

    [Header("Combo Meter FX")]
    public Color m_Full;
    public Color m_Semi;
    public Color m_Empty;

    [Header("Crack economy")]
    public float m_Crack_1;
    public float m_Crack_2;
    public float m_Crack_3;
    public float m_Crack_4;

    [Header("Helmet Lights")]
    public float m_HelmetLightOnTime;
    public float m_HelmetLightOffTime;
    public float m_HelmetLightLerpTime;
    #endregion

    public static HUDManager GetInstance { get; private set; }

    #region scr ref
    private PlayerManager m_PlayerManScr;
    private GunManager m_GunManScr;
    private ScoreManager m_ScoreManScr;
    #endregion

    private Image m_HealthLeftImg;
    private Image m_HealthRightImg;
    private Image m_DashSliderImg;
    private Image m_ComboMeterImg;
    private Image m_WaveMeterImg;
    private Image m_LeftLightImg;
    private Image m_RightLightImg;
    private Image[] m_PlayerCracks;
    private Text m_ScoreTxt;
    private Text m_ChainTxt;
    private Text m_Multiplier;
    private Text m_GunBulletText;
    private float m_PrevHealth;
    private int m_ScoreComboDecimalPoints;

    private float m_MagEmptyBlinkTime;
    private float m_FlasherThingTimer;
    private bool m_bFlasherThing;

    private float m_NowHelmetFlashLightTime;
    private bool m_bHelmetFlashOnOff;

    private int m_PrevFrameScore;

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

    // FX objets
    struct TxtRumbleFX
    {
        public Text TxtRef;
        public Vector3 RumbleInitPos;
        public float RumbleRange;
        public float RumbleTimeTotal;
        public float RumbleInitTime;
        public bool RumbleDirFlipper;
    }

    struct TxtBounceFX
    {
        public Text TxtRef;
        public Vector3 BounceInitScale;
        public Vector3 BounceInitPos;
        public float TargetScale;
        public float ScaleMultiUp;
        public float ScaleMultiDown;
        public bool BounceDirFlipper;
    }

    private TxtRumbleFX m_ScoreRumbleFX;
    private TxtBounceFX m_ScoreBounceFX;

    private TxtRumbleFX m_ChainRumbleFX;
    private TxtBounceFX m_ChainBounceFX;

    // Color FX
    private GradientColorKey[] m_ColorKey;
    private GradientAlphaKey[] m_AlphaKey;
    private Gradient m_RingGradient;

    // cracks
    private bool[] m_IsCrack;

    // dash
    private float m_DashCoolDownTimeTarget;
    private float m_NowDashCoolDownTime;

    // helmet lights
    private Color m_ScoreColorBase;
    private Color m_LeftRightHelmetLightBaseColor;




    // highscore
    // old / kill when new is in
    //[System.Serializable]
    //private struct HighScoreData
    //{
    //    public string m_Name;
    //    public int m_Score;
    //}

    //private struct HighScoreJsonData
    //{
    //    public List<HighScoreData> m_HighScoreEntryData;
    //}

    //private List<HighScoreData> m_HighScoreEntryList;
    //private List<Transform> m_HighScoreTransformList;
    // end old


    // highscore
    private enum EHighScoreState
    {
        ENTRY,
        TABLE
    }
    private EHighScoreState m_HighScoreState;

    [System.Serializable, HideInInspector]
    public class HighScoreData
    {
        public string m_Name;
        public int m_Score;
    }

    private List<Transform> m_HighScoreTransformList;
    private List<HighScoreData> m_HighScoreDataList;
    private Transform m_HighScoreTableRoot;
    private Transform m_HighScoreBG;
    private Transform m_HighScoreHeaderHeader;
    private Transform m_HighScoreHeaderName;
    private Transform m_HighScoreHeaderScore;
    private Transform m_HighScoreNewPlayerEntry;
    private Transform m_HighScoreEntryTemplate;
    private string m_UserInputName;
    private int m_HighScoreMaxEntries;
    private bool m_bDisplayHighScore;


    //----------------------------------------------------------------------------------------------------


    private void Init()
    {
        Transform canvas = transform.Find("HUDCanvas");

        // player status
        m_PlayerManScr = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        m_HealthLeftImg = canvas.transform.Find("PlayerStatus").transform.Find("HealthLeftImg").GetComponent<Image>();
        m_HealthRightImg = canvas.transform.Find("PlayerStatus").transform.Find("HealthRightImg").GetComponent<Image>();
        m_DashSliderImg = canvas.transform.Find("PlayerStatus").transform.Find("WaveSliderImg").GetComponent<Image>();

        m_PrevHealth = m_PlayerManScr.GetCurrentHealth;

        // guns / bulllets
        m_GunManScr = GameObject.Find("GunManager").GetComponent<GunManager>();
        m_GunBulletText = canvas.transform.Find("GunBullet").transform.Find("BulletCounterTxt").GetComponent<Text>();

        // score / combo
        m_ScoreManScr = ScoreManager.GetInstance.GetComponent<ScoreManager>();

        m_ScoreTxt = canvas.transform.Find("ScoreCombo").transform.Find("ScoreTxt").GetComponent<Text>();
        m_ScoreTxt.text = " ";
        m_ScoreColorBase = m_ScoreTxt.color;

        m_ComboMeterImg = canvas.transform.Find("ScoreCombo").transform.Find("ComboMeterImg").GetComponent<Image>();
        m_ComboMeterImg.fillAmount = m_ScoreManScr.GetChainTimeLeft;

        m_ChainTxt = canvas.transform.Find("ScoreCombo").transform.Find("CurrentChainTxt").GetComponent<Text>();
        m_ChainTxt.text = "0";

        m_Multiplier = canvas.transform.Find("ScoreCombo").transform.Find("ComboMultiplierTxt").GetComponent<Text>();
        m_Multiplier.text = "0x";

        {
            // endscreen / probably
            m_SpareChainTimeTxt = canvas.transform.Find("ScoreCombo").transform.Find("EndScreen").transform.Find("SpareChainTimeNum").GetComponent<Text>();
            m_SpareChainTimeTxt.text = " ";
            m_TotalChainsTxt = canvas.transform.Find("ScoreCombo").transform.Find("EndScreen").transform.Find("TotalChainsNum").GetComponent<Text>();
            m_TotalChainsTxt.text = " ";
            m_LongestChain = canvas.transform.Find("ScoreCombo").transform.Find("EndScreen").transform.Find("LongestChainNum").GetComponent<Text>();
            m_LongestChain.text = " ";
        }

        // waves
        //m_WaveMeterImg = canvas.transform.Find("Waves").transform.Find("WaveSliderImage").GetComponent<Image>();
        //m_WaveMeterImg.fillAmount = 1.0f;

        // randoms
        m_ScoreComboDecimalPoints = 2;
        m_MagEmptyBlinkTime = 0.15f;
        m_FlasherThingTimer = m_MagEmptyBlinkTime;
        m_bFlasherThing = true;

        m_PrevFrameScore = 0;

        // FX obj
        m_ScoreRumbleFX = new TxtRumbleFX
        {
            TxtRef = m_ScoreTxt,
            RumbleInitPos = m_ScoreTxt.transform.position,
            RumbleRange = m_ScoreRumbleValue,
            RumbleTimeTotal = m_ScoreRumbleTime,
            RumbleInitTime = 0.0f,
            RumbleDirFlipper = false
        };

        m_ScoreBounceFX = new TxtBounceFX
        {
            TxtRef = m_ScoreTxt,
            BounceInitScale = m_ScoreTxt.transform.localScale,
            BounceInitPos = m_ScoreTxt.transform.localPosition,
            TargetScale = m_ScoreBounceTargetScale,
            ScaleMultiUp = m_ScoreBounceScaleMultiUp,
            ScaleMultiDown = m_ScoreBounceScaleMultiDown,
            BounceDirFlipper = false
        };

        m_ChainRumbleFX = new TxtRumbleFX
        {
            TxtRef = m_ChainTxt,
            RumbleInitPos = m_ChainTxt.transform.position,
            RumbleRange = m_ChainRumbleValue,
            RumbleTimeTotal = m_ChainRumbleTime,
            RumbleInitTime = 0.0f,
            RumbleDirFlipper = false
        };

        m_ChainBounceFX = new TxtBounceFX
        {
            TxtRef = m_ChainTxt,
            BounceInitScale = m_ChainTxt.transform.localScale,
            BounceInitPos = m_ChainTxt.transform.localPosition,
            TargetScale = m_ChainBounceTargetScale,
            ScaleMultiUp = m_ChainBounceScaleMultiUp,
            ScaleMultiDown = m_ChainBounceScaleMultiDown,
            BounceDirFlipper = false
        };

        // Color FX
        m_ComboMeterImg.color = m_Empty;

        m_ColorKey = new GradientColorKey[3];
        m_AlphaKey = new GradientAlphaKey[3];

        m_RingGradient = new Gradient();

        m_ColorKey[0].color = m_Full;
        m_ColorKey[0].time = 1.0f;
        m_ColorKey[1].color = m_Semi;
        m_ColorKey[1].time = 0.5f;
        m_ColorKey[2].color = m_Empty;
        m_ColorKey[2].time = 0.0f;

        m_AlphaKey[0].alpha = 1.0f;
        m_AlphaKey[0].time = 1.0f;
        m_AlphaKey[1].alpha = 1.0f;
        m_AlphaKey[1].time = 0.5f;
        m_AlphaKey[2].alpha = 1.0f;
        m_AlphaKey[2].time = 0.0f;

        // cracks
        m_PlayerCracks = new Image[4];
        m_PlayerCracks[0] = canvas.transform.Find("PlayerCracks").transform.Find("Crack_1").GetComponent<Image>();
        m_PlayerCracks[1] = canvas.transform.Find("PlayerCracks").transform.Find("Crack_2").GetComponent<Image>();
        m_PlayerCracks[2] = canvas.transform.Find("PlayerCracks").transform.Find("Crack_3").GetComponent<Image>();
        m_PlayerCracks[3] = canvas.transform.Find("PlayerCracks").transform.Find("Crack_4").GetComponent<Image>();

        // dash
        m_DashCoolDownTimeTarget = PlayerManager.GetInstance.GetPlayerMoveScr.m_DashCooldown;
        m_NowDashCoolDownTime = m_DashCoolDownTimeTarget;

        // helmet lights
        m_LeftLightImg = canvas.transform.Find("Statics").transform.Find("LeftLightImg").GetComponent<Image>();
        m_RightLightImg = canvas.transform.Find("Statics").transform.Find("RightLightImg").GetComponent<Image>();
        m_LeftRightHelmetLightBaseColor = m_RightLightImg.color;

        m_NowHelmetFlashLightTime = m_HelmetLightOnTime;
        m_bHelmetFlashOnOff = true;

        // helmet cracks
        m_IsCrack = new bool[4];
        for(int i = 0; i < 4; ++i)
        {
            m_IsCrack[i] = false;
        }

        // highscore
        m_HighScoreState = EHighScoreState.ENTRY;
        m_bDisplayHighScore = false;
        m_UserInputName = "";

        m_HighScoreTableRoot = canvas.transform.Find("HighScoreTable").transform;
        m_HighScoreBG = m_HighScoreTableRoot.Find("BG").transform;
        m_HighScoreHeaderHeader = m_HighScoreTableRoot.Find("Header").transform;
        m_HighScoreHeaderName = m_HighScoreTableRoot.Find("Name").transform;
        m_HighScoreHeaderScore = m_HighScoreTableRoot.Find("Score").transform;
        m_HighScoreNewPlayerEntry = m_HighScoreTableRoot.Find("NewPlayerEntry").transform;
        m_HighScoreEntryTemplate = m_HighScoreTableRoot.Find("NameEntryTemplate").transform;

        m_HighScoreBG.gameObject.SetActive(false);
        m_HighScoreTableRoot.gameObject.SetActive(false);
        m_HighScoreNewPlayerEntry.gameObject.SetActive(false);
        m_HighScoreEntryTemplate.gameObject.SetActive(false);

        m_HighScoreDataList = new List<HighScoreData>();
        LoadHighScore();

        m_HighScoreMaxEntries = 5;
        if (m_HighScoreDataList.Count != m_HighScoreMaxEntries)
        {
            m_HighScoreDataList.Clear();
            HighScoreData[] initScores = new HighScoreData[m_HighScoreMaxEntries];
            initScores[0] = new HighScoreData { m_Name = "AAA", m_Score = 555 };
            initScores[1] = new HighScoreData { m_Name = "BBB", m_Score = 444 };
            initScores[2] = new HighScoreData { m_Name = "CCC", m_Score = 333 };
            initScores[3] = new HighScoreData { m_Name = "DDD", m_Score = 222 };
            initScores[4] = new HighScoreData { m_Name = "EEE", m_Score = 111 };
            for (int i = 0; i < m_HighScoreMaxEntries; ++i)
            {
                SaveHighScore(initScores[i]);
            }
            SortHighScoreList();
        }

#if DEBUG
        for (int i = 0; i < m_HighScoreDataList.Count; ++i)
        {
            Debug.Log(m_HighScoreDataList[i].m_Name + " " + m_HighScoreDataList[i].m_Score);
        }
#endif

        m_HighScoreTransformList = new List<Transform>();
        InitHighScoreEntryTable();
    }


    public void SaveHighScore(HighScoreData scoreData)
    {
        m_HighScoreDataList.Add(scoreData);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SavedHighScores.grr"); //you can call it anything you want
        bf.Serialize(file, m_HighScoreDataList);
        file.Close();

        //Debug.Log(Application.persistentDataPath);
    }


    public void LoadHighScore()
    {
        if (File.Exists(Application.persistentDataPath + "/SavedHighScores.grr"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SavedHighScores.grr", FileMode.Open);
            m_HighScoreDataList = (List<HighScoreData>)bf.Deserialize(file);
            file.Close();
        }
    }


    public void HighScoreEnable()
    {
        Time.timeScale = (Time.timeScale == 0.0f) ? 1.0f : 0.0f;
        ScoreManager.GetInstance.m_GetBulletTimeEnabled = false;
        m_bDisplayHighScore = true;

        m_HighScoreBG.gameObject.SetActive(true);
        m_HighScoreTableRoot.gameObject.SetActive(true);
    }


    private void DisplayHighscore()
    {
        if(m_bDisplayHighScore == true)
        {
            switch (m_HighScoreState)
            {
                case EHighScoreState.ENTRY:

                    m_HighScoreNewPlayerEntry.gameObject.SetActive(true);
                    //GUI.TextField();

                    break;
                case EHighScoreState.TABLE:

                    m_HighScoreHeaderHeader.gameObject.SetActive(true);
                    m_HighScoreHeaderName.gameObject.SetActive(true);
                    m_HighScoreHeaderScore.gameObject.SetActive(true);

                    for (int i = 0; i < m_HighScoreTransformList.Count; ++i)
                    {
                        m_HighScoreTransformList[i].gameObject.SetActive(true);
                    }

                    break;
            }
        }
    }


    private void SortHighScoreList()
    {
        for (int i = 0; i < m_HighScoreDataList.Count; ++i)
        {
            for (int j = (i + 1); j < m_HighScoreDataList.Count; ++j)
            {
                if (m_HighScoreDataList[j].m_Score > m_HighScoreDataList[i].m_Score)
                {
                    HighScoreData tmp = m_HighScoreDataList[i];
                    m_HighScoreDataList[i] = m_HighScoreDataList[j];
                    m_HighScoreDataList[j] = tmp;
                }
            }
        }
    }


    private void InitHighScoreEntryTable()
    {
        float entryOffset = 60.0f;
        for (int i = 0; i < m_HighScoreDataList.Count; ++i)
        {
            Transform entry = Instantiate(m_HighScoreEntryTemplate, m_HighScoreTableRoot);
            RectTransform entryRect = entry.GetComponent<RectTransform>();
            entryRect.anchoredPosition = new Vector2(0.0f, -entryOffset * i);
            int place = i + 1;
            string rankNum = place.ToString();
            rankNum += ".";
            Transform rank = entry.Find("RankPos").transform;
            Text rankTxt = rank.GetComponent<Text>();
            rankTxt.text = rankNum;

            // if we wan't st, nd, rd, th
            //int place = i;
            //++place;
            //string rankNum = place.ToString();
            //string rankNumSuffix = "";
            //switch (i)
            //{
            //    case 0:     rankNumSuffix = "st";  break;
            //    case 1:     rankNumSuffix = "nd";  break;
            //    case 2:     rankNumSuffix = "rd";  break;
            //    default:    rankNumSuffix = "th";  break;
            //}
            //rankNum += rankNumSuffix;
            //Transform rank = entry.Find("RankPos").transform;
            //Text rankTxt = rank.GetComponent<Text>();
            //rankTxt.text = rankNum;

            entryRect.gameObject.SetActive(true);

            Text nameTxt = entry.transform.Find("NamePos").transform.GetComponent<Text>();
            nameTxt.text = m_HighScoreDataList[i].m_Name;

            Text scoreTxt = entry.transform.Find("ScorePos").transform.GetComponent<Text>();
            scoreTxt.text = m_HighScoreDataList[i].m_Score.ToString();

            m_HighScoreTransformList.Add(entry);
            m_HighScoreTransformList[m_HighScoreTransformList.Count - 1].gameObject.SetActive(false);
        }
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


    private IEnumerator TxtRumblerFX(TxtRumbleFX rumbleFxObj)
    {
        while((rumbleFxObj.RumbleInitTime + rumbleFxObj.RumbleTimeTotal) > Time.time)
        {
            if (rumbleFxObj.RumbleDirFlipper == true)
            {
                rumbleFxObj.TxtRef.transform.position = rumbleFxObj.RumbleInitPos;
                rumbleFxObj.RumbleDirFlipper = false;
            }
            else
            {
                rumbleFxObj.TxtRef.transform.position += new Vector3(
                    UnityEngine.Random.Range(-rumbleFxObj.RumbleRange, rumbleFxObj.RumbleRange),
                    UnityEngine.Random.Range(-rumbleFxObj.RumbleRange, rumbleFxObj.RumbleRange),
                    0.0f);

                rumbleFxObj.RumbleDirFlipper = true;
            }

            yield return null;
        }

        rumbleFxObj.TxtRef.transform.position = rumbleFxObj.RumbleInitPos;
        rumbleFxObj.RumbleDirFlipper = false;
        StopCoroutine("Rumbler");
    }


    private IEnumerator TxtBouncerFX(TxtBounceFX bounceFxObj)
    {
        m_ScoreTxt.color = m_ScoreColorChange;

        while (bounceFxObj.TxtRef.transform.localScale.x >= bounceFxObj.BounceInitScale.x)
        {
            if (bounceFxObj.BounceDirFlipper == false)
            {
                bounceFxObj.TxtRef.transform.localScale += new Vector3(bounceFxObj.ScaleMultiUp, bounceFxObj.ScaleMultiUp, 0.0f) * Time.deltaTime;
                if (bounceFxObj.TxtRef.transform.localScale.x > bounceFxObj.TargetScale)
                {
                    bounceFxObj.TxtRef.transform.localScale = new Vector3(bounceFxObj.TargetScale, bounceFxObj.TargetScale, 1.0f);
                    bounceFxObj.BounceDirFlipper = true;
                }
            }
            else
            {
                bounceFxObj.TxtRef.transform.localScale -= new Vector3(bounceFxObj.ScaleMultiDown, bounceFxObj.ScaleMultiDown, 0.0f) * Time.deltaTime;
            }

            yield return null;
        }

        m_ScoreTxt.color = m_ScoreColorBase;
        bounceFxObj.TxtRef.transform.localScale = bounceFxObj.BounceInitScale;
        bounceFxObj.BounceDirFlipper = false;
        StopCoroutine("TxtBouncerFX");
    }


    private void LeftRightLightBlink()
    {
        m_NowHelmetFlashLightTime -= Time.deltaTime;
        if(m_bHelmetFlashOnOff == true &&
            m_NowHelmetFlashLightTime < 0.0f)
        {
            m_bHelmetFlashOnOff = false;
            m_NowHelmetFlashLightTime = m_HelmetLightOffTime;
        }
        else if (m_bHelmetFlashOnOff == false &&
            m_NowHelmetFlashLightTime < 0.0f)
        {
            m_bHelmetFlashOnOff = true;
            m_NowHelmetFlashLightTime = m_HelmetLightOnTime;
        }


        if (m_bHelmetFlashOnOff == true)
        {
            m_LeftLightImg.color = m_LeftRightHelmetLightBaseColor;
            m_RightLightImg.color = m_LeftRightHelmetLightBaseColor;
        }
        else if (m_bHelmetFlashOnOff == false)
        {
            m_LeftLightImg.color = Color.Lerp(m_LeftLightImg.color, Color.black, m_HelmetLightLerpTime);
            m_RightLightImg.color = Color.Lerp(m_RightLightImg.color, Color.black, m_HelmetLightLerpTime);
        }
    }


    private void PlayerUpdate()
    {
        HealthUpdate();
        DashUpdate();
        CrackUpdate();
    }


    private void HealthUpdate()
    {
        m_PrevHealth = Mathf.Lerp(m_PrevHealth, GetZeroToOneRange(m_PlayerManScr.GetCurrentHealth, m_PlayerManScr.GetBaseHealth), 0.2f);
        m_HealthLeftImg.fillAmount = m_PrevHealth;
        m_HealthRightImg.fillAmount = m_PrevHealth;
    }


    private void DashUpdate()
    {
        float dashInput = PlayerManager.GetInstance.GetPlayerCtrlScr.GetBasicInput.DashInput;
        int playerStateIdx = PlayerManager.GetInstance.GetPlayerCtrlScr.GetFSM.GetCurrentStateIdx;
        if (dashInput != 0 &&
            playerStateIdx == (int)PlayerCtrl.EPlayerState.DASH)
        {
            m_DashSliderImg.fillAmount = Mathf.Lerp(m_DashSliderImg.fillAmount, 0.0f, 0.25f);
            m_NowDashCoolDownTime = 0.0f;
        }
        else
        {
            if (m_NowDashCoolDownTime < m_DashCoolDownTimeTarget)
            {
                m_NowDashCoolDownTime += Time.deltaTime;
                if (m_NowDashCoolDownTime > m_DashCoolDownTimeTarget)
                {
                    m_NowDashCoolDownTime = m_DashCoolDownTimeTarget;
                }
            }

            float convertedT = GetZeroToOneRange(m_NowDashCoolDownTime, m_DashCoolDownTimeTarget);
            m_DashSliderImg.fillAmount = Mathf.Lerp(m_DashSliderImg.fillAmount, convertedT, 0.1f);
        }
    }


    private void CrackUpdate()
    {
        if(m_IsCrack[0] == false)
        {
            if (PlayerManager.GetInstance.GetCurrentHealth <= m_Crack_1)
            {
                m_IsCrack[0] = true;
                m_PlayerCracks[0].gameObject.SetActive(true);
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.HELMET_CRACK, PlayerManager.GetInstance.GetPlayer.transform.position);
            }
        }
        else if (m_IsCrack[1] == false)
        {
            if (PlayerManager.GetInstance.GetCurrentHealth <= m_Crack_2)
            {
                m_IsCrack[1] = true;
                m_PlayerCracks[1].gameObject.SetActive(true);
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.HELMET_CRACK, PlayerManager.GetInstance.GetPlayer.transform.position);
            }
        }
        else if (m_IsCrack[2] == false)
        {
            if (PlayerManager.GetInstance.GetCurrentHealth <= m_Crack_3)
            {
                m_IsCrack[2] = true;
                m_PlayerCracks[2].gameObject.SetActive(true);
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.HELMET_CRACK, PlayerManager.GetInstance.GetPlayer.transform.position);
            }
        }
        else if (m_IsCrack[3] == false)
        {
            if (PlayerManager.GetInstance.GetCurrentHealth <= m_Crack_4)
            {
                m_IsCrack[3] = true;
                m_PlayerCracks[3].gameObject.SetActive(true);
                SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.HELMET_CRACK, PlayerManager.GetInstance.GetPlayer.transform.position);
            }
        }
    }


    private void GunBulletUpdate()
    {
        bool isReloading = m_GunManScr.ActiveGun.GetComponent<GunTemplate>().GetIsReloading;
        if (isReloading == false)
        {
            m_GunBulletText.text = m_GunManScr.ActiveGun.GetComponent<GunTemplate>().GetCurrentMagSize.ToString();
        }
        else
        {
            m_GunBulletText.text = (FlasherThing(m_MagEmptyBlinkTime) == false) ? "OUT" : " ";
        }
    }


    private void ScoreUpdate()
    {
        int nowScore = (int)m_ScoreManScr.GetPlayerScore;   // resetted at the bottom of the file

        // score and chain FX
        if (nowScore != m_PrevFrameScore)
        {
            m_ScoreRumbleFX.RumbleInitTime = Time.time;
            StartCoroutine(TxtRumblerFX(m_ScoreRumbleFX));

            StartCoroutine(TxtBouncerFX(m_ScoreBounceFX));

            m_ChainRumbleFX.RumbleInitTime = Time.time;
            StartCoroutine(TxtRumblerFX(m_ChainRumbleFX));
        }
        m_ScoreTxt.text = nowScore.ToString();
        m_ChainTxt.text = m_ScoreManScr.GetCurrentChain.ToString();

        // combo meter
        float translatedToRange = GetZeroToOneRange(m_ScoreManScr.GetChainTimeLeft, m_ScoreManScr.GetBaseChainTime);
        if (translatedToRange < 0.98f &&
            translatedToRange > 0.97f)
        {
            m_ComboMeterImg.enabled = false;
            m_ComboMeterImg.fillAmount = 1.0f;
            m_ComboMeterImg.enabled = true;
        }

        if (translatedToRange < 1.0f &&
            translatedToRange >= 0.0f)
        {
            m_RingGradient.SetKeys(m_ColorKey, m_AlphaKey);
            m_ComboMeterImg.color = m_RingGradient.Evaluate(translatedToRange);

            m_ComboMeterImg.fillAmount = Mathf.Lerp(m_ComboMeterImg.fillAmount, translatedToRange, 0.25f);
        }

        string scaleSymbol = "x ";
        //float multi = TruncateFloat(m_ScoreManScr.GetCurrentComboMultiplier, m_ScoreComboDecimalPoints);  // saved if artists change their mind about decimals
        int multi = (int)TruncateFloat(m_ScoreManScr.GetCurrentComboMultiplier, m_ScoreComboDecimalPoints);
        if(multi > 9)
        {
            multi = 9;
        }
        m_Multiplier.text = string.Concat(scaleSymbol, multi.ToString());

        // things that probably goes to endscreen
        {
            float timeLeft = TruncateFloat(m_ScoreManScr.GetChainTimeLeft, m_ScoreComboDecimalPoints);
            m_SpareChainTimeTxt.text = timeLeft.ToString();
            float totalChains = TruncateFloat(m_ScoreManScr.GetTotalChains, m_ScoreComboDecimalPoints);
            m_TotalChainsTxt.text = totalChains.ToString();
            float longestChain = TruncateFloat(m_ScoreManScr.GetLongestChain, m_ScoreComboDecimalPoints);
            m_LongestChain.text = longestChain.ToString();
        }

        m_PrevFrameScore = nowScore;
    }


    public void WavesUpdate()
    {

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
        PlayerUpdate();
        GunBulletUpdate();
        ScoreUpdate();
        WavesUpdate();
        LeftRightLightBlink();
        DisplayHighscore();
    }
}
