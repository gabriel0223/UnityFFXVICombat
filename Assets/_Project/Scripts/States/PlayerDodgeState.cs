using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : BaseState
{
    private DodgeController _dodgeController;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _dodgeController = ctx.gameObject.GetComponent<DodgeController>();

        _dodgeController.Dodge();
        _dodgeController.OnDodgeEnd += SwitchToIdleMove;
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _dodgeController.OnDodgeEnd -= SwitchToIdleMove;
    }

    private void SwitchToIdleMove()
    {
        _stateManager.SwitchState(new PlayerIdleMovementState());
    }

}
