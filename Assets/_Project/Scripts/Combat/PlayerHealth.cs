using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerStateManager _playerStateManager;

    private bool _isTakingDamage;

    private void Awake()
    {
        CurrentHealth = _initialHealth;
    }

    public override void TakeDamage(int damage, DamageType damageType)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;

            Die();
        }
        else
        {
            OnTakeDamage?.Invoke(this);
            StartCoroutine(TakeDamageCoroutine());
        }
    }

    public void SetInvulnerability(bool state)
    {
        IsInvulnerable = state;
    }

    /// <summary>
    /// Makes the player invulnerable for a set amount of time.
    /// </summary>
    /// <param name="timer"></param>
    public void StartInvulnerabilityTimer(float timer)
    {
        IsInvulnerable = true;
        DOVirtual.DelayedCall(timer, () => IsInvulnerable = false);
    }

    private IEnumerator TakeDamageCoroutine()
    {
        if (_isTakingDamage)
        {
            yield break;
        }

        _animator.SetTrigger(AnimationParameters.PlayerTakeDamage);
        _playerStateManager.SwitchState(new PlayerTakeDamageState());
        _isTakingDamage = true;

        yield return new WaitForSeconds(1f);

        if (CurrentHealth <= 0)
        {
            yield break;
        }       

        _isTakingDamage = false;
        _playerStateManager.SwitchState(new PlayerIdleMovementState());
    }

    public override void Die()
    {
        OnDie?.Invoke(this);
    }
}
