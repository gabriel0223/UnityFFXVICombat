using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class EnemyVFX : MonoBehaviour
{
    private const string SkinnedMeshName = "EnemyMesh";

    [SerializeField] private SkinnedMeshRenderer _skinnedMesh;
    [SerializeField] private Transform _hipsTransform;
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private VisualEffect _onFirePrefab;

    private void OnEnable()
    {
        _enemyHealth.OnGetFireDamage += HandleFireDamage;
    }

    private void OnDisable()
    {
        _enemyHealth.OnGetFireDamage -= HandleFireDamage;
    }

    private void HandleFireDamage()
    {
        VisualEffect visualEffect = Instantiate(_onFirePrefab, transform.position, Quaternion.identity);

        visualEffect.SetSkinnedMeshRenderer(SkinnedMeshName, _skinnedMesh);
        MyVFXTransformBinder transformBinder = visualEffect.gameObject.AddComponent<MyVFXTransformBinder>();
        transformBinder.Target = _hipsTransform;
    }
}
