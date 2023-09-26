using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class PlayerComboController : MonoBehaviour
{
    public event Action OnComboEnd;
    public event Action OnFullComboComplete; 

    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private DashController _dashController;
    [SerializeField] private Animator _animator;
    [SerializeField] private WeaponController _playerWeapon;
    [SerializeField] private AttackData[] _attackList;
    [SerializeField] private float _dashDistanceBetweenHits;
    [SerializeField] private float _maxDashDistance;
    [SerializeField] private float _dashStoppingDistance;
    [SerializeField] private float _distanceToEnemyMultiplier;

    private bool _isInAttackAnimation;
    private bool _isCheckingForAttack;
    private int _currentCombo;
    private Queue<AttackData> _attackQueue;
    private Vector3 _inputDirection;

    private void OnEnable()
    {
        WeaponController.OnWeaponHitHealth += HandleWeaponHitEnemy;
    }

    private void OnDisable()
    {
        WeaponController.OnWeaponHitHealth -= HandleWeaponHitEnemy;
    }

    public void Attack()
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

    private void HandleWeaponHitEnemy(HealthBase health, int damage)
    {
        if (health.gameObject.GetComponent<EnemyHealth>() == null)
        {
            return;
        }

        if (_currentCombo == _attackList.Length)
        {
            OnFullComboComplete?.Invoke();
        }
    }

    private void StartCombo()
    {
        _attackQueue = new Queue<AttackData>(_attackList);
        _currentCombo = 0;

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
        _dashController.DashTowardsInput(_dashDistanceBetweenHits, 0.3f, transform.forward);
    }

    private void ExecuteNextAttack()
    {
        _isCheckingForAttack = false;

        if (_attackQueue.Count != 0)
        {
            AttackData attackData = _attackQueue.Dequeue();

            _playerWeapon.SetAttackData(attackData);
            _animator.SetTrigger(attackData.AnimationName);
        }
    }

    public void OnAttackAnimationStart()
    {
        _isInAttackAnimation = true;
        _currentCombo++;
    }

    public void OnAttackAnimationEnd()
    {
        //if it's in the middle of the combo, return
        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slash"))
        {
            return;
        }

        _isInAttackAnimation = false;
    }

    public void StartCheckingForAttack()
    {
        _isCheckingForAttack = true;
    }

    public void EndCheckingForAttack()
    {
        //if there's no attack buffered
        if (_isCheckingForAttack)
        {
            OnComboEnd?.Invoke();
        }

        _isCheckingForAttack = false;
    }
}
