using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndMenuGUI : MonoBehaviour
{
    public void MainMenu()
    {
        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.MAIN);
    }
}
