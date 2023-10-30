using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

public class DodgeController : MonoBehaviour
{
    public event Action OnDodgeEnd;
    public event Action OnPrecisionDodge;
    public event Action OnPrecisionCounter;

    [SerializeField] private WeaponController _weaponController;

    [Header("DODGE SETTINGS")]
    [SerializeField] private float _dodgeDistance;
    [SerializeField] private float _dodgeDuration;
    [Tooltip("Speed in which the player character rotates to face the enemy during the dodge")]
    [SerializeField] private float _rotationSpeed;
    [Tooltip("Radius in which dodgeable should be for it be considered a perfect dodge")]
    [SerializeField] private float _perfectDodgeRadius;
    [SerializeField] private LayerMask _dodgeableLayer;
    [Tooltip("For how long the player will be invulnerable after pressing the dodge button")]
    [SerializeField] private float _invulnerabilityTimer;

    [Header("COUNTER SETTINGS")]
    [SerializeField] private float _counterDashRange;
    [SerializeField] private float _counterDashDuration;
    [Tooltip("The color of the glowy outline that appears in the player while using this ability")]
    [SerializeField] private Color _counterOutlineColor;
    [SerializeField] private GameObject _counterImpactVfx;
    [SerializeField] private AttackData _counterAttackData;

    private DashController _dashController;
    private PlayerCombatController _combatController;
    private PlayerHealth _playerHealth;
    private InputManager _inputManager;
    private PlayerVFX _playerVfx;
    private Animator _animator;

    private EnemyHealth _dodgedAttacker;
    private bool _isCheckingForDodge = true;
    private bool _isNewDodgeBuffered;
    private bool _isPlayingDodgeAnimation;
    private bool _isPrecisionDodge;
    private bool _isPrecisionCounterBuffered;
    private bool _isCheckingForCounter;

    private void Awake()
    {
        _dashController = GetComponent<DashController>();
        _combatController = GetComponent<PlayerCombatController>();
        _playerHealth = GetComponent<PlayerHealth>();
        _inputManager = GetComponent<InputManager>();
        _playerVfx = GetComponent<PlayerVFX>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _inputManager.OnAttackPressed += HandleAttackPressed;
        _playerHealth.OnTakeDamage += HandleTakeDamage;
        WeaponController.OnWeaponHitHealth += HandleWeaponHitHealth;
    }

    private void OnDisable()
    {
        _inputManager.OnAttackPressed -= HandleAttackPressed;
        _playerHealth.OnTakeDamage -= HandleTakeDamage;
        WeaponController.OnWeaponHitHealth -= HandleWeaponHitHealth;
    }

    private void Update()
    {
        if (!_isPlayingDodgeAnimation || _combatController.CurrentTarget == null)
        {
            return;
        }

        RotateTowardsDodgedAttacker();
    }

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

        _animator.SetTrigger(AnimationParameters.Dodge);
        _animator.SetFloat("DodgeX", 0);
        _animator.SetFloat("DodgeY", _inputManager.move.magnitude == 0? 0 : 1);

        _isCheckingForDodge = false;
        _isNewDodgeBuffered = false;
        _isCheckingForCounter = false;
        _isPlayingDodgeAnimation = true;
        _isPrecisionCounterBuffered = false;
        _isPrecisionDodge = CanPrecisionDodge();

        _playerHealth.StartInvulnerabilityTimer(_invulnerabilityTimer);
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
        if (_isPrecisionCounterBuffered)
        {
            return;
        }

        _isPlayingDodgeAnimation = false;

        if (_isNewDodgeBuffered)
        {
            TriggerDodge();
            return;
        }

        OnDodgeEnd?.Invoke();
    }

    public void OnCounterAnimationEnd()
    {
        _isPlayingDodgeAnimation = false;
        _playerVfx.AnimateCharacterOutlineIntensity(_counterOutlineColor, 10, -10, 0.5f);
        _playerHealth.SetInvulnerability(false);

        OnDodgeEnd?.Invoke();
    }

    public void StartCheckingForCounter()
    {
        _isCheckingForCounter = true;
    }

    public void ExecutePrecisionCounter()
    {
        Vector3 attackerDirection = (_dodgedAttacker.gameObject.transform.position - transform.position).normalized;

        _dashController.DashTowardsDirection(attackerDirection * _counterDashRange, _counterDashDuration, true);
        _playerVfx.AnimateCharacterOutlineIntensity(_counterOutlineColor, 0, 10, 0.1f);
    }

    private void RotateTowardsDodgedAttacker()
    {
        EnemyHealth currentTarget = _combatController.CurrentTarget;
        Vector3 targetDirection = (currentTarget.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private bool CanPrecisionDodge()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 1f, 0f), 
            _perfectDodgeRadius, _dodgeableLayer);

        if (colliders.Length == 0)
        {
            return false;
        }

        IDodgeable[] dodgeables = colliders.Select(c => c.gameObject.GetComponent<IDodgeable>()).ToArray();
        IDodgeable dodgeable = dodgeables.FirstOrDefault(d => d.IsInDodgeWindow);

        if (dodgeable == null)
        {
            return false;
        }

        if (dodgeable.gameObject.transform.root.TryGetComponent(out EnemyHealth enemyHealth))
        {
            _dodgedAttacker = enemyHealth;
        }

        return true;
    }

    private void HandleAttackPressed()
    {
        if (!_isPrecisionDodge || _dodgedAttacker == null || !_isCheckingForCounter)
        {
            return;
        }

        TriggerPrecisionCounter();
    }

    private void TriggerPrecisionCounter()
    {
        _animator.SetTrigger(AnimationParameters.PrecisionCounter);
        _weaponController.SetAttackData(_counterAttackData);
        _playerHealth.SetInvulnerability(true);

        _isPrecisionCounterBuffered = true;
        _isCheckingForCounter = false;
    }

    private void HandleWeaponHitHealth(HealthBase health, int damage)
    {
        if (!_isPlayingDodgeAnimation || !_isPrecisionCounterBuffered)
        {
            return;
        }

        Collider enemyHitCollider = health.gameObject.GetComponent<Collider>();
        Vector3 impactPoint = _weaponController.GetImpactPoint(enemyHitCollider);

        Instantiate(_counterImpactVfx, impactPoint, Quaternion.identity);

        _isPrecisionCounterBuffered = false;

        OnPrecisionCounter?.Invoke();
    }

    private void HandleTakeDamage(HealthBase health)
    {
        _isCheckingForDodge = true;
        _isNewDodgeBuffered = false;
        _isCheckingForCounter = false;
        _isPlayingDodgeAnimation = false;
        _isPrecisionCounterBuffered = false;
    }
}
