using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DashController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    private Vector3 _dashDirection;
    private float _dashForceMultiplier;
    private bool _isInDash;

    // Update is called once per frame
    void Update()
    {
        if (!_isInDash)
        {
            return;
        }

        _characterController.Move(_dashDirection * _dashForceMultiplier * Time.deltaTime);
    }

    public void ExecuteDash(Vector3 direction, float duration)
    {
        _isInDash = true;
        _dashDirection = direction;

        Sequence dashSequence = DOTween.Sequence();
        dashSequence.Append(DOVirtual.Float(0f, 1f, duration / 2, value =>
            _dashForceMultiplier = value).SetEase(Ease.Unset));
        dashSequence.Append(DOVirtual.Float(1f, 0f, duration / 2, value =>
            _dashForceMultiplier = value).SetEase(Ease.Unset));
        dashSequence.AppendCallback(() => _isInDash = false);
    }
}
