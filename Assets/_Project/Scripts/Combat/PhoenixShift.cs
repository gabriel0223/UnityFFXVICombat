using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using StarterAssets;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles "Phoenix Shift" ability, a powerful fire teleport attack.
/// </summary>
public class PhoenixShift : MonoBehaviour
{
    public event Action OnShiftStart;
    public event Action OnShiftEnd;
    public event Action OnShiftAttack;
    public event Action OnShiftHitEnemy;

    private const float CameraDistortionAmount = 1.25f;

    [SerializeField] private DistortionSphere _distortionSphere;
    [SerializeField] private WeaponController _playerWeapon;
    [Header("SETTINGS")]
    [SerializeField] private AttackData _attackData;
    [SerializeField] private float _dashDuration;
    [Tooltip("The distance the dash is going to cover")]
    [SerializeField] private float _range;
    [Tooltip("Multiplier that defines how much the distance to the enemy impact the dash distance")]
    [SerializeField] private float _distanceToEnemyMultiplier;
    [Tooltip("The color of the glowy outline that appears in the player while using this ability")]
    [SerializeField] private Color _outlineColor;

    private InputManager _input;
    private PlayerVFX _playerVFX;
    private PlayerCombatController _combatController;
    private DashController _dashController;
    private Animator _animator;

    private Camera _mainCamera;
    private Vector3 _inputDirection;
    private Vector3 _shiftDirection;
    private bool _isExecutingShift;

    public bool IsWaitingForAttack { get; private set; }

    private void Awake()
    {
        _mainCamera = Camera.main;

        _input = GetComponent<InputManager>();
        _playerVFX = GetComponent<PlayerVFX>();
        _combatController = GetComponent<PlayerCombatController>();
        _dashController = GetComponent<DashController>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        WeaponController.OnWeaponHitHealth += HandleShiftHitEnemy;
    }

    private void OnDisable()
    {
        WeaponController.OnWeaponHitHealth -= HandleShiftHitEnemy;
    }

    public void StartPhoenixShift()
    {
        CalculateShiftDirection();

        _animator.speed = 1.5f;
        _animator.SetTrigger(AnimationParameters.PhoenixShift);
        _playerVFX.SetFireVfxSlashActive(true);
        _playerVFX.SpawnBeginPhoenixShiftVfx(_shiftDirection);
        _playerWeapon.SetAttackData(_attackData);
        _isExecutingShift = true;

        _playerVFX.AnimateCharacterOutlineIntensity(_outlineColor, 0, 10, 0.2f);
    }

    private void HandleShiftHitEnemy(HealthBase health, int damage)
    {
        if (health.gameObject.GetComponent<EnemyHealth>() == null || !_isExecutingShift)
        {
            return;
        }
        
        OnShiftHitEnemy?.Invoke();
    }

    private void CalculateShiftDirection()
    {
        if (_combatController.IsOnCombatMode)
        {
            EnemyHealth currentTarget = _combatController.CurrentTarget;
            Vector3 enemyDirection = (currentTarget.transform.position - transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(transform.position, currentTarget.transform.position);
            float dashDistance = distanceToEnemy * _distanceToEnemyMultiplier;
        
            dashDistance = Mathf.Clamp(dashDistance, 0, _range);
            _shiftDirection = enemyDirection * dashDistance;
        }
        else
        {
            _inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            float inputDirectionAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg +
                                        _mainCamera.transform.eulerAngles.y;

            Vector3 targetDirection = _inputDirection.magnitude == 0? transform.forward :
                Quaternion.Euler(0.0f, inputDirectionAngle, 0.0f) * Vector3.forward;

            _shiftDirection = _input.move == Vector2.zero ? transform.forward * _range
                : targetDirection * _range;
        }

        Quaternion directionRotation = Quaternion.LookRotation(_shiftDirection);
        transform.DORotateQuaternion(directionRotation, 0.1f);
    }

    public void ExecuteShift()
    {
        StartCoroutine(ShiftCoroutine());
    }

    /// <summary>
    /// Stars waiting for attack input to switch to attack state by the
    /// end of the ability.
    /// </summary>
    public void StartWaitingForAttack()
    {
        IsWaitingForAttack = true;
    }

    public void StopWaitingForAttack()
    {
        IsWaitingForAttack = false;
    }

    private IEnumerator ShiftCoroutine()
    {
        OnShiftStart?.Invoke();

        DisableMeshes();

        yield return new WaitForSeconds(0.15f);

        CalculateShiftDirection();
        _dashController.DashTowardsDirection(_shiftDirection, _dashDuration);

        yield return new WaitForSeconds(_dashDuration);

        EndShift();
    }

    private void DisableMeshes()
    {
        _playerVFX.SpawnPhoenixProjection();
        _playerVFX.SetCharacterMeshesEnabled(false);

        _animator.speed = 0;
        DOVirtual.DelayedCall(_dashDuration, EnableMeshes);
    }

    private void EnableMeshes()
    {
        _animator.speed = 1;

        _playerVFX.SetCharacterMeshesEnabled(true);
    }

    private void EndShift()
    {
        OnShiftAttack?.Invoke();
        _playerVFX.SpawnEndPhoenixShiftVfx();

        _playerVFX.AnimateCharacterOutlineIntensity(_outlineColor, 10, -10, 0.6f).
            OnComplete(() => OnShiftEnd?.Invoke());

        Sequence endShiftSequence = DOTween.Sequence();
        endShiftSequence.Append(_distortionSphere.AnimateDistortion(0f, CameraDistortionAmount, 0.1f));
        endShiftSequence.Append(_distortionSphere.AnimateDistortion(CameraDistortionAmount, 0, 0.1f));
        endShiftSequence.AppendCallback(() => _playerVFX.SetFireVfxSlashActive(false));
        endShiftSequence.AppendCallback(() => _isExecutingShift = false);
    }
}
