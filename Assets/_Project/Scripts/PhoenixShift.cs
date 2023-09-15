using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PhoenixShift : MonoBehaviour
{
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

    public void DisableMeshes()
    {
        foreach (Renderer mesh in _meshes)
        {
            mesh.enabled = false;
        }
    }

    public void OnPhoenixShift()
    {
        _animator.SetTrigger("PhoenixShift");

        Color currentColor = _materialInstance.GetColor("_OutlineColor");
        DOVirtual.Float(0, 10, 0.2f,
            value => _materialInstance.SetColor("_OutlineColor", currentColor * Mathf.Pow(2, value)));

        // _playerStateManager.SetPlayerState(PlayerState.Shifting);
        //
        // if (!_combatController.IsOnCombatMode)
        // {
        //     _dashController.DashTowardsInput(_range, _dashDuration);
        // }
        // else
        // {
        //     // EnemyHealth currentTarget = _combatController.CurrentTarget;
        //     // float distanceToEnemy = Vector3.Distance(currentTarget.transform.position, transform.position);
        //     // Vector3 enemyDirection = currentTarget.transform.position - transform.position;
        //     // float dashDistance = _maxDashDistance;
        //     //
        //     // dashDistance = Mathf.Clamp(dashDistance, 0, _maxDashDistance);
        //     //
        //     // if (distanceToEnemy < _dashStoppingDistance)
        //     // {
        //     //     dashDistance = 0;
        //     // }
        //     //
        //     // Vector3 dashDirection = enemyDirection * dashDistance;
        //     // _dashController.DashTowardsDirection(dashDirection, 0.3f, true);
        //
        //     EnemyHealth currentTarget = _combatController.CurrentTarget;
        //     float distanceToEnemy = Vector3.Distance(currentTarget.transform.position, transform.position);
        //     Vector3 enemyDirection = (currentTarget.transform.position - transform.position).normalized;
        //     float dashDistance = distanceToEnemy * _distanceToEnemyMultiplier;
        //
        //     dashDistance = Mathf.Clamp(dashDistance, 0, _range);
        //     _dashController.DashTowardsDirection(enemyDirection * dashDistance, _dashDuration, true);
        // }
        //
        // DOVirtual.DelayedCall(_dashDuration, () => _playerStateManager.SetPlayerState(PlayerState.Idle));
    }
}
