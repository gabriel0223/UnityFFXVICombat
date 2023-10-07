using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionIndicatorView : MonoBehaviour
{
    [SerializeField] private float _delayToShowText;
    [SerializeField] private float _maskMovementDuration;
    [SerializeField] private float _actionTextMovementDuration;
    [SerializeField] private float _timeOnScreen;

    [SerializeField] private RectTransform _mask;
    [SerializeField] private RectTransform _actionText;
    [SerializeField] private Image _frame;
    [SerializeField] private CanvasGroup[] _stars;

    // Start is called before the first frame update
    void Start()
    {
        Sequence sequence = DOTween.Sequence();

        _actionText.gameObject.SetActive(false);
        _mask.DOAnchorPosX(_mask.anchoredPosition.x + 1000, _maskMovementDuration).SetEase(Ease.Linear);

        sequence.AppendInterval(_delayToShowText);
        sequence.AppendCallback(() => _actionText.gameObject.SetActive(true));
        sequence.Append(_actionText.DOAnchorPosX(_actionText.anchoredPosition.x + 20, _actionTextMovementDuration));
        sequence.Join(_frame.DOFade(0.05f, 0.25f));

        foreach (CanvasGroup star in _stars)
        {
            RectTransform starTransform = star.GetComponent<RectTransform>();

            starTransform.localScale = Vector3.one * 2;
            star.alpha = 0;

            sequence.Append(starTransform.DOScale(Vector3.one, 0.25f));
            sequence.Join(star.DOFade(1f, 0.1f));
        }

        sequence.AppendInterval(_timeOnScreen);
        sequence.Append(_frame.rectTransform.DOAnchorPosX(_frame.rectTransform.anchoredPosition.x + 1000, 0.5f));

        foreach (CanvasGroup star in _stars)
        {
            RectTransform starTransform = star.GetComponent<RectTransform>();

            sequence.Join(star.DOFade(0f, 0.2f));
            sequence.Join(starTransform.DOAnchorPosX(starTransform.anchoredPosition.x + 700, 0.5f));
        }

        sequence.AppendCallback(() => Destroy(gameObject));
    }
}
