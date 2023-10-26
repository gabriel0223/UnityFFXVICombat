using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

public class EA_InfernalSkyfall : EikonicAbility
{
    [SerializeField] private VisualEffect _abilityVfxPrefab;
    [SerializeField] private float _delayToStartDoingDamage;
    [SerializeField] private int _damage;
    [SerializeField] private float _damageRate;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private Color _playerOutlineColor;

    private VisualEffect _abilityVfxInstance;
    private PlayerVFX _playerVFX;

    public override void Initialize(AbilityManager ctx)
    {
        base.Initialize(ctx);

        _playerVFX = ctx.gameObject.GetComponent<PlayerVFX>();

        _playableDirector.SetGenericBinding(_playableDirector.playableAsset.outputs.First().sourceObject, _playerAnimator);
        _playableDirector.stopped += HandleAbilityAnimationFinished;
    }

    private void OnDestroy()
    {
        _playableDirector.stopped -= HandleAbilityAnimationFinished;
    }

    public override void Activate()
    {
        base.Activate();

        _abilityVfxInstance = Instantiate(_abilityVfxPrefab, transform.position, transform.rotation);
        _playerVFX.AnimateCharacterOutlineIntensity(_playerOutlineColor, 0, 10, 0.2f);

        StartCoroutine(DamageCoroutine());

        DOVirtual.DelayedCall(_abilityData.Duration, End);
    }

    protected override void End()
    {
        base.End();

        _abilityVfxInstance.Stop();

        DOVirtual.DelayedCall(3f, () =>
        {
            Destroy(_abilityVfxInstance.gameObject);
        });
    }

    protected override void HandleAbilityAnimationFinished(PlayableDirector obj)
    {
        base.HandleAbilityAnimationFinished(obj);

        _playerAnimator.applyRootMotion = false;
        _playerVFX.AnimateCharacterOutlineIntensity(_playerOutlineColor, 10, -10, 0.6f);
    }

    private IEnumerator DamageCoroutine()
    {
        yield return new WaitForSeconds(_delayToStartDoingDamage);

        while (!_isFinished)
        {
            Collider[] enemiesOnRadius = Physics.OverlapSphere(transform.position, _radius, _enemyLayer);

            foreach (Collider enemy in enemiesOnRadius)
            {
                enemy.gameObject.GetComponent<IDamageable>().TakeDamage(_damage, DamageType.Fire);
            }

            yield return new WaitForSeconds(_damageRate);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
