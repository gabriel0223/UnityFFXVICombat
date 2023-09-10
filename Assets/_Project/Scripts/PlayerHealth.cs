using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public event Action OnTakeDamage;

    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _initialHealth;
    public int CurrentHealth { get; private set; }
    public int MaxHealth => _maxHealth;

    private void Awake()
    {
        CurrentHealth = _initialHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            //Die();
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
        OnTakeDamage?.Invoke();

        yield return new WaitForSeconds(1f);

        _playerStateManager.SetPlayerState(PlayerState.Idle);
    }
}
