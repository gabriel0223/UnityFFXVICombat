using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private PlayerWeapon _playerWeapon;

    private List<EnemyController> _enemiesDetectedList = new List<EnemyController>();

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

    public EnemyController GetNearestEnemy()
    {
        //implement later
        return _enemiesDetectedList[0];
    }

    private void AddEnemyToList(EnemyController enemy)
    {
        if (_enemiesDetectedList.Contains(enemy))
        {
            return;
        }

        _enemiesDetectedList.Add(enemy);
        enemy.OnDie += RemoveEnemyFromList;

        IsOnCombatMode = true;
    }

    private void RemoveEnemyFromList(EnemyController enemy)
    {
        _enemiesDetectedList.Remove(enemy);
        enemy.OnDie -= RemoveEnemyFromList;

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
