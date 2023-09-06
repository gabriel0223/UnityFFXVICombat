using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;

    private Vector3 _moveDirection;

    private void Update()
    {
        _animator.SetFloat("VelocityX", _characterController.velocity.x);
        _animator.SetFloat("VelocityY", _characterController.velocity.z);

        _characterController.Move(_moveDirection * _movementSpeed * Time.deltaTime);
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _moveDirection = direction;
    }

    public void RotateTowardsDirection(Vector3 direction)
    {
        float targetYAngle = Quaternion.LookRotation(direction).eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetYAngle, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
