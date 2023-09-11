using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private CanvasGroup _targetMarker;
    [SerializeField] private CanvasGroup _enemyText;

    private void Awake()
    {
        _targetMarker.alpha = 0;
        _enemyText.alpha = 0;
    }

    private void OnEnable()
    {
        _enemyHealth.OnTargeted += HandleTargeted;
        _enemyHealth.OnUntargeted += HandleUntargeted;
    }

    private void OnDisable()
    {
        _enemyHealth.OnTargeted -= HandleTargeted;
        _enemyHealth.OnUntargeted -= HandleUntargeted;
    }

    private void HandleTargeted()
    {
        _targetMarker.DOKill();
        _enemyText.DOKill();

        _targetMarker.DOFade(1f, 0.2f);
        _enemyText.DOFade(1f, 0.2f);
    }

    private void HandleUntargeted()
    {
        _targetMarker.DOKill();
        _enemyText.DOKill();

        _targetMarker.DOFade(0f, 0.2f);
        _enemyText.DOFade(0f, 0.2f);
    }
}
