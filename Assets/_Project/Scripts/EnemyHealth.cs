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

    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;

    private readonly string[] _takeDamageAnimations = new[] { "TakeDamage1", "TakeDamage2" };

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
            int randomAnimationIndex = Random.Range(0, _takeDamageAnimations.Length);
            _animator.SetTrigger(_takeDamageAnimations[randomAnimationIndex]);
        }

        OnTakeDamage?.Invoke(this);

        if (damageType == DamageType.Fire)
        {
            OnGetFireDamage?.Invoke();
        }
    }

    public override void Die()
    {
        _animator.SetTrigger("Death");
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
