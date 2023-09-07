using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _rotationSpeed;

    private Vector3 _moveDirection;
    private bool _isChashing;

    private void Start()
    {
        _animator.speed = Random.Range(0.9f, 1.1f);
    }

    private void Update()
    {
        Vector3 localVelocity = transform.InverseTransformVector(_characterController.velocity);

        _animator.SetFloat("VelocityX", localVelocity.x, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityZ", localVelocity.z, 0.1f, Time.deltaTime);

        if (_isChashing)
        {
            _characterController.Move(_moveDirection * _chaseSpeed * Time.deltaTime);
        }
        else
        {
            _characterController.Move(_moveDirection * _movementSpeed * Time.deltaTime);
        }
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _moveDirection = direction;
    }

    public void SetIsChashing(bool state)
    {
        _isChashing = state;
    }

    public void RotateTowardsDirection(Vector3 direction)
    {
        float targetYAngle = Quaternion.LookRotation(direction).eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetYAngle, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
