using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using StarterAssets;
using Unity.Mathematics;
using UnityEngine;

public class PhoenixShift : MonoBehaviour
{
    public event Action OnShiftEnd;
    public event Action OnShiftHitEnemy;

    private const float CameraDistortionAmount = 1.25f;

    [SerializeField] private Material _projectionMaterial;
    [SerializeField] private DistortionSphere _distortionSphere;
    [SerializeField] private InputManager _input;
    [SerializeField] private PlayerVFX _playerVFX;
    [SerializeField] private CinemachineImpulseSource _cameraImpulseSource;
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private WeaponController _playerWeapon;
    [SerializeField] private DashController _dashController;
    [SerializeField] private Animator _animator;
    [Space] 
    [Header("SETTINGS")] 
    [SerializeField] private AttackData _attackData;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _range;
    [SerializeField] private float _distanceToEnemyMultiplier;

    private Camera _mainCamera;
    private Vector3 _inputDirection;
    private Renderer[] _meshes;
    private SkinnedMeshRenderer[] _skinnedMeshes;
    private Vector3 _shiftDirection;
    private Material _materialInstance;
    private bool _isExecutingShift;

    public bool IsWaitingForAttack { get; private set; }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _meshes = GetComponentsInChildren<Renderer>();
        _skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        _materialInstance = new Material(_skinnedMeshes[0].material);

        foreach (SkinnedMeshRenderer skinnedMesh in _skinnedMeshes)
        {
            skinnedMesh.material = _materialInstance;
        }
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
        _animator.SetTrigger("PhoenixShift");
        _playerVFX.SetFireVfxSlashActive(true);
        _playerVFX.SpawnBeginPhoenixShiftVfx(_shiftDirection);
        _playerWeapon.SetAttackData(_attackData);
        _isExecutingShift = true;

        AnimatePhoenixGlow(0, 10 ,0.2f);
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
        DisableMeshes();
        _cameraImpulseSource.GenerateImpulse();

        yield return new WaitForSeconds(0.15f);

        CalculateShiftDirection();
        _dashController.DashTowardsDirection(_shiftDirection, _dashDuration);

        yield return new WaitForSeconds(_dashDuration);

        EndShift();
    }

    private void DisableMeshes()
    {
        SpawnProjection();

        foreach (Renderer mesh in _meshes)
        {
            mesh.enabled = false;
        }

        _animator.speed = 0;
        DOVirtual.DelayedCall(_dashDuration, EnableMeshes);
    }

    private void EnableMeshes()
    {
        _animator.speed = 1;

        foreach (Renderer mesh in _meshes)
        {
            mesh.enabled = true;
        }
    }

    private void EndShift()
    {
        _cameraImpulseSource.GenerateImpulse();
        _playerVFX.SpawnEndPhoenixShiftVfx();

        AnimatePhoenixGlow(10, -10, 0.6f).OnComplete(() => OnShiftEnd?.Invoke());

        Sequence endShiftSequence = DOTween.Sequence();
        endShiftSequence.Append(_distortionSphere.AnimateDistortion(0f, CameraDistortionAmount, 0.1f));
        endShiftSequence.Append(_distortionSphere.AnimateDistortion(CameraDistortionAmount, 0, 0.1f));
        endShiftSequence.AppendCallback(() => _playerVFX.SetFireVfxSlashActive(false));
        endShiftSequence.AppendCallback(() => _isExecutingShift = false);
    }

    private Tween AnimatePhoenixGlow(float startValue, float endValue, float duration)
    {
        Color currentColor = _materialInstance.GetColor("_OutlineColor");

        return DOVirtual.Float(startValue, endValue, duration,
            value => _materialInstance.SetColor(
                "_OutlineColor", currentColor * Mathf.Pow(2, value)));
    }

    private void SpawnProjection()
    {
        GameObject projection = Instantiate(_playerMesh, _playerMesh.transform.position, _playerMesh.transform.rotation);
        projection.transform.parent = null;

        Renderer[] projectionMeshes = projection.GetComponentsInChildren<Renderer>();

        foreach (Renderer mesh in projectionMeshes)
        {
            mesh.material = _projectionMaterial;
        }

        projection.AddComponent<ProjectionController>();
    }
}
