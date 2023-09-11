using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumberView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _timeOnScreen;

    public void Initialize(int damage, Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
        _damageText.SetText(damage.ToString());

        Sequence damageTextAnim = DOTween.Sequence();

        damageTextAnim.AppendCallback(() => _canvasGroup.alpha = 0);
        damageTextAnim.Append(_canvasGroup.DOFade(1f, _fadeDuration));
        damageTextAnim.AppendInterval(_timeOnScreen);
        damageTextAnim.Append(_canvasGroup.DOFade(0f, _fadeDuration));
        damageTextAnim.Join(_canvasGroup.transform.DOMoveY(_canvasGroup.transform.position.y - 1000f, 20));
    }
}
