using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBase : MonoBehaviour, IDamageable
{
    public Action<HealthBase> OnTakeDamage;
    public Action<HealthBase> OnDie;

    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _initialHealth;
    public int CurrentHealth { get; protected set; }
    public int MaxHealth => _maxHealth;

    public abstract void TakeDamage(int damage, DamageType damageType);
    public abstract void Die();
}
