using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected BaseStateManager _stateManager;

    public virtual void EnterState(BaseStateManager ctx)
    {
        _stateManager = ctx;
    }
    public abstract void UpdateState(BaseStateManager ctx);
    public abstract void ExitState(BaseStateManager ctx);
}
