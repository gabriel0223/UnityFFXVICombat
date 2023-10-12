using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// This class can be attached to skill buttons that play an animation when
/// the player executes the input
/// </summary>
public class SkillButtonAnimationPlayer : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private RectTransform _targetTransform;
    [SerializeField] private float _sizeMultiplier;
    [SerializeField] private float _animationDuration;

    private SkillButtonView _skillButton;
    private Vector3 _initialScale;

    private void Awake()
    {
        _skillButton = GetComponent<SkillButtonView>();
        _initialScale = _targetTransform.localScale;
    }

    private void OnEnable()
    {
        switch (_skillButton.ButtonDirection)
        {
            case ButtonDirection.West:
                _inputManager.OnAttackPressed += HandleInputPressed;
                break;
            case ButtonDirection.East:
                _inputManager.OnPhoenixShiftPressed += HandleInputPressed;
                break;
        }
    }

    private void OnDisable()
    {
        switch (_skillButton.ButtonDirection)
        {
            case ButtonDirection.West:
                _inputManager.OnAttackPressed -= HandleInputPressed;
                break;
            case ButtonDirection.East:
                _inputManager.OnPhoenixShiftPressed -= HandleInputPressed;
                break;
        }
    }

    private void HandleInputPressed()
    {
        DOTween.Kill(_targetTransform);

        _targetTransform.localScale = _initialScale;
        _targetTransform.DOScale(Vector3.one * _sizeMultiplier, _animationDuration).SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo);
    }
}
