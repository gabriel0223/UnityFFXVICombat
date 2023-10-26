using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerAttackState : BaseState
{
    private InputManager _inputManager;
    private PlayerComboController _playerComboController;
    private AbilityManager _abilityManager;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _inputManager = ctx.gameObject.GetComponent<InputManager>();
        _playerComboController = ctx.gameObject.GetComponent<PlayerComboController>();
        _abilityManager = ctx.gameObject.GetComponent<AbilityManager>();

        _abilityManager.SetCanUseAbilities(true);

        _inputManager.OnAttackPressed += HandlePlayerAttack;
        _inputManager.OnPhoenixShiftPressed += SwitchToPhoenixShift;
        _inputManager.OnDodgePressed += SwitchToDodge;
        _playerComboController.OnComboEnd += SwitchToIdleMove;
        _abilityManager.OnEikonicAbilityExecuted += SwitchToAbilityUse;

        _playerComboController.Attack();
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _abilityManager.SetCanUseAbilities(false);

        _inputManager.OnAttackPressed -= HandlePlayerAttack;
        _inputManager.OnPhoenixShiftPressed -= SwitchToPhoenixShift;
        _inputManager.OnDodgePressed -= SwitchToDodge;
        _playerComboController.OnComboEnd -= SwitchToIdleMove;
        _abilityManager.OnEikonicAbilityExecuted -= SwitchToAbilityUse;
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

    private void SwitchToDodge()
    {
        _stateManager.SwitchState(new PlayerDodgeState());
    }

    private void SwitchToAbilityUse(ButtonDirection buttonDirection, EikonicAbility ability)
    {
        _stateManager.SwitchState(new PlayerUsingAbilityState());
    }
}
