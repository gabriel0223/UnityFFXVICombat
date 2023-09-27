using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class DodgeController : MonoBehaviour
{
    public event Action OnDodgeEnd;
    public event Action OnPrecisionDodge;

    [SerializeField] private DashController _dashController;
    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dodgeDistance;
    [SerializeField] private float _dodgeDuration;
    [Tooltip("Speed in which the player character rotates to face the enemy during the dodge")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _dodgeRadius;
    [SerializeField] private LayerMask _dodgeableLayer;
    [Tooltip("How long will the player be invulnerable after pressing the dodge button")]
    [SerializeField] private float _invulnerabilityTimer;

    private bool _isCheckingForDodge = true;
    private bool _isNewDodgeBuffered;
    private bool _isPlayingDodgeAnimation;
    private bool _isPrecisionDodge;

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
        _isPrecisionDodge = CanPrecisionDodge();

        _playerHealth.EnableInvulnerability(_invulnerabilityTimer);
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

            if (_isPrecisionDodge)
            {
                OnPrecisionDodge?.Invoke();
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

    private bool CanPrecisionDodge()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 1f, 0f), 
            _dodgeRadius, _dodgeableLayer);

        if (colliders.Length == 0)
        {
            return false;
        }

        IDodgeable[] dodgeables = colliders.Select(c => c.gameObject.GetComponent<IDodgeable>()).ToArray();

        return dodgeables.Any(dodgeable => dodgeable.IsInDodgeWindow);
    }
}
