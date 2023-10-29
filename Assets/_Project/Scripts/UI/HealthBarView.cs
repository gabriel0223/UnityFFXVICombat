using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of health bars.
/// </summary>
public class HealthBarView : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentHpText;
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _damageBar;
    [SerializeField] private HealthBase _ownerHealth;
    [SerializeField] private float _animationDuration;

    private void OnEnable()
    {
        _ownerHealth.OnTakeDamage += HandleTakeDamage;
    }

    private void OnDisable()
    {
        _ownerHealth.OnTakeDamage -= HandleTakeDamage;
    }

    private void Start()
    {
        if (_currentHpText != null)
        {
            _currentHpText.SetText(_ownerHealth.CurrentHealth.ToString());
        }

        _lifeBar.fillAmount = (float)_ownerHealth.CurrentHealth / _ownerHealth.MaxHealth;
    }

    private void HandleTakeDamage(HealthBase healthOwner)
    {
        Sequence healthBarDamage = DOTween.Sequence();

        float fillAmountToBeLost = _lifeBar.fillAmount - (float)_ownerHealth.CurrentHealth / _ownerHealth.MaxHealth;
        Vector3 damageBarPosition = new Vector2(GetEndPointOfBar(_lifeBar).x, _damageBar.rectTransform.anchoredPosition.y);
        Vector2 damageBarInitialSize = new Vector2(_lifeBar.rectTransform.rect.width * fillAmountToBeLost,
            _damageBar.rectTransform.sizeDelta.y);
        Vector2 damageBarFinalSize = new Vector2(0, _damageBar.rectTransform.sizeDelta.y);

        if (_currentHpText != null)
        {
            healthBarDamage.AppendCallback(() => _currentHpText.SetText(_ownerHealth.CurrentHealth.ToString()));
        }

        //setup damage bar
        healthBarDamage.AppendCallback(() => _damageBar.rectTransform.pivot = new Vector2(1, 0.5f));
        healthBarDamage.AppendCallback(() => _damageBar.rectTransform.sizeDelta = new Vector2(0, 
            _damageBar.rectTransform.sizeDelta.y));
        healthBarDamage.AppendCallback(() => _damageBar.rectTransform.anchoredPosition = damageBarPosition);

        //animate health bar and damage bar
        healthBarDamage.Append(_lifeBar.DOFillAmount((float)_ownerHealth.CurrentHealth / _ownerHealth.MaxHealth, _animationDuration));
        healthBarDamage.Join(_damageBar.rectTransform.DOSizeDelta(damageBarInitialSize, _animationDuration));

        healthBarDamage.AppendCallback(() => _damageBar.rectTransform.SetPivot(new Vector2(0, 0.5f)));
        healthBarDamage.Append(_damageBar.rectTransform.DOSizeDelta(damageBarFinalSize, _animationDuration));
    }

    private Vector3 GetEndPointOfBar(Image fillImage)
    {
        float width = fillImage.rectTransform.rect.width;
        Vector3 endPoint = Vector3.zero;
        
        endPoint.x = -width / 2;
        endPoint.x += width * fillImage.fillAmount;

        return endPoint;
    }
}
