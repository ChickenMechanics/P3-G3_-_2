using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class G_StateMainMenu : IState
{
    public G_StateMainMenu(IController controller)
    {
        m_Owner = (GameManager)controller;
    }

    private GameManager m_Owner;

    public bool GetIsAvailable { get; set; }


    //----------------------------------------------------------------------------------------------------


    public void Enter()
    {

    }


    public void FixedUpdate()
    {

    }


    public void Update()
    {

    }


    public void LateUpdate()
    {

    }


    public void Exit()
    {

    }
}
