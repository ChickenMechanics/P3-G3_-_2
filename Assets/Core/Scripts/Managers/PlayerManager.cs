using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager GetInstance { private set; get; }

    #region design vars
    public float m_BaseHealth = 100.0f;
    public float m_TakeDmgShake = 0.5f;
    #endregion
    
    [HideInInspector]
    public GameObject GetPlayer { private set; get; }
    [HideInInspector]
    public PlayerCtrl GetPlayerCtrlScr { private set; get; }
    [HideInInspector]
    public PlayerLook GetPlayerLookScr { private set; get; }
    [HideInInspector]
    public PlayerMove GetPlayerMoveScr { private set; get; }
    [HideInInspector]
    public float GetBaseHealth { private set; get; }
    [HideInInspector]
    public float GetCurrentHealth { private set; get; }
    [HideInInspector]
    public bool GetIsAlive { private set; get; }

    // screen shakes
    private float m_ShakePrevHealth;
    private float m_ShakeStartTime;
    private float m_ShakeTime;


    //----------------------------------------------------------------------------------------------------


    public void DecreaseHealth(float value)
    {
        GetCurrentHealth -= value;
        if(GetCurrentHealth <= 0.0f)
        {
            GetIsAlive = false;
        }
    }


    private void Init()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        GameObject resource = (GameObject)Resources.Load("Prefabs/Player");
        GetPlayer = Instantiate(resource, transform.position + new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity, transform);

        GetPlayerCtrlScr = GetPlayer.GetComponent<PlayerCtrl>();
        GetPlayerLookScr = GetPlayer.GetComponent<PlayerLook>();
        GetPlayerMoveScr = GetPlayer.GetComponent<PlayerMove>();

        GetBaseHealth = m_BaseHealth;
        GetCurrentHealth = GetBaseHealth;

        GetIsAlive = true;

        m_ShakePrevHealth = GetCurrentHealth;
        m_ShakeStartTime = Time.time;
        m_ShakeTime = 0.25f;
    }


    private IEnumerator PlayerScreenShake()
    {
        while (Time.time < m_ShakeStartTime + m_ShakeTime)
        {
            Vector3 ranPos = new Vector3(Random.Range(-m_TakeDmgShake, m_TakeDmgShake),
                Random.Range(-m_TakeDmgShake, m_TakeDmgShake),
                Random.Range(-m_TakeDmgShake, m_TakeDmgShake));

            Vector3 nowPos = GetPlayerLookScr.gameObject.transform.position;
            GetPlayerLookScr.gameObject.transform.position = nowPos + ranPos;

            yield return null;
        }

        StopCoroutine(PlayerScreenShake());
    }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        Init();
    }


    private void Update()
    {
        if (GetCurrentHealth <= 0.0f)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);
        }

        // shakes
        if(GetCurrentHealth != m_ShakePrevHealth)
        {
            m_ShakePrevHealth = GetCurrentHealth;
            m_ShakeStartTime = Time.time;
            StartCoroutine(PlayerScreenShake());
        }
    }
}
