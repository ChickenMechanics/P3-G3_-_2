﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndMenuGUI : MonoBehaviour
{
    public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.MAIN);
    }


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
