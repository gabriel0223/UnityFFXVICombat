using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerStateManager _playerStateManager;

    private void Awake()
    {
        CurrentHealth = _initialHealth;
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;

            Die();
        }
        else
        {
            StartCoroutine(TakeDamageCoroutine());
            //OnTakeHit?.Invoke(this);
        }
    }

    private IEnumerator TakeDamageCoroutine()
    {
        if (_playerStateManager.PlayerState == PlayerState.TakingDamage)
        {
            yield break;
        }

        _animator.SetTrigger("TakeDamage");
        _playerStateManager.SetPlayerState(PlayerState.TakingDamage);
        OnTakeDamage?.Invoke(this);

        yield return new WaitForSeconds(1f);

        _playerStateManager.SetPlayerState(PlayerState.Idle);
    }

    public override void Die()
    {
        OnDie?.Invoke(this);
    }
}
