using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class DodgeController : MonoBehaviour
{
    public event Action OnDodgeEnd;

    [SerializeField] private DashController _dashController;
    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dodgeDistance;
    [SerializeField] private float _dodgeDuration;
    [Tooltip("Speed in which the player character rotates to face the enemy during the dodge")]
    [SerializeField] private float _rotationSpeed;

    private bool _isCheckingForDodge = true;
    private bool _isNewDodgeBuffered;
    private bool _isPlayingDodgeAnimation;

    public void TriggerDodge()
    {
        if (!_isCheckingForDodge)
        {
            return;
        }

        if (_isPlayingDodgeAnimation)
        {
            _isNewDodgeBuffered = true;
            return;
        }

        _animator.SetTrigger("Dodge");

        _animator.SetFloat("DodgeX", 0);
        _animator.SetFloat("DodgeY", _inputManager.move.magnitude == 0? 0 : 1);

        _isCheckingForDodge = false;
        _isNewDodgeBuffered = false;
        _isPlayingDodgeAnimation = true;
    }

    private void Update()
    {
        if (!_isPlayingDodgeAnimation || _combatController.CurrentTarget == null)
        {
            return;
        }

        EnemyHealth currentTarget = _combatController.CurrentTarget;
        Vector3 targetDirection = (currentTarget.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    public void ExecuteDodgeMovement()
    {
        if (_combatController.IsOnCombatMode)
        {
            EnemyHealth currentTarget = _combatController.CurrentTarget;
            Vector3 targetDirection = (currentTarget.transform.position - transform.position).normalized;

            if (_inputManager.move.magnitude == 0)
            {
                _dashController.DashTowardsDirection(-targetDirection * _dodgeDistance, _dodgeDuration);
            }
            else
            {
                _dashController.DashTowardsInput(_dodgeDistance, _dodgeDuration, -transform.forward, false);
            }
        }
        else
        {
            _dashController.DashTowardsInput(_dodgeDistance, _dodgeDuration, -transform.forward, 
                _inputManager.move.magnitude != 0);
        }
    }

    public void StartCheckingForDodge()
    {
        _isCheckingForDodge = true;
    }

    public void OnDodgeAnimationEnd()
    {
        _isPlayingDodgeAnimation = false;

        if (_isNewDodgeBuffered)
        {
            TriggerDodge();
            return;
        }

        OnDodgeEnd?.Invoke();
    }
}
