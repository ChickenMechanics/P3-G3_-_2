using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownScript : MonoBehaviour
{
    //[SerializeField] private GameObject txtObj;
    private Text text;
    [SerializeField] int mainTimer;

    private float timer;
    private bool canCount = true;
    private bool doOnce = false;

    public Color Color1;
    public Color Color2;
    

    void Awake()
    {
        timer = (float)mainTimer;
        text = GetComponent<Text>();
        text.color = Color1;
    }


    void Update()
    {
        if (timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            int truncated = (int)timer;
            text.text = truncated.ToString();
        }

        else if(timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            text.text = "0";
            timer = 0.0f;
        }

        if(timer < 6)
        {
            text.color = Color2;
        }
    }
}
