using System.Collections;
using System.Collections.Generic;


public interface IState
{
    bool GetIsAvailable();
    void SetIsAvailable(bool isAvailable);
    void Enter();
    void FixedUpdate();
    void Update();
    void LateUpdate();
    void Exit();
}
