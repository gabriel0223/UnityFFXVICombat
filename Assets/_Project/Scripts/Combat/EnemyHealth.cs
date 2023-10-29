using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHealth : HealthBase
{
    public event Action OnTargeted;
    public event Action OnUntargeted;
    public event Action OnGetFireDamage;

    private CharacterController _controller;
    private Animator _animator;

    private void Awake()
    {
        CurrentHealth = _initialHealth;

        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
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
            int randomAnimationIndex = Random.Range(0, AnimationParameters.EnemyTakeDamage.Length);
            _animator.SetTrigger(AnimationParameters.EnemyTakeDamage[randomAnimationIndex]);
        }

        OnTakeDamage?.Invoke(this);

        if (damageType == DamageType.Fire)
        {
            OnGetFireDamage?.Invoke();
        }
    }

    public override void Die()
    {
        _animator.SetTrigger(AnimationParameters.Death);
        _controller.enabled = false;

        OnDie?.Invoke(this);
    }

    public void HandleTargeted()
    {
        OnTargeted?.Invoke();
    }

    public void HandleUntargeted()
    {
        OnUntargeted?.Invoke();
    }
}
