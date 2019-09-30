using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager GetInstance { private set; get; }

    #region design vars
    public float m_Health = 100.0f;
    #endregion
    
    [HideInInspector]
    public GameObject Player { private set; get; }
    private float m_CurrentHealth;
    private bool m_IsAlive;


    public void DecreaseHealth(float value)
    {
        m_CurrentHealth -= value;
        if(m_CurrentHealth <= 0.0f)
        {
            m_IsAlive = false;
        }
    }


    private void Init()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        GameObject resource = (GameObject)Resources.Load("Prefabs/Player Variant");
        Player = Instantiate(resource, transform.position + new Vector3(0.0f, 1.5f, 0.0f), Quaternion.identity, transform);
        Player.transform.parent = transform;

        m_CurrentHealth = m_Health;

        m_IsAlive = true;
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
