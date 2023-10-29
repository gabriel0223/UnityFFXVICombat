using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Base class for weapons that damages both
/// the player and the enemies.
/// </summary>
public class WeaponController : MonoBehaviour
{
    public static event Action<HealthBase, int> OnWeaponHitHealth;

    [SerializeField] private GameObject _vfxImpactPrefab;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private Collider _collider;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private int _initialDamage;
    [SerializeField] private int _damageVariation;

    private bool _canDoDamage;
    private AttackData _currentAttackDataData;
    private List<Collider> _enemiesHitInThisAttack;

    private void Update()
    {
        if (!_canDoDamage)
        {
            return;
        }

        CheckForOverlaps();
    }

    /// <summary>
    /// Checks if anything damageable is overlapping
    /// the weapon collider.
    /// </summary>
    private void CheckForOverlaps()
    {
        Vector3 boxCenter = _collider.bounds.center;
        Vector3 halfExtents = _collider.bounds.extents;
        Quaternion boxOrientation = transform.rotation;
        
        Collider[] overlappingColliders = Physics.OverlapBox(boxCenter, halfExtents, boxOrientation, _targetLayer);

        if (overlappingColliders.Length == 0)
        {
            return;
        }

        foreach (Collider collider in overlappingColliders)
        {
            if (_enemiesHitInThisAttack.Contains(collider))
            {
                return;
            }

            HandleOverlap(collider);
        }
    }

    /// <summary>
    /// Handles damageable overlapping weapon collider.
    /// </summary>
    /// <param name="other"></param>
    private void HandleOverlap(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out IDamageable damageable) || 
            ((1 << other.gameObject.layer) & _targetLayer) == 0)
        {
            return;
        }

        int hitDamage = (int)(_initialDamage * _currentAttackDataData.DamageMultiplier) 
                        + Random.Range(-_damageVariation, _damageVariation);

        if (other.gameObject.TryGetComponent(out HealthBase health))
        {
            if (health.IsInvulnerable)
            {
                return;
            }

            OnWeaponHitHealth?.Invoke(health, hitDamage);
        }

        damageable.TakeDamage(hitDamage, _currentAttackDataData.DamageType);
        _enemiesHitInThisAttack.Add(other);

        Vector3 impactPoint = GetImpactPoint(other);

        SpawnHitVFX(impactPoint);
    }

    public void SetAttackData(AttackData newAttackData)
    {
        _currentAttackDataData = newAttackData;
    }

    public void SetCanDoDamage(bool state)
    {
        _canDoDamage = state;
        _enemiesHitInThisAttack = new List<Collider>();
    }

    /// <summary>
    /// Get the impact point where the weapon hit the damageable.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public Vector3 GetImpactPoint(Collider collider)
    {
        CharacterController targetCc = collider.gameObject.GetComponent<CharacterController>();
        Vector3 targetCenterPosition = collider.bounds.center;
        Vector3 targetDirection = (targetCenterPosition - _raycastOrigin.position).normalized;
        float targetDistance = Vector3.Distance(_raycastOrigin.position, targetCenterPosition);
        Vector3 impactOffset = targetDirection + (transform.root.position - targetCenterPosition) * targetCc.radius / 2;
        Vector3 impactPoint = _raycastOrigin.position + (targetDirection * targetDistance) - impactOffset;

        return impactPoint;
    }

    private void SpawnHitVFX(Vector3 position)
    {
        Instantiate(_vfxImpactPrefab, position, Quaternion.identity);
    }
}
