using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public event Action<EnemyController> OnEnemyDetected;
    public event Action<EnemyController> OnEnemyLeaveDetection;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out EnemyController enemy))
        {
            return;
        }

        OnEnemyDetected?.Invoke(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out EnemyController enemy))
        {
            return;
        }

        OnEnemyLeaveDetection?.Invoke(enemy);
    }
}
