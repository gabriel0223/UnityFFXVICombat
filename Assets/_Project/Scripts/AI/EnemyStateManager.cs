using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private float _minTimeBetweenStateReevaluation;
    [SerializeField] private float _maxTimeBetweenStateReevaluation;

    public EnemyBaseState CurrentState { get; private set; }
    public EnemyBaseState PreviousState { get; private set; }
    public EnemyMovingState MovingState { get; private set; }
    public EnemyBeingHitState BeingHitState { get; private set; }
    public EnemyAttackingState AttackingState { get; private set; }

    private void OnEnable()
    {
        _enemyHealth.OnDie += HandleEnemyDie;
        _enemyHealth.OnTakeDamage += HandleEnemyTakeHit;
    }

    private void OnDisable()
    {
        _enemyHealth.OnDie -= HandleEnemyDie;
        _enemyHealth.OnTakeDamage -= HandleEnemyTakeHit;
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
        if (_enemyHealth.CurrentHealth <= 0)
        {
            return;
        }

        CurrentState?.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        if (newState == CurrentState || _enemyHealth.CurrentHealth <= 0)
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

    private void HandleEnemyDie(HealthBase enemy)
    {
        SwitchState(null);
    }

    private void HandleEnemyTakeHit(HealthBase enemy)
    {
        SwitchState(BeingHitState);
    }
}
