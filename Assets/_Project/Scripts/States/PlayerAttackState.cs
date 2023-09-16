using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerAttackState : BaseState
{
    private InputManager _inputManager;
    private PlayerComboController _playerComboController;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _inputManager = ctx.gameObject.GetComponent<InputManager>();
        _playerComboController = ctx.gameObject.GetComponent<PlayerComboController>();

        _inputManager.OnAttackPressed += HandlePlayerAttack;
        _inputManager.OnPhoenixShiftPressed += SwitchToPhoenixShift;
        _playerComboController.OnComboEnd += SwitchToIdleMove;

        _playerComboController.Attack();
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _inputManager.OnAttackPressed -= HandlePlayerAttack;
        _inputManager.OnPhoenixShiftPressed -= SwitchToPhoenixShift;
        _playerComboController.OnComboEnd -= SwitchToIdleMove;
    }

    private void HandlePlayerAttack()
    {
        _playerComboController.Attack();
    }

    private void SwitchToIdleMove()
    {
        _stateManager.SwitchState(new PlayerIdleMovementState());
    }

    private void SwitchToPhoenixShift()
    {
        _stateManager.SwitchState(new PhoenixShiftState());
    }
}
