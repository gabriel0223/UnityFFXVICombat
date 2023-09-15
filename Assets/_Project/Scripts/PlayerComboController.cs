using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class PlayerComboController : MonoBehaviour
{
    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private StarterAssetsInputs _input;
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private DashController _dashController;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dashDistanceBetweenHits;
    [SerializeField] private float _maxDashDistance;
    [SerializeField] private float _dashStoppingDistance;
    [SerializeField] private float _distanceToEnemyMultiplier;

    private bool _isInAttackAnimation;
    private bool _isCheckingForAttack;
    private int _currentCombo;

    private readonly string[] _attackList = new[] { "Slash1", "Slash2", "Slash3", "Slash4", "Slash5" };
    private Queue<string> _attackQueue;
    private Vector3 _inputDirection;

    public void OnAttack()
    {
        bool isInLastAttack = _isCheckingForAttack && _attackQueue.Count == 0;

        if (!_isInAttackAnimation || isInLastAttack)
        {
            StartCombo();
        }
        else
        {
            if (!_isCheckingForAttack)
            {
                return;
            }

            ExecuteNextAttack();
        }
    }

    public void ComboDashForward()
    {
        if (_combatController.IsOnCombatMode)
        {
            DashToCurrentTarget();
        }
        else
        {
            DashForward();
        }
    }

    private void StartCombo()
    {
        _attackQueue = new Queue<string>(_attackList);

        ExecuteNextAttack();
    }

    private void DashToCurrentTarget()
    {
        EnemyHealth currentTarget = _combatController.CurrentTarget;
        float distanceToEnemy = Vector3.Distance(currentTarget.transform.position, transform.position);
        Vector3 enemyDirection = (currentTarget.transform.position - transform.position).normalized;
        float dashDistance = distanceToEnemy * _distanceToEnemyMultiplier;

        dashDistance = Mathf.Clamp(dashDistance, 0, _maxDashDistance);

        if (distanceToEnemy < _dashStoppingDistance)
        {
            dashDistance = 0;
        }

        Vector3 dashDirection = enemyDirection * dashDistance;
        _dashController.DashTowardsDirection(dashDirection, 0.3f, true);
    }

    private void DashForward()
    {
        _dashController.DashTowardsInput(_dashDistanceBetweenHits, 0.3f);
    }

    private void ExecuteNextAttack()
    {
        if (_playerStateManager.PlayerState == PlayerState.TakingDamage ||
            _playerStateManager.PlayerState == PlayerState.Shifting)
        {
            return;
        }

        _isCheckingForAttack = false;

        if (_attackQueue.Count != 0)
        {
            _animator.SetTrigger(_attackQueue.Dequeue());
        }
    }

    public void OnAttackAnimationStart()
    {
        _isInAttackAnimation = true;
        _playerStateManager.SetPlayerState(PlayerState.Attacking);
    }

    public void OnAttackAnimationEnd()
    {
        //if it's in the middle of the combo, return
        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slash"))
        {
            return;
        }

        _isInAttackAnimation = false;

        if (_playerStateManager.PlayerState == PlayerState.TakingDamage ||
            _playerStateManager.PlayerState == PlayerState.Shifting)
        {
            return;
        }

        _playerStateManager.SetPlayerState(PlayerState.Idle);
    }

    public void StartCheckingForAttack()
    {
        _isCheckingForAttack = true;
    }

    public void EndCheckingForAttack()
    {
        _isCheckingForAttack = false;
    }
}
