using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public EnemyBaseState CurrentState { get; private set; }
    public EnemyOrbitingState OrbitingState { get; private set; }

    private void Awake()
    {
        OrbitingState = new EnemyOrbitingState();
    }

    private void Start()
    {
        CurrentState = OrbitingState;
        CurrentState.EnterState(this);
    }

    private void Update()
    {
        CurrentState.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        CurrentState = newState;
        CurrentState.EnterState(this);
    }
}
