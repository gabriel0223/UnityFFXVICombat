using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private const float TimeToReturnToMovement = 1f;
    private const float DistanceToAttack = 1f;

    private EnemyCombat _enemyCombat;
    private PlayerCombatController _player;
    private EnemyMovement _enemyMovement;
    private float _timer;
    private bool _hasAttacked;

    public override void EnterState(EnemyStateManager ctx)
    {
        _hasAttacked = false;
        _timer = TimeToReturnToMovement;
        _player = GameObject.FindObjectOfType<PlayerCombatController>();
        _enemyCombat = ctx.gameObject.GetComponent<EnemyCombat>();
        _enemyMovement = ctx.gameObject.GetComponent<EnemyMovement>();
    }

    public override void UpdateState(EnemyStateManager ctx)
    {
        if (_hasAttacked)
        {
            _timer -= Time.deltaTime;

            if (_timer < 0)
            {
                ctx.SwitchState(ctx.MovingState);
            }

            return;
        }
        
        Vector3 playerDirection = (_player.transform.position - ctx.transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(ctx.transform.position, _player.transform.position);

        if (distanceToPlayer >= DistanceToAttack)
        {
            _enemyMovement.SetMovementDirection(playerDirection);
            _enemyMovement.SetIsChashing(true);
        }
        else
        {
            _enemyCombat.Attack();
            _hasAttacked = true;

            _enemyMovement.SetIsChashing(false);
            _enemyMovement.SetMovementDirection(Vector3.zero);
        }

        _enemyMovement.RotateTowardsDirection(playerDirection);
    }

    public override void ExitState(EnemyStateManager ctx)
    {
        
    }

    public override void ReevaluateState(EnemyStateManager ctx)
    {
        
    }
}
