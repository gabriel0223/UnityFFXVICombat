using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// This class can be attached to skill buttons that play an animation when
/// the player executes the input
/// </summary>
public class SkillButtonAnimationPlayer : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private SkillType _skillType;
    [SerializeField] private RectTransform _targetTransform;
    [SerializeField] private float _sizeMultiplier;
    [SerializeField] private float _animationDuration;

    private Vector3 _initialScale;

    private void Awake()
    {
        _initialScale = _targetTransform.localScale;
    }

    private void OnEnable()
    {
        switch (_skillType)
        {
            case SkillType.Attack:
                _inputManager.OnAttackPressed += HandleInputPressed;
                break;
            case SkillType.PhoenixShift:
                _inputManager.OnPhoenixShiftPressed += HandleInputPressed;
                break;
        }
    }

    private void OnDisable()
    {
        switch (_skillType)
        {
            case SkillType.Attack:
                _inputManager.OnAttackPressed -= HandleInputPressed;
                break;
            case SkillType.PhoenixShift:
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
