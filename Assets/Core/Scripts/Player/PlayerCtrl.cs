using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour, IController
{
    public struct BasicInput
    {
        public Vector2 LookInput;
        public Vector2 MoveInput;
    }
    private BasicInput m_BasicInput;

    #region player states
    private IState[] m_States;
    private IState m_IdleState;
    private IState m_WalkState;
    private FSM m_FSM;
    #endregion

    public PlayerLook PlayerLook { get; private set; }

    public enum EP_State
    {
        IDLE = 0,
        WALK,
        RUN,
        DEAD,
        SIZE
    }


    public BasicInput GetBasicInput()
    {
        return m_BasicInput;
    }


    public FSM GetFsm()
    {
        return m_FSM;
    }


    public IState GetState(EP_State state)
    {
        return m_States[(int)state];
    }


    private void UpdateMouseInput()
    {
        m_BasicInput.MoveInput.x = 0.0f;

        m_BasicInput.LookInput.x = Input.GetAxisRaw("Mouse X");
        m_BasicInput.LookInput.y = Input.GetAxisRaw("Mouse Y");
    }


    private void UpdateMoveInput()
    {
        m_BasicInput.MoveInput.x = Input.GetAxisRaw("Horizontal");
        m_BasicInput.MoveInput.y = Input.GetAxisRaw("Vertical");
    }


    private void Awake()
    {
        m_BasicInput = new BasicInput();
        m_BasicInput.LookInput = new Vector2();
        m_BasicInput.MoveInput = new Vector2();

        m_States = new IState[(int)EP_State.SIZE];
        m_States[(int)EP_State.IDLE] = new P_StateIdle((IController)this);
        m_States[(int)EP_State.WALK] = new P_StateWalk((IController)this);

        m_FSM = new FSM(m_States[(int)EP_State.IDLE]);

        PlayerLook = GetComponent<PlayerLook>();
    }


    private void FixedUpdate()
    {
        m_FSM.FixedUpdate();
    }


    private void LateUpdate()
    {
        UpdateMouseInput();
        UpdateMoveInput();
        m_FSM.Update();
    }
}
