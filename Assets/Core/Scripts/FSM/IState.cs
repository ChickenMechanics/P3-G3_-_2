using System.Collections;
using System.Collections.Generic;


public interface IState
{

    bool GetIsAvailable { get; set; }
    void Enter();
    void FixedUpdate();
    void Update();
    void LateUpdate();
    void Exit();
}
