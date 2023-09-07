using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyStateManager ctx);
    public abstract void UpdateState(EnemyStateManager ctx);
    public abstract void ExitState(EnemyStateManager ctx);
    public abstract void ReevaluateState(EnemyStateManager ctx);
}
