using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _vfxImpactPrefab;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private Collider _collider;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private int _initialDamage;
    [SerializeField] private int _damageVariation;

    private void Awake()
    {
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out IDamageable damageable) || 
            ((1 << other.gameObject.layer) & _targetLayer) == 0)
        {
            return;
        }

        damageable.TakeDamage(_initialDamage + Random.Range(-_damageVariation, _damageVariation));

        CharacterController targetCC = other.gameObject.GetComponent<CharacterController>();
        Vector3 targetCenterPosition = other.bounds.center;
        Vector3 targetDirection = (targetCenterPosition - _raycastOrigin.position).normalized;
        float targetDistance = Vector3.Distance(_raycastOrigin.position, targetCenterPosition);
        Vector3 impactOffset = targetDirection + (transform.root.position - targetCenterPosition) * targetCC.radius / 2;
        Vector3 impactPoint = _raycastOrigin.position + (targetDirection * targetDistance) - impactOffset;

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
