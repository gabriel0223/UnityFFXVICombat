using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrbitingState : EnemyBaseState
{
    private PlayerCombatController _player;
    private EnemyMovement _enemyMovement;

    public override void EnterState(EnemyStateManager ctx)
    {
        _player = GameObject.FindObjectOfType<PlayerCombatController>();
        _enemyMovement = ctx.gameObject.GetComponent<EnemyMovement>();
    }

    public override void UpdateState(EnemyStateManager ctx)
    {
        Vector3 playerDirection = (_player.transform.position - ctx.transform.position).normalized;

        _enemyMovement.RotateTowardsDirection(playerDirection);
    }
}
