using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentHpText;
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _damageBarImage;
    [SerializeField] private PlayerHealth _health;

    private void OnEnable()
    {
        _health.OnTakeDamage += HandleTakeDamage;
    }

    private void OnDisable()
    {
        _health.OnTakeDamage -= HandleTakeDamage;
    }

    private void Start()
    {
        _currentHpText.SetText(_health.CurrentHealth.ToString());
        _lifeBar.fillAmount = (float)_health.CurrentHealth / _health.MaxHealth;
    }

    private void HandleTakeDamage()
    {
        Sequence lifeBarAnim = DOTween.Sequence();

        float fillAmountToBeLost = _lifeBar.fillAmount - (float)_health.CurrentHealth / _health.MaxHealth;
        Vector3 damageBarPosition = new Vector2(GetEndPointOfFill(_lifeBar).x, _damageBarImage.rectTransform.position.y);
        Vector2 damageBarInitialSize = new Vector2(_lifeBar.rectTransform.rect.width * fillAmountToBeLost,
            _damageBarImage.rectTransform.sizeDelta.y);
        Vector2 damageBarFinalSize = new Vector2(0, _damageBarImage.rectTransform.sizeDelta.y);

        lifeBarAnim.AppendCallback(() => _currentHpText.SetText(_health.CurrentHealth.ToString()));
        lifeBarAnim.AppendCallback(() => _damageBarImage.rectTransform.pivot = new Vector2(1, 0.5f));
        lifeBarAnim.AppendCallback(() => _damageBarImage.rectTransform.sizeDelta = new Vector2(0, 
            _damageBarImage.rectTransform.sizeDelta.y));
        lifeBarAnim.AppendCallback(() => _damageBarImage.rectTransform.position = damageBarPosition);


        lifeBarAnim.Append(_lifeBar.DOFillAmount((float)_health.CurrentHealth / _health.MaxHealth, 0.25f));
        lifeBarAnim.Join(_damageBarImage.rectTransform.DOSizeDelta(damageBarInitialSize, 0.25f));

        lifeBarAnim.AppendCallback(() => _damageBarImage.rectTransform.SetPivot(new Vector2(0, 0.5f)));
        lifeBarAnim.Append(_damageBarImage.rectTransform.DOSizeDelta(damageBarFinalSize, 0.25f));
    }

    private Vector3 GetEndPointOfFill(Image fillImage)
    {
        RectTransform rectTransform = fillImage.rectTransform;
        
        // Width of the health bar in local space
        float totalWidth = rectTransform.rect.width;

        // Calculate end point in local space of the Image's RectTransform
        float filledWidth = totalWidth * fillImage.fillAmount;
        Vector3 endPointLocal = new Vector3(-totalWidth * 0.5f + filledWidth, 0, 0);

        // If you want the endpoint in world space
        Vector3 endPointWorld = rectTransform.TransformPoint(endPointLocal);

        return endPointWorld;
    }
}
