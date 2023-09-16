using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PhoenixShift : MonoBehaviour
{
    [SerializeField] private Material _projectionMaterial;
    [SerializeField] private GameObject _beginShiftVfxPrefab;
    [SerializeField] private GameObject _endShiftVfxPrefab;
    [SerializeField] private CinemachineImpulseSource _cameraImpulseSource;
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private PlayerCombatController _combatController; 
    [SerializeField] private DashController _dashController;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _range;
    [SerializeField] private float _distanceToEnemyMultiplier;

    private Renderer[] _meshes;
    private SkinnedMeshRenderer[] _skinnedMeshes;
    private Vector3 _shiftDirection;
    private Material _materialInstance;

    private void Awake()
    {
        _meshes = GetComponentsInChildren<Renderer>();
        _skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        _materialInstance = new Material(_skinnedMeshes[0].material);

        foreach (SkinnedMeshRenderer skinnedMesh in _skinnedMeshes)
        {
            skinnedMesh.material = _materialInstance;
        }
    }

    public void ExecuteShift()
    {
        StartCoroutine(ShiftCoroutine());
    }

    private IEnumerator ShiftCoroutine()
    {
        _playerStateManager.SetPlayerState(PlayerState.Shifting);

        DisableMeshes();
        Instantiate(_beginShiftVfxPrefab, transform.position, quaternion.identity);
        _cameraImpulseSource.GenerateImpulse();

        yield return new WaitForSeconds(0.2f);

        if (!_combatController.IsOnCombatMode)
        {
            _dashController.DashTowardsInput(_range, _dashDuration);
        }
        else
        {
            // EnemyHealth currentTarget = _combatController.CurrentTarget;
            // float distanceToEnemy = Vector3.Distance(currentTarget.transform.position, transform.position);
            // Vector3 enemyDirection = currentTarget.transform.position - transform.position;
            // float dashDistance = _maxDashDistance;
            //
            // dashDistance = Mathf.Clamp(dashDistance, 0, _maxDashDistance);
            //
            // if (distanceToEnemy < _dashStoppingDistance)
            // {
            //     dashDistance = 0;
            // }
            //
            // Vector3 dashDirection = enemyDirection * dashDistance;
            // _dashController.DashTowardsDirection(dashDirection, 0.3f, true);
        
            EnemyHealth currentTarget = _combatController.CurrentTarget;
            float distanceToEnemy = Vector3.Distance(currentTarget.transform.position, transform.position);
            Vector3 enemyDirection = (currentTarget.transform.position - transform.position).normalized;
            float dashDistance = distanceToEnemy * _distanceToEnemyMultiplier;
        
            dashDistance = Mathf.Clamp(dashDistance, 0, _range);
            _dashController.DashTowardsDirection(enemyDirection * dashDistance, _dashDuration, true);
        }

        yield return new WaitForSeconds(_dashDuration);

        OnShiftEnd();
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

    private void OnShiftEnd()
    {
        _cameraImpulseSource.GenerateImpulse();
        Instantiate(_endShiftVfxPrefab, transform.position, quaternion.identity);
        _playerStateManager.SetPlayerState(PlayerState.Idle);

        AnimatePhoenixGlow(10, -10 ,0.5f);
    }

    public void OnPhoenixShift()
    {
        _animator.SetTrigger("PhoenixShift");

        AnimatePhoenixGlow(0, 10 ,0.2f);
    }

    private void AnimatePhoenixGlow(float startValue, float endValue, float duration)
    {
        Color currentColor = _materialInstance.GetColor("_OutlineColor");

        DOVirtual.Float(startValue, endValue, duration,
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
