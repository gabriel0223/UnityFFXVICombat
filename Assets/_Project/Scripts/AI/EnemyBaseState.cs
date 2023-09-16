using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : BaseState
{
    public abstract void ReevaluateState(EnemyStateManager ctx);
}
