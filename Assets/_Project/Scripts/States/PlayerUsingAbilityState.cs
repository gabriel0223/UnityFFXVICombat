using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUsingAbilityState : BaseState
{
    private AbilityManager _abilityManager;
    private PlayerHealth _playerHealth;

    public override void EnterState(BaseStateManager ctx)
    {
        base.EnterState(ctx);

        _abilityManager = ctx.gameObject.GetComponent<AbilityManager>();
        _playerHealth = ctx.gameObject.GetComponent<PlayerHealth>();

        _abilityManager.AbilityInUse.OnAnimationFinished += HandleAbilityAnimationFinished;

        _playerHealth.SetInvulnerability(true);
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        
    }

    public override void ExitState(BaseStateManager ctx)
    {
        
    }

    private void HandleAbilityAnimationFinished()
    {
        _playerHealth.SetInvulnerability(false);

        _stateManager.SwitchState(new PlayerIdleMovementState());
    }
}
