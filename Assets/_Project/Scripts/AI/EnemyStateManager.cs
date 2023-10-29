using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A state manager for the enemies' AI. It can switch between
/// states, and update and reevaluate them.
/// </summary>
public class EnemyStateManager : BaseStateManager
{
    [Tooltip("Minimum time for a state reevaluation to happen")]
    [SerializeField] private float _minTimeBetweenStateReevaluation;
    [Tooltip("Maximum time for a state reevaluation to happen")]
    [SerializeField] private float _maxTimeBetweenStateReevaluation;

    private EnemyHealth _enemyHealth;

    public EnemyBaseState CurrentState { get; private set; }
    public EnemyBaseState PreviousState { get; private set; }
    public EnemyMovingState MovingState { get; private set; }
    public EnemyBeingHitState BeingHitState { get; private set; }
    public EnemyAttackingState AttackingState { get; private set; }

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();

        MovingState = new EnemyMovingState();
        BeingHitState = new EnemyBeingHitState();
        AttackingState = new EnemyAttackingState();
    }

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

    public override void SwitchState(BaseState newState)
    {
        if (newState == CurrentState || _enemyHealth.CurrentHealth <= 0)
        {
            return;
        }

        CurrentState.ExitState(this);

        PreviousState = CurrentState;
        CurrentState = (EnemyBaseState)newState;

        CurrentState?.EnterState(this);
    }

    /// <summary>
    /// Reevaluates the enemy state after a random time.
    /// Each state can implement its own reevaluation, which can change
    /// behaviour or transition to a different state.
    /// </summary>
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
