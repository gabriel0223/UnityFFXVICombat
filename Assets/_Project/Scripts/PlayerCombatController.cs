using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private PlayerWeapon _playerWeapon;

    private List<EnemyController> _enemiesDetectedList;

    public bool IsOnCombatMode { get; private set; }

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

    private void AddEnemyToList(EnemyController enemy)
    {
        if (_enemiesDetectedList.Contains(enemy))
        {
            return;
        }

        _enemiesDetectedList.Add(enemy);

        IsOnCombatMode = true;
    }

    private void RemoveEnemyFromList(EnemyController enemy)
    {
        _enemiesDetectedList.Remove(enemy);

        if (_enemiesDetectedList.Count == 0)
        {
            IsOnCombatMode = false;
        }
    }

    public void EnableWeaponCollider()
    {
        _playerWeapon.SetColliderActive(true);
    }

    public void DisableWeaponCollider()
    {
        _playerWeapon.SetColliderActive(false);
    }
}
