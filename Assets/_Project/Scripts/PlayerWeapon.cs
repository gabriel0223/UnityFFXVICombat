using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private int _initialDamage;
    [SerializeField] private int _damageVariation;

    private void Awake()
    {
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out IDamageable damageable))
        {
             return;
        }

        damageable.TakeDamage(_initialDamage + Random.Range(-_damageVariation, _damageVariation));
    }

    public void SetColliderActive(bool active)
    {
        _collider.enabled = active;
    }
}
