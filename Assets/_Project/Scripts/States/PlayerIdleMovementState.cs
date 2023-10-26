using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerIdleMovementState : BaseState
{
    private PlayerMovement _playerMovement;
    private InputManager _inputManager;
    private AbilityManager _abilityManager;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _playerMovement = ctx.gameObject.GetComponent<PlayerMovement>();
        _inputManager = ctx.gameObject.GetComponent<InputManager>();
        _abilityManager = ctx.gameObject.GetComponent<AbilityManager>();

        _abilityManager.SetCanUseAbilities(true);

        _inputManager.OnAttackPressed += SwitchToAttack;
        _inputManager.OnPhoenixShiftPressed += SwitchToPhoenixShift;
        _inputManager.OnDodgePressed += SwitchToDodge;
        _abilityManager.OnEikonicAbilityExecuted += SwitchToAbilityUse;
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        _playerMovement.Move();
    }

    public override void ExitState(BaseStateManager ctx)
    {
        _abilityManager.SetCanUseAbilities(false);

        _inputManager.OnAttackPressed -= SwitchToAttack;
        _inputManager.OnPhoenixShiftPressed -= SwitchToPhoenixShift;
        _inputManager.OnDodgePressed -= SwitchToDodge;
        _abilityManager.OnEikonicAbilityExecuted -= SwitchToAbilityUse;
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

    private void SwitchToAbilityUse(ButtonDirection buttonDirection, EikonicAbility ability)
    {
        _stateManager.SwitchState(new PlayerUsingAbilityState());
    }
}
