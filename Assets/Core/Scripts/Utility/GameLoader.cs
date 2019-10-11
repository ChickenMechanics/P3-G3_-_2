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
        if(SoundManager.GetInstance == null)
        {
            Instantiate(Resources.Load("Prefabs/SoundManagerResource"), new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
        }

        // set first scene
        LevelManager.GetInstance.ChangeScene(LevelManager.EScene.MAIN);
    }
}
