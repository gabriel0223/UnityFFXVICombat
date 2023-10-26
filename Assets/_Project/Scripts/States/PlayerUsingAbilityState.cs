using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUsingAbilityState : BaseState
{
    private AbilityManager _abilityManager;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _abilityManager = ctx.gameObject.GetComponent<AbilityManager>();

        _abilityManager.AbilityInUse.OnAnimationFinished += HandleAbilityAnimationFinished;
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        
    }

    private void HandleAbilityAnimationFinished()
    {
        _stateManager.SwitchState(new PlayerIdleMovementState());
    }
}
