using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

/// <summary>
/// Controls everything related to the combo system
/// and how the attacks link to each other.
/// </summary>
public class PlayerComboController : MonoBehaviour
{
    public event Action OnComboEnd;
    public event Action OnFullComboComplete; 

    [SerializeField] private WeaponController _playerWeapon;
    [SerializeField] private AttackData[] _attackList;
    [Tooltip("Distance the player dashes forward each time they attack")]
    [SerializeField] private float _dashDistanceBetweenHits;
    [SerializeField] private float _maxDashDistance;
    [Tooltip("Distance from the enemy in which the player won't dash forward anymore")]
    [SerializeField] private float _dashStoppingDistance;
    [SerializeField] private float _distanceToEnemyMultiplier;

    private PlayerCombatController _combatController;
    private DashController _dashController;
    private Animator _animator;

    private bool _isInAttackAnimation;
    private bool _isCheckingForAttackInput;
    private int _currentCombo;
    private Queue<AttackData> _attackQueue;
    private Vector3 _inputDirection;

    private void Awake()
    {
        _combatController = GetComponent<PlayerCombatController>();
        _dashController = GetComponent<DashController>();
        _animator = GetComponent<Animator>();
    }

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
        bool isInLastAttack = _isCheckingForAttackInput && _attackQueue.Count == 0;
        
        if (!_isInAttackAnimation || isInLastAttack)
        {
            StartCombo();
        }
        else
        {
            if (!_isCheckingForAttackInput)
            {
                return;
            }

            ExecuteNextAttack();
        }
    }

    /// <summary>
    /// Performs an attack and dashes forward.
    /// </summary>
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
    
        OnComboEnd?.Invoke();
        Reset();
    }

    /// <summary>
    /// Start checking for attack input to trigger next
    /// attack in the sequence.
    /// </summary>
    public void StartCheckingForAttack()
    {
        _isCheckingForAttackInput = true;
    }

    /// <summary>
    /// Stop checking for attack input to trigger next
    /// attack in the sequence.
    /// </summary>
    public void EndCheckingForAttack()
    {
        //if there's no attack buffered
        if (_isCheckingForAttackInput)
        {
            _isInAttackAnimation = false;
            
            OnComboEnd?.Invoke();
        }

        _isCheckingForAttackInput = false;
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
        _isCheckingForAttackInput = false;

        if (_attackQueue.Count != 0)
        {
            AttackData attackData = _attackQueue.Dequeue();

            _playerWeapon.SetAttackData(attackData);
            _animator.SetTrigger(attackData.AnimationName);
        }
    }

    /// <summary>
    /// Reset combo values to initial values.
    /// </summary>
    private void Reset()
    {
        _attackQueue = new Queue<AttackData>(_attackList);
        _isCheckingForAttackInput = false;
        _isInAttackAnimation = false;
    }
}
