using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuGUI : MonoBehaviour
{
    public void NewGame()
    {
        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.ARENA);
    }


    public void Options()
    {
        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.OPTIONS);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
