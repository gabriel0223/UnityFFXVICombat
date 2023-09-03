using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject _vfxImpactPrefab;
    [SerializeField] private Transform _raycastOrigin;
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

        CharacterController enemyCc = other.gameObject.GetComponent<CharacterController>();
        Vector3 enemyCenterPosition = other.bounds.center;
        Vector3 enemyDirection = (enemyCenterPosition - _raycastOrigin.position).normalized;
        float enemyDistance = Vector3.Distance(_raycastOrigin.position, enemyCenterPosition);
        Vector3 impactOffset = enemyDirection + (transform.root.position - enemyCenterPosition) * enemyCc.radius / 2;
        Vector3 impactPoint = _raycastOrigin.position + (enemyDirection * enemyDistance) - impactOffset;

        SpawnHitVFX(impactPoint);
    }

    public void SetColliderActive(bool active)
    {
        _collider.enabled = active;
    }

    private void SpawnHitVFX(Vector3 position)
    {
        Instantiate(_vfxImpactPrefab, position, Quaternion.identity);
    }
}
