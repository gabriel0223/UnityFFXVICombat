using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IDamageable
{
    public event Action<EnemyController> OnDie;
    public event Action<EnemyController> OnTakeHit; 
    public event Action OnTargeted;
    public event Action OnUntargeted;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _initialHealth;

    private readonly string[] _takeDamageAnimations = new[] { "TakeDamage1", "TakeDamage2" };

    private int _health;

    private void Awake()
    {
         _health = _initialHealth;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
        else
        {
            int randomAnimationIndex = Random.Range(0, _takeDamageAnimations.Length);

            _animator.SetTrigger(_takeDamageAnimations[randomAnimationIndex]);
            OnTakeHit?.Invoke(this);
        }
    }

    public void HandleTargeted()
    {
        OnTargeted?.Invoke();
    }

    public void HandleUntargeted()
    {
        OnUntargeted?.Invoke();
    }

    private void Die()
    {
        _animator.SetTrigger("Death");
        _controller.enabled = false;

        OnDie?.Invoke(this);
    }
}
