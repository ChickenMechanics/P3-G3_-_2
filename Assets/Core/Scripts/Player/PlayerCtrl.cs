using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour, IController
{
    #region player states
    private IState[] m_States;
    private IState m_IdleState;
    private IState m_WalkState;
    private FSM m_FSM;
    #endregion


    private Vector2 m_MouseInput;
    private Vector2 m_MoveInput;
    [HideInInspector]
    public PlayerLook m_PlayerLook { get; private set; }

    public enum EP_State
    {
        IDLE = 0,
        WALK,
        RUN,
        DEAD,
        SIZE
    }


    public FSM GetFsm()
    {
        return m_FSM;
    }


    public IState GetState(EP_State state)
    {
        return m_States[(int)state];
    }


    public Vector2 GetMouseInput()
    {
        return m_MouseInput;
    }


    public Vector2 GetMoveInput()
    {
        return m_MoveInput;
    }


    private void UpdateMouseInput()
    {
        m_MouseInput.x = Input.GetAxisRaw("Mouse X");
        m_MouseInput.y = Input.GetAxisRaw("Mouse Y");
    }


    private void UpdateMoveInput()
    {
        m_MoveInput.x = Input.GetAxisRaw("Horizontal");
        m_MoveInput.y = Input.GetAxisRaw("Vertical");
    }


    private void Awake()
    {
        m_States = new IState[(int)EP_State.SIZE];
        m_States[(int)EP_State.IDLE] = new P_StateIdle((IController)this);
        m_States[(int)EP_State.WALK] = new P_StateWalk((IController)this);

        m_FSM = new FSM(m_States[(int)EP_State.IDLE]);

        m_MouseInput = new Vector2();
        m_MoveInput = new Vector2();

        m_PlayerLook = GetComponent<PlayerLook>();
    }


    private void Update()
    {
        UpdateMouseInput();
        UpdateMoveInput();
        m_FSM.Update();
    }

}
