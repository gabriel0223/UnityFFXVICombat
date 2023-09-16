using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PhoenixShiftState : BaseState
{
    private InputManager _inputManager;
    private PhoenixShift _phoenixShift;
    private bool _hasSwitchedToAttack;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _inputManager = ctx.GetComponent<InputManager>();
        _phoenixShift = ctx.GetComponent<PhoenixShift>();

        _inputManager.OnAttackPressed += HandleAttackPressed;
        _phoenixShift.OnShiftEnd += SwitchToIdle;
        _phoenixShift.StartPhoenixShift();
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _phoenixShift.OnShiftEnd -= SwitchToIdle;
    }

    private void HandleAttackPressed()
    {
        if (_phoenixShift.IsWaitingForAttack && !_hasSwitchedToAttack)
        {
            SwitchToAttack();
        }
    }

    private void SwitchToIdle()
    {
        if (_hasSwitchedToAttack)
        {
            return;
        }

        _stateManager.SwitchState(new PlayerIdleMovementState());
    }

    private void SwitchToAttack()
    {
        _hasSwitchedToAttack = true;
        _stateManager.SwitchState(new PlayerAttackState());
    }
}
