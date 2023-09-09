using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private float _minTimeBetweenStateReevaluation;
    [SerializeField] private float _maxTimeBetweenStateReevaluation;

    public EnemyBaseState CurrentState { get; private set; }
    public EnemyBaseState PreviousState { get; private set; }
    public EnemyMovingState MovingState { get; private set; }
    public EnemyBeingHitState BeingHitState { get; private set; }
    public EnemyAttackingState AttackingState { get; private set; }

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
        MovingState = new EnemyMovingState();
        BeingHitState = new EnemyBeingHitState();
        AttackingState = new EnemyAttackingState();
    }

    private void Start()
    {
        CurrentState = MovingState;
        CurrentState.EnterState(this);

        StartCoroutine(StateReevalutation());
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        if (newState == CurrentState)
        {
            return;
        }

        CurrentState.ExitState(this);

        PreviousState = CurrentState;
        CurrentState = newState;

        CurrentState?.EnterState(this);
    }

    private IEnumerator StateReevalutation()
    {
        while (CurrentState != null)
        {
            yield return new WaitForSeconds(Random.Range(_minTimeBetweenStateReevaluation, _maxTimeBetweenStateReevaluation));

            CurrentState?.ReevaluateState(this);
        }
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
