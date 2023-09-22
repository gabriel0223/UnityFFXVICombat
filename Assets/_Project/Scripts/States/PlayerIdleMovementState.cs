using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerIdleMovementState : BaseState
{
    private PlayerMovement _playerMovement;
    private InputManager _inputManager;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _playerMovement = ctx.gameObject.GetComponent<PlayerMovement>();
        _inputManager = ctx.gameObject.GetComponent<InputManager>();

        _inputManager.OnAttackPressed += SwitchToAttack;
        _inputManager.OnPhoenixShiftPressed += SwitchToPhoenixShift;
        _inputManager.OnDodgePressed += SwitchToDodge;
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        _playerMovement.Move();
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _inputManager.OnAttackPressed -= SwitchToAttack;
        _inputManager.OnPhoenixShiftPressed -= SwitchToPhoenixShift;
        _inputManager.OnDodgePressed -= SwitchToDodge;
    }

    private void SwitchToAttack()
    {
        _stateManager.SwitchState(new PlayerAttackState());
    }

    private void SwitchToPhoenixShift()
    {
        _stateManager.SwitchState(new PhoenixShiftState());
    }

    private void SwitchToDodge()
    {
        _stateManager.SwitchState(new PlayerDodgeState());
    }
}
