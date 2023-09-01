using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private CanvasGroup _targetMarker;

    private void Awake()
    {
        _targetMarker.alpha = 0;
    }

    private void OnEnable()
    {
        _enemyController.OnTargeted += HandleTargeted;
        _enemyController.OnUntargeted += HandleUntargeted;
    }

    private void OnDisable()
    {
        _enemyController.OnTargeted -= HandleTargeted;
        _enemyController.OnUntargeted -= HandleUntargeted;
    }

    private void HandleTargeted()
    {
        _targetMarker.DOKill();
        _targetMarker.DOFade(1f, 0.2f);
    }

    private void HandleUntargeted()
    {
        _targetMarker.DOKill();
        _targetMarker.DOFade(0f, 0.2f);
    }
}
