using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager GetInstance { private set; get; }

    #region design vars
    public float m_BaseHealth = 100.0f;
    #endregion
    
    [HideInInspector]
    public GameObject GetPlayer { private set; get; }
    private PlayerCtrl m_PlayerCtrlScr;
    private PlayerLook m_PlayerLook;
    private PlayerMove m_PlayerMove;
    [HideInInspector]
    public float GetHealth { private set; get; }
    [HideInInspector]
    public bool GetIsAlive { private set; get; }


    //----------------------------------------------------------------------------------------------------


    public enum EPlayerText
    {
        HEALTH = 0,
        SIZE
    }


    public void DecreaseHealth(float value)
    {
        GetHealth -= value;
        if(GetHealth <= 0.0f)
        {
            GetIsAlive = false;
        }
    }


    private void Init()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        GameObject resource = (GameObject)Resources.Load("Prefabs/Player");
        GetPlayer = Instantiate(resource, transform.position + new Vector3(0.0f, 1.5f, 0.0f), Quaternion.identity, transform);
        GetPlayer.transform.parent = transform;

        m_PlayerCtrlScr = GetPlayer.GetComponent<PlayerCtrl>();
        m_PlayerLook = GetPlayer.GetComponent<PlayerLook>();
        m_PlayerMove = GetPlayer.GetComponent<PlayerMove>();

        GetHealth = m_BaseHealth;

        GetIsAlive = true;
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
}
