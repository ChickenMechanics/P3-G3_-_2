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

    [HideInInspector]
    public FSM GetFSM { private set; get; }

    public enum EPlayerState
    {
        IDLE = 0,
        WALK,
        RUN,    // not implemented. Here if needed
        DASH,   // not implemented
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
        return GetFSM;
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

        GetFSM = new FSM(this);
        GetFSM.AddState(new P_StateIdle((IController)this));
        GetFSM.AddState(new P_StateWalk((IController)this));
        GetFSM.Init();
    }


    private void FixedUpdate()
    {
        GetFSM.FixedUpdate();
    }


    private void Update()
    {
        GetFSM.Update();
    }


    private void LateUpdate()
    {
        GetFSM.LateUpdate();
    }
}
