using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A state manager that controls player states and their transitions.
/// </summary>
public class PlayerStateManager : BaseStateManager
{ 
    public BaseState CurrentState { get; private set; }
    public BaseState PreviousState { get; private set; }
    public PlayerIdleMovementState IdleMovementState { get; private set; }

    private void Awake()
    {
        IdleMovementState = new PlayerIdleMovementState();
    }

    private void Start()
    {
        CurrentState = IdleMovementState;
        CurrentState.EnterState(this);
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public override void SwitchState(BaseState newState)
    {
        CurrentState.ExitState(this);

        PreviousState = CurrentState;
        CurrentState = newState;

        CurrentState?.EnterState(this);
    }
}
