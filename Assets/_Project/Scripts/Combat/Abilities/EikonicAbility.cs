using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Base class from which all abilities derive.
/// </summary>
public class EikonicAbility : MonoBehaviour
{
    public event Action OnAnimationFinished;

    [SerializeField] protected AbilityData _abilityData;
    [SerializeField] protected PlayableDirector _playableDirector;

    protected bool _isFinished;
    protected Transform _playerTransform;
    protected Animator _playerAnimator;

    public AbilityData AbilityData => _abilityData;
    public float CurrentCooldown { get; protected set; }

    public virtual void Initialize(AbilityManager ctx)
    {
        _playerTransform = ctx.transform;
        _playerAnimator = ctx.gameObject.GetComponent<Animator>();
    }

    public virtual void Activate()
    {
        _isFinished = false;
        transform.position = _playerTransform.position;
        transform.rotation = _playerTransform.rotation;
        CurrentCooldown = AbilityData.Cooldown;

        _playerAnimator.applyRootMotion = true;
        _playableDirector.Play();
    }

    protected virtual void End()
    {
        _isFinished = true;
    }

    protected virtual void HandleAbilityAnimationFinished(PlayableDirector obj)
    {
        OnAnimationFinished?.Invoke();
    }

    private void Update()
    {
        CurrentCooldown -= Time.deltaTime;
    }
}
