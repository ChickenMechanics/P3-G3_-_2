using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneFadeInOut : MonoBehaviour
{
    private Animator m_Animator;
    private int m_SceneToLoadIdx;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_SceneToLoadIdx = 0;
    }


    public void FadeScene(int sceneIdx)
    {
        m_SceneToLoadIdx = sceneIdx;
        m_Animator.SetTrigger("FadeOut");
    }


    public void OnFadeComplete()
    {
        SceneManager.LoadScene(m_SceneToLoadIdx);
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FadeScene(3);
        }
    }

}
