using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class EA_InfernalSkyfall : EikonicAbility
{
    [SerializeField] private VisualEffect _abilityVfxPrefab;
    [SerializeField] private float _delayToStartDoingDamage;
    [SerializeField] private int _damage;
    [SerializeField] private float _damageRate;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _enemyLayer;

    private VisualEffect _abilityVfxInstance; 
    
    public override void Activate(Vector3 position)
    {
        _isFinished = false;
        transform.position = position;
        CurrentCooldown = AbilityData.Cooldown;

        _abilityVfxInstance = Instantiate(_abilityVfxPrefab, transform.position, Quaternion.identity);

        StartCoroutine(DamageCoroutine());

        DOVirtual.DelayedCall(_abilityData.Duration, End);
    }

    public override void End()
    {
        _isFinished = true;
        _abilityVfxInstance.Stop();

        DOVirtual.DelayedCall(3f, () =>
        {
            Destroy(_abilityVfxInstance.gameObject);
        });
    }

    private IEnumerator DamageCoroutine()
    {
        yield return new WaitForSeconds(_delayToStartDoingDamage);

        while (!_isFinished)
        {
            Collider[] enemiesOnRadius = Physics.OverlapSphere(transform.position, _radius, _enemyLayer);

            foreach (Collider enemy in enemiesOnRadius)
            {
                enemy.gameObject.GetComponent<IDamageable>().TakeDamage(_damage, DamageType.Normal);
            }

            yield return new WaitForSeconds(_damageRate);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
