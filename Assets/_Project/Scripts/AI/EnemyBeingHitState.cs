using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeingHitState : EnemyBaseState
{
    private EnemyMovement _enemyMovement;

    public override void EnterState(EnemyStateManager ctx)
    {
        _enemyMovement = ctx.gameObject.GetComponent<EnemyMovement>();
        _enemyMovement.SetMovementDirection(Vector3.zero);
    }

    public override void UpdateState(EnemyStateManager ctx)
    {
        
    }

    public override void ExitState(EnemyStateManager ctx)
    {
        
    }
}
