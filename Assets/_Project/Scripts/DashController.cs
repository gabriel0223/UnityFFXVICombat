using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class DashController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private InputManager _input;

    private Camera _mainCamera;
    private Vector3 _inputDirection;
    private Vector3 _dashDirection;
    private float _dashForceMultiplier;
    private bool _isInDash;
    private Sequence _dashSequence;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isInDash)
        {
            return;
        }

        _characterController.Move(_dashDirection * _dashForceMultiplier * Time.deltaTime);
    }

    public void DashTowardsDirection(Vector3 direction, float duration, bool rotatesTowardsDirection = false)
    {
        _isInDash = true;
        _dashDirection = direction;

        if (rotatesTowardsDirection && direction != Vector3.zero)
        {
            Quaternion directionRotation = Quaternion.LookRotation(direction);
            transform.DORotateQuaternion(directionRotation, 0.25f);
        }

        _dashSequence?.Kill();
        _dashSequence = DOTween.Sequence();

        _dashSequence.Append(DOVirtual.Float(0f, 1f, duration / 2, value =>
            _dashForceMultiplier = value).SetEase(Ease.Unset));
        _dashSequence.Append(DOVirtual.Float(1f, 0f, duration / 2, value =>
            _dashForceMultiplier = value).SetEase(Ease.Unset));
        _dashSequence.AppendCallback(() => _isInDash = false);
    }

    public void DashTowardsInput(float distance, float duration, Vector3 noInputDirection, bool rotatesTowardsDirection = true)
    {
        _inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        float inputDirectionAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg +
                                    _mainCamera.transform.eulerAngles.y;

        Vector3 targetDirection = _inputDirection.magnitude == 0? noInputDirection :
            Quaternion.Euler(0.0f, inputDirectionAngle, 0.0f) * Vector3.forward;

        Vector3 dashDirection = _input.move == Vector2.zero ? noInputDirection * distance
            : targetDirection * distance;

        DashTowardsDirection(dashDirection, duration, rotatesTowardsDirection);
    }
}
