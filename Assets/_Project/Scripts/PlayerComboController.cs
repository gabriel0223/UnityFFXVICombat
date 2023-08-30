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
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dashDistanceBetweenHits;
    [SerializeField] private float _maxDashDistance;
    [SerializeField] private float _dashStoppingDistance;

    private Camera _mainCamera;
    private bool _isInAttackAnimation;
    private bool _isCheckingForAttack;
    private int _currentCombo;

    private readonly string[] _attackList = new[] { "Slash1", "Slash2", "Slash3", "Slash4", "Slash5" };
    private Queue<string> _attackQueue;
    private bool _isDashing;
    private float _dashSpeed;
    private float _dashDistance;
    private Vector3 _dashDirection;
    private Vector3 _inputDirection;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!_isDashing)
        {
            return;
        }

        _characterController.Move(_dashDirection * _dashSpeed * Time.deltaTime);
    }

    public void OnAttack()
    {
        if (!_isInAttackAnimation)
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
        StartCoroutine(ComboDashCoroutine());
    }

    private void StartCombo()
    {
        _attackQueue = new Queue<string>(_attackList);

        ExecuteNextAttack();
    }

    private IEnumerator ComboDashCoroutine()
    {
        _isDashing = true;

        if (_combatController.IsOnCombatMode)
        {
            DashToNearestEnemy();
        }
        else
        {
            DashForward();
        }

        Sequence dashSequence = DOTween.Sequence();
        dashSequence.Append(DOVirtual.Float(0f, 1f, 0.15f, value => _dashSpeed = value).SetEase(Ease.Unset));
        dashSequence.Append(DOVirtual.Float(1f, 0f, 0.15f, value => _dashSpeed = value).SetEase(Ease.Unset));

        yield return new WaitForSeconds(0.3f);

        _isDashing = false;
    }

    private void DashToNearestEnemy()
    {
        EnemyController nearestEnemy = _combatController.GetNearestEnemy();
        float distanceToEnemy = Vector3.Distance(nearestEnemy.transform.position, transform.position);
        Vector3 enemyDirection = nearestEnemy.transform.position - transform.position; 

        _dashDistance = _maxDashDistance;
        _dashDistance = Mathf.Clamp(_dashDistance, 0, _maxDashDistance);

        if (distanceToEnemy < _dashStoppingDistance)
        {
            _dashDistance = 0;
        }

        _dashDirection = enemyDirection * _dashDistance;

        Quaternion targetRotation = Quaternion.LookRotation(enemyDirection);
        transform.DORotateQuaternion(targetRotation, 0.25f);
    }

    private void DashForward()
    {
        _inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        float inputDirectionAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg +
                                    _mainCamera.transform.eulerAngles.y;

        Vector3 targetDirection = _inputDirection.magnitude == 0? transform.forward :
            Quaternion.Euler(0.0f, inputDirectionAngle, 0.0f) * Vector3.forward;

        _dashDistance = _dashDistanceBetweenHits;
        _dashDirection = _input.move == Vector2.zero ? transform.forward * _dashDistance
            : targetDirection * _dashDistance;

        Quaternion targetRotation = Quaternion.LookRotation(_dashDirection);
        transform.DORotateQuaternion(targetRotation, 0.25f);
    }

    private void ExecuteNextAttack()
    {
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
