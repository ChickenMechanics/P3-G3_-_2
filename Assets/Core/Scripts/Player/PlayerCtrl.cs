using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour, IController
{
    public struct BasicInput
    {
        public Vector3 MoveInput;
        public Vector2 LookInput;
    }
    private BasicInput m_BasicInput;

    #region player states
    private IState[] m_States;
    private IState m_IdleState;
    private IState m_WalkState;
    private FSM m_FSM;
    #endregion

    public enum EP_State
    {
        IDLE = 0,
        WALK,
        RUN,
        DEAD,
        SIZE
    }


    //----------------------------------------------------------------------------------------------------


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


    public void UpdateLookInput()
    {
        m_BasicInput.LookInput.x = Input.GetAxisRaw("Mouse X");
        m_BasicInput.LookInput.y = Input.GetAxisRaw("Mouse Y");
    }


    public void UpdateMoveInput()
    {
        m_BasicInput.MoveInput.x = Input.GetAxisRaw("Horizontal");
        m_BasicInput.MoveInput.z = Input.GetAxisRaw("Vertical");
    }


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        m_BasicInput = new BasicInput();
        m_BasicInput.MoveInput = Vector3.zero;
        m_BasicInput.LookInput = Vector2.zero;

        m_States = new IState[(int)EP_State.SIZE];
        m_States[(int)EP_State.IDLE] = new P_StateIdle((IController)this);
        m_States[(int)EP_State.WALK] = new P_StateWalk((IController)this);

        m_FSM = new FSM(m_States[(int)EP_State.IDLE]);
    }


    private void FixedUpdate()
    {
        m_FSM.FixedUpdate();
    }


    private void Update()
    {
        m_FSM.Update();
    }


    private void LateUpdate()
    {
        m_FSM.LateUpdate();
    }
}
