using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeController : MonoBehaviour
{
    public event Action OnDodgeEnd;

    [SerializeField] private DashController _dashController;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dodgeDistance;
    [SerializeField] private float _dodgeDuration;

    public void Dodge(Vector3 direction)
    {
        _animator.SetTrigger("Dodge");
        _dashController.DashTowardsDirection(direction * _dodgeDistance, _dodgeDuration);
    }

    public void OnDodgeAnimationEnd()
    {
        OnDodgeEnd?.Invoke();
    }
}
