using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public event Action OnEnableWeaponCollider;

    [SerializeField] private InputManager _input;
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private WeaponController _playerWeapon;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _maxDetectionDistance;

    private List<EnemyHealth> _enemiesDetectedList = new List<EnemyHealth>();
    private Camera _mainCamera;

    public bool IsOnCombatMode { get; private set; }
    public EnemyHealth CurrentTarget { get; private set; }

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

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
            SetTarget(_enemiesDetectedList[0]);
            return;
        }

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        float inputDirectionAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                    _mainCamera.transform.eulerAngles.y;

        Vector3 targetDirection = inputDirection.magnitude == 0? transform.forward :
            Quaternion.Euler(0.0f, inputDirectionAngle, 0.0f) * Vector3.forward;


        Collider[] enemiesInFront = Physics.OverlapSphere(
            transform.position + new Vector3(0f, 1f, 0f) + (targetDirection.normalized * 0.5f),
            0.5f, _enemyLayer);

        if (enemiesInFront.Length > 0)
        {
            EnemyHealth enemy = enemiesInFront[0].gameObject.GetComponent<EnemyHealth>();
            SetTarget(enemy);
            
            return;
        }

        if (Physics.SphereCast(transform.position + new Vector3(0f, 1f, 0f), 1.5f, targetDirection,
                out RaycastHit hitInfo, _maxDetectionDistance, _enemyLayer))
        {
            EnemyHealth enemy = hitInfo.collider.gameObject.GetComponent<EnemyHealth>();
            SetTarget(enemy);
        }
    }

    private void SetTarget(EnemyHealth newTarget)
    {
        if (newTarget == null && CurrentTarget != null)
        {
            CurrentTarget.HandleUntargeted();
            CurrentTarget = null;
        }

        if (CurrentTarget == newTarget)
        {
            return;
        }

        if (CurrentTarget != null)
        {
            CurrentTarget.HandleUntargeted();
        }

        CurrentTarget = newTarget;
        CurrentTarget.HandleTargeted();
    }

    private void AddEnemyToList(EnemyHealth enemy)
    {
        if (_enemiesDetectedList.Contains(enemy))
        {
            return;
        }

        _enemiesDetectedList.Add(enemy);
        enemy.OnDie += HandleEnemyDie;

        IsOnCombatMode = true;
    }

    private void RemoveEnemyFromList(EnemyHealth enemy)
    {
        _enemiesDetectedList.Remove(enemy);
        enemy.OnDie -= HandleEnemyDie;

        if (_enemiesDetectedList.Count == 0)
        {
            IsOnCombatMode = false;
        }
    }

    private void HandleEnemyDie(HealthBase enemy)
    {
        EnemyHealth enemyHealth = enemy.gameObject.GetComponent<EnemyHealth>();
        RemoveEnemyFromList(enemyHealth);

        if (enemyHealth != CurrentTarget)
        {
            return;
        }

        if (_enemiesDetectedList.Count == 0)
        {
            SetTarget(null);
            return;
        }

        EnemyHealth closestEnemy = _enemiesDetectedList.OrderByDescending(e => 
            Vector3.Distance(transform.position, e.transform.position)).First();

        SetTarget(closestEnemy);
    }

    public void EnableWeaponCollider()
    {
        _playerWeapon.SetColliderActive(true);

        OnEnableWeaponCollider?.Invoke();
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

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + new Vector3(0f, 1f, 0f) + (inputDirection.normalized * 0.5f), 0.2f);
    }
}
