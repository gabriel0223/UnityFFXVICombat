using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateManager : MonoBehaviour
{
    public BaseState CurrentState { get; private set; }
    public BaseState PreviousState { get; private set; }
    public abstract void SwitchState(BaseState newState);
}
