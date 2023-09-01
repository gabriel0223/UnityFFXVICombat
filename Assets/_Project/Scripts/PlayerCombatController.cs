using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs _input;
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private PlayerWeapon _playerWeapon;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _maxDetectionDistance;

    private List<EnemyController> _enemiesDetectedList = new List<EnemyController>();

    public bool IsOnCombatMode { get; private set; }
    public EnemyController CurrentTarget { get; private set; }

    private void OnEnable()
    {
        _enemyDetector.OnEnemyDetected += AddEnemyToList;
        _enemyDetector.OnEnemyLeaveDetection += RemoveEnemyFromList;
    }

    private void OnDisable()
    {
        _enemyDetector.OnEnemyDetected += AddEnemyToList;
        _enemyDetector.OnEnemyLeaveDetection += RemoveEnemyFromList;
    }

    private void Update()
    {
        if (!IsOnCombatMode)
        {
            CurrentTarget = null;
            return;
        }

        LookForTargets();
    }

    private void LookForTargets()
    {
        if (_enemiesDetectedList.Count == 1)
        {
            CurrentTarget = _enemiesDetectedList[0];
            return;
        }
        
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (Physics.SphereCast(transform.position + new Vector3(0f, 1f, 0f), 2f, inputDirection,
                out RaycastHit hitInfo, _maxDetectionDistance, _enemyLayer))
        {
            EnemyController enemy = hitInfo.collider.gameObject.GetComponent<EnemyController>();

            CurrentTarget = enemy;
        }
    }

    private void AddEnemyToList(EnemyController enemy)
    {
        if (_enemiesDetectedList.Contains(enemy))
        {
            return;
        }

        _enemiesDetectedList.Add(enemy);
        enemy.OnDie += HandleEnemyDie;

        IsOnCombatMode = true;
    }

    private void RemoveEnemyFromList(EnemyController enemy)
    {
        _enemiesDetectedList.Remove(enemy);
        enemy.OnDie -= HandleEnemyDie;

        if (_enemiesDetectedList.Count == 0)
        {
            IsOnCombatMode = false;
        }
    }

    private void HandleEnemyDie(EnemyController enemy)
    {
        DOVirtual.DelayedCall(1f, () => RemoveEnemyFromList(enemy));
    }

    public void EnableWeaponCollider()
    {
        _playerWeapon.SetColliderActive(true);
    }

    public void DisableWeaponCollider()
    {
        _playerWeapon.SetColliderActive(false);
    }

    private void OnDrawGizmos()
    {
        if (CurrentTarget == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CurrentTarget.transform.position, 0.5f);
        Gizmos.DrawLine(transform.position + new Vector3(0, 0.25f, 0), CurrentTarget.transform.position + new Vector3(0, 0.25f, 0));

        Gizmos.color = Color.blue;

        Vector3 startPosition = transform.position + new Vector3(0, 0.25f, 0);
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        Gizmos.DrawLine(startPosition, startPosition + inputDirection * 10);
    }
}
