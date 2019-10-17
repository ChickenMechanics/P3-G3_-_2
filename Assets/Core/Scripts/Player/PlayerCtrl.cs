using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour, IController
{
    [HideInInspector]
    public struct BasicInput
    {
        public Vector3 MoveInput;
        public Vector2 LookInput;
        public float RunInput;
        public float DashInput;
    }
    [HideInInspector]
    public BasicInput GetBasicInput;

    [HideInInspector]
    public FSM GetFSM { private set; get; }


    public enum EPlayerState
    {
        IDLE = 0,
        WALK,
        RUN,
        DASH,
        SIZE
        //DEAD, // Exists in player manager. Should prob be organized in a better way
    }

    private FxRumbleTransform m_FxRumbleScr;


    //----------------------------------------------------------------------------------------------------


    //public BasicInput GetBasicInput()
    //{
    //    return GetBasicInput;
    //}


    public FSM GetFsm()
    {
        return GetFSM;
    }


    public void UpdateLookInput()
    {
        GetBasicInput.LookInput.x = Input.GetAxisRaw("Mouse X");
        GetBasicInput.LookInput.y = Input.GetAxisRaw("Mouse Y");
    }


    public void UpdateMoveInput()
    {
        GetBasicInput.MoveInput.x = Input.GetAxisRaw("Horizontal");
        GetBasicInput.MoveInput.z = Input.GetAxisRaw("Vertical");

        GetBasicInput.RunInput = Input.GetAxisRaw("Run");
    }


    public void UpdateDashInput()
    {
        GetBasicInput.DashInput = Input.GetAxisRaw("Dash");
    }


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        GetBasicInput = new BasicInput();
        GetBasicInput.MoveInput = Vector3.zero;
        GetBasicInput.LookInput = Vector2.zero;
        GetBasicInput.RunInput = 0.0f;
        GetBasicInput.DashInput = 0.0f;

        // add states in the same order as they appear in the state enum, located in this file
        GetFSM = new FSM(this);
        GetFSM.AddState(new P_StateIdle(this));
        GetFSM.AddState(new P_StateWalk(this));
        GetFSM.AddState(new P_StateRun(this));
        GetFSM.AddState(new P_StateDash(this));
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


    private void OnDestroy()
    {
        Destroy(this);
    }
}
