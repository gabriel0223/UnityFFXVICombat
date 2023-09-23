using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerDodgeState : BaseState
{
    private InputManager _inputManager;
    private DodgeController _dodgeController;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _dodgeController = ctx.gameObject.GetComponent<DodgeController>();
        _inputManager = ctx.gameObject.GetComponent<InputManager>();

        _dodgeController.TriggerDodge();

        _dodgeController.OnDodgeEnd += SwitchToIdleMove;
        _inputManager.OnDodgePressed += HandleDodgePressed;
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _dodgeController.OnDodgeEnd -= SwitchToIdleMove;
        _inputManager.OnDodgePressed -= HandleDodgePressed;
    }

    private void HandleDodgePressed()
    {
        _dodgeController.TriggerDodge();
    }

    private void SwitchToIdleMove()
    {
        _stateManager.SwitchState(new PlayerIdleMovementState());
    }
}
