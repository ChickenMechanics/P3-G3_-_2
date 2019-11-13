using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameLoader : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.GetInstance == null)
        {
            Instantiate(Resources.Load("Prefabs/GameManagerResource"), new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
        }
        if (LevelManager.GetInstance == null)
        {
            Instantiate(Resources.Load("Prefabs/LevelManagerResource"), new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
        }
        if (SoundManager.GetInstance == null)
        {
            Instantiate(Resources.Load("Prefabs/SoundManagerResource"), new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
        }
        if (ScoreManager.GetInstance == null)
        {
            Instantiate(Resources.Load("Prefabs/ScoreManagerResource"), new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
        }
        if (HUDManager.GetInstance == null)
        {
            GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/HUDManagerResource"), new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
            obj.GetComponent<HUDManager>().DisablePlayerHUD();
        }


        // set first scene
        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.MAIN);
    }
}
