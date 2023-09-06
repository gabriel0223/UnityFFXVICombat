using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;

    public EnemyBaseState CurrentState { get; private set; }
    public EnemyBaseState PreviousState { get; private set; }
    public EnemyOrbitingState OrbitingState { get; private set; }
    public EnemyBeingHitState BeingHitState { get; private set; }

    private void OnEnable()
    {
        _enemyController.OnDie += HandleEnemyDie;
        _enemyController.OnTakeHit += HandleEnemyTakeHit;
    }

    private void OnDisable()
    {
        _enemyController.OnDie -= HandleEnemyDie;
        _enemyController.OnTakeHit -= HandleEnemyTakeHit;
    }

    private void Awake()
    {
        OrbitingState = new EnemyOrbitingState();
        BeingHitState = new EnemyBeingHitState();
    }

    private void Start()
    {
        CurrentState = OrbitingState;
        CurrentState.EnterState(this);
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        CurrentState.ExitState(this);

        PreviousState = CurrentState;
        CurrentState = newState;

        CurrentState?.EnterState(this);
    } 

    private void HandleEnemyDie(EnemyController enemy)
    {
        SwitchState(null);
    }

    private void HandleEnemyTakeHit(EnemyController enemy)
    {
        SwitchState(BeingHitState);
    }
}
