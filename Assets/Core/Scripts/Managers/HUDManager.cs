using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    public static HUDManager GetInstance { get; private set; }

    private ScoreManager m_ScoreMan;

    // Temp
    private Text[] m_arrText;
    private Text m_Score;
    // Temp


    private void Init()
    {
        m_ScoreMan = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        // Temp score text
        int size = (int)ScoreManager.EText.SIZE;
        m_arrText = new Text[size];
        for (int i = 0; i < size; ++i)
        {
            m_arrText[i] = transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<Text>();
        }
        // Temp
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
        int score = (int)m_ScoreMan.PlayerScore;    // Truncuate
        m_arrText[(int)ScoreManager.EText.SCORE].text = score.ToString();
        m_arrText[(int)ScoreManager.EText.TOTAL_CHAINS].text = m_ScoreMan.TotalChains.ToString();
        m_arrText[(int)ScoreManager.EText.LONGEST_CHAIN].text = m_ScoreMan.LongestChain.ToString();

        m_arrText[(int)ScoreManager.EText.CHAIN_TIME_LEFT].text = m_ScoreMan.ChainTimeLeft.ToString();
        m_arrText[(int)ScoreManager.EText.SPARE_CHAIN_TIME].text = m_ScoreMan.SpareChainTime.ToString();
        m_arrText[(int)ScoreManager.EText.CURRENT_CHAIN].text = m_ScoreMan.CurrentChain.ToString();
        m_arrText[(int)ScoreManager.EText.CURRENT_MULTI].text = m_ScoreMan.CurrentComboMultiplier.ToString();
    }
}
