using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeingHitState : EnemyBaseState
{
    private const float TimeToReturnToMovement = 1.25f;
    private const float KnockbackForce = 2f;
    private const float KnockbackDuration = 0.5f;

    private EnemyMovement _enemyMovement;
    private EnemyHealth _enemyHealth;
    private DashController _dashController;
    private PlayerCombatController _player;
    private float _timer;

    public override void EnterState(EnemyStateManager ctx)
    {
        _timer = TimeToReturnToMovement;
        _enemyMovement = ctx.gameObject.GetComponent<EnemyMovement>();
        _enemyHealth = ctx.gameObject.GetComponent<EnemyHealth>();
        _dashController = ctx.gameObject.GetComponent<DashController>();
        _player = GameObject.FindObjectOfType<PlayerCombatController>();

        _enemyMovement.SetMovementDirection(Vector3.zero);
        ApplyKnockbackAwayFromPlayer();

        _enemyHealth.OnTakeDamage += HandleEnemyTakeHit;

    }

    public override void UpdateState(EnemyStateManager ctx)
    {
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            ctx.SwitchState(ctx.MovingState);
        }
    }

    public override void ExitState(EnemyStateManager ctx)
    {
        _enemyHealth.OnTakeDamage -= HandleEnemyTakeHit;
    }

    public override void ReevaluateState(EnemyStateManager ctx)
    {
        
    }

    private void HandleEnemyTakeHit(HealthBase enemy)
    {
        _timer = TimeToReturnToMovement;

        ApplyKnockbackAwayFromPlayer();
    }

    private void ApplyKnockbackAwayFromPlayer()
    {
        Vector3 playerDirection = (_player.transform.position - _enemyHealth.transform.position).normalized;

        _enemyMovement.SetMovementDirection(Vector3.zero);
        _dashController.ExecuteDash(-playerDirection * KnockbackForce, KnockbackDuration);
    }
}
