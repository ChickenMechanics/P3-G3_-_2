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
    public GameObject Player { private set; get; }
    [HideInInspector]
    public float Health { private set; get; }
    [HideInInspector]
    public bool IsAlive { private set; get; }


    public enum EPlayerText
    {
        HEALTH = 0,
        SIZE
    }


    public void DecreaseHealth(float value)
    {
        Health -= value;
        if(Health <= 0.0f)
        {
            IsAlive = false;
        }
    }


    private void Init()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        GameObject resource = (GameObject)Resources.Load("Prefabs/Player");
        Player = Instantiate(resource, transform.position + new Vector3(0.0f, 1.5f, 0.0f), Quaternion.identity, transform);
        Player.transform.parent = transform;

        Health = m_BaseHealth;

        IsAlive = true;
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
