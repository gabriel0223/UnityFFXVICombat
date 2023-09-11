using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public event Action<EnemyHealth> OnEnemyDetected;
    public event Action<EnemyHealth> OnEnemyLeaveDetection;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out EnemyHealth enemy))
        {
            return;
        }

        OnEnemyDetected?.Invoke(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out EnemyHealth enemy))
        {
            return;
        }

        OnEnemyLeaveDetection?.Invoke(enemy);
    }
}
