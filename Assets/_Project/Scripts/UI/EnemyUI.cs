using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Controls everything related to the UI of an enemy.
/// </summary>
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private ParticleSystem _healthSparksPs;
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private CanvasGroup _targetMarker;
    [SerializeField] private CanvasGroup _enemyText;
    [SerializeField] private CanvasGroup _healthBar;

    private void Awake()
    {
        _targetMarker.alpha = 0;
        _enemyText.alpha = 0;
    }

    private void OnEnable()
    {
        _enemyHealth.OnTargeted += HandleTargeted;
        _enemyHealth.OnUntargeted += HandleUntargeted;
        _enemyHealth.OnDie += HandleDie;
    }

    private void OnDisable()
    {
        _enemyHealth.OnTargeted -= HandleTargeted;
        _enemyHealth.OnUntargeted -= HandleUntargeted;
        _enemyHealth.OnDie += HandleDie;
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

    private void HandleDie(HealthBase enemyHealth)
    {
        Sequence deathUiSequence = DOTween.Sequence();

        deathUiSequence.AppendInterval(0.5f);
        deathUiSequence.AppendCallback(() => _healthSparksPs.Play());
        deathUiSequence.AppendCallback(() => _healthSparksPs.transform.DOScale(Vector3.one * 1.75f, 0.2f));
        deathUiSequence.Join(_healthBar.DOFade(0f, 0.5f));
    }
}
