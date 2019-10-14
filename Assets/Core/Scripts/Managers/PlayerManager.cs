using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager GetInstance { private set; get; }

    #region design vars
    public float m_BaseHealth = 100.0f;
    public float m_TakeDmgShakeIntensity = 0.5f;
    public float m_TakeDmgShakeTime = 0.25f;
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
    public bool GetIsUndead { private set; get; }
    [HideInInspector]
    public bool GetIsAlive { private set; get; }
    [HideInInspector]

    // screen shakes
    private float m_ShakePrevHealth;
    private float m_ShakeStartTime;


    //----------------------------------------------------------------------------------------------------


    public void DecreaseHealth(float value)
    {
#if DEBUG
        if(GetIsUndead == true)
        {
            return;
        }
#endif

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
        GetPlayer = Instantiate(resource, transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity, transform);

        GetPlayerCtrlScr = GetPlayer.GetComponent<PlayerCtrl>();
        GetPlayerLookScr = GetPlayer.GetComponent<PlayerLook>();
        GetPlayerMoveScr = GetPlayer.GetComponent<PlayerMove>();

        GetBaseHealth = m_BaseHealth;
        GetCurrentHealth = GetBaseHealth;

        GetIsAlive = true;
        GetIsUndead = false;

        m_ShakePrevHealth = GetCurrentHealth;
        m_ShakeStartTime = Time.time;
    }


    private IEnumerator PlayerScreenShake()
    {
        while (Time.time < m_ShakeStartTime + m_TakeDmgShakeTime)
        {
            Vector3 ranPos = new Vector3(Random.Range(-m_TakeDmgShakeIntensity, m_TakeDmgShakeIntensity),
                Random.Range(0.0f, 0.0f),
                Random.Range(-m_TakeDmgShakeIntensity, m_TakeDmgShakeIntensity));

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
#if DEBUG
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetIsUndead = !GetIsUndead;

            string msg = GetIsUndead ? "Godmode On" : "Godmode Off";
            Debug.LogError(msg);
        }
#endif

        if (GetCurrentHealth != m_ShakePrevHealth)
        {
            SoundManager.GetInstance.PlaySoundClip(SoundManager.ESoundClip.PLAYER_HURT, transform.position);

            m_ShakePrevHealth = GetCurrentHealth;
            m_ShakeStartTime = Time.time;
            StartCoroutine(PlayerScreenShake());
        }

        // TODO: move below blocks to gamemanager
        if (GetIsAlive == false &&
            GetIsUndead == false)
        {
            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            LevelManager.GetInstance.ChangeScene(LevelManager.EScene.END);
        }
                
    }
}
