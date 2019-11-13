using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class MainMenuGUI : MonoBehaviour
{
    [System.Serializable, HideInInspector]
    public class HighScoreData
    {
        public string m_Name;
        public int m_Score;
    }

    private List<Transform> m_HighScoreTransformList;
    private List<HighScoreData> m_HighScoreDataList;
    private Transform m_HighScoreTableRoot;
    private Transform m_HighScoreEntryTemplate;
    private float m_VerticalNameSpacing;

    private Transform m_HighScoreTransform;
    private Transform m_HowToPlayTransform;
    private Transform m_CreditsTransform;


    private void Start()
    {
        m_HighScoreTransform = transform.Find("HighScoreButton").gameObject.transform.Find("HighScoreImg").gameObject.transform;
        m_HighScoreTransform.gameObject.SetActive(false);

        m_HowToPlayTransform = transform.Find("HowToPlayButton").gameObject.transform.Find("HowToPlayImg").gameObject.transform;
        m_HowToPlayTransform.gameObject.SetActive(false);

        m_CreditsTransform = transform.Find("CreditsButton").gameObject.transform.Find("CreditsImg").gameObject.transform;
        m_CreditsTransform.gameObject.SetActive(false);



        //m_HighScoreTableRoot = transform.Find("HighScoreButton").gameObject.transform.Find("HighScoreTable").transform;
        //m_HighScoreTableRoot.gameObject.SetActive(false);
        //m_HighScoreEntryTemplate = m_HighScoreTableRoot.Find("NameEntryTemplate").transform;

        //m_HighScoreDataList = new List<HighScoreData>();
        //m_VerticalNameSpacing = 55.0f;
        //LoadHighScore();
        //CreateHighScoreEntryTable();
    }


    private void ButtonSound()
    {
        SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.MENU_BUTTON_CLICK, transform.position);
    }


    public void NewGame()
    {
        ButtonSound();

        ScoreManager.GetInstance.ResetPlayerStats();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.ARENA);
    }


    public void HighScore()
    {
        //ButtonSound();

        //if (m_HowToPlayTransform.gameObject.activeInHierarchy == true)
        //{
        //    m_HowToPlayTransform.gameObject.SetActive(false);
        //}
        //if (m_CreditsTransform.gameObject.activeInHierarchy == true)
        //{
        //    m_CreditsTransform.gameObject.SetActive(false);
        //}

        //if (m_HighScoreTransform.gameObject.activeInHierarchy == false)
        //{
        //    m_HighScoreTableRoot.gameObject.SetActive(true);
        //    //m_HighScoreTransform.gameObject.SetActive(true);
        //}
        //else
        //{
        //    m_HighScoreTableRoot.gameObject.SetActive(false);
        //    //m_HighScoreTransform.gameObject.SetActive(false);
        //}
    }


    public void HowToPlay()
    {
        ButtonSound();

        if (m_HighScoreTransform.gameObject.activeInHierarchy == true)
        {
            m_HighScoreTransform.gameObject.SetActive(false);
        }
        if (m_CreditsTransform.gameObject.activeInHierarchy == true)
        {
            m_CreditsTransform.gameObject.SetActive(false);
        }

        if (m_HowToPlayTransform.gameObject.activeInHierarchy == false)
        {
            m_HowToPlayTransform.gameObject.SetActive(true);
        }
        else
        {
            m_HowToPlayTransform.gameObject.SetActive(false);
        }
    }


    public void Credits()
    {
        ButtonSound();

        if (m_HighScoreTransform.gameObject.activeInHierarchy == true)
        {
            m_HighScoreTransform.gameObject.SetActive(false);
        }
        if (m_HowToPlayTransform.gameObject.activeInHierarchy == true)
        {
            m_HowToPlayTransform.gameObject.SetActive(false);
        }

        if (m_CreditsTransform.gameObject.activeInHierarchy == false)
        {
            m_CreditsTransform.gameObject.SetActive(true);
        }
        else
        {
            m_CreditsTransform.gameObject.SetActive(false);
        }
    }


    public void QuitGame()
    {
        ButtonSound();

        Application.Quit();
    }


    private void LoadHighScore()
    {
        if (File.Exists(Application.persistentDataPath + "/SavedHighScores.grr"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SavedHighScores.grr", FileMode.Open);
            m_HighScoreDataList.Clear();
            m_HighScoreDataList = (List<HighScoreData>)bf.Deserialize(file);
            file.Close();
        }
    }


    private void CreateHighScoreEntryTable()
    {
        m_HighScoreTransformList = new List<Transform>();
        for (int i = 0; i < m_HighScoreDataList.Count; ++i)
        {
            Transform entry = Instantiate(m_HighScoreEntryTemplate, m_HighScoreTableRoot);
            RectTransform entryRect = entry.GetComponent<RectTransform>();
            entryRect.anchoredPosition = new Vector2(0.0f, -m_VerticalNameSpacing * i);
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
}
