using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DodgeController : MonoBehaviour
{
    public event Action OnDodgeEnd;

    [SerializeField] private DashController _dashController;
    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _dodgeDistance;
    [SerializeField] private float _dodgeDuration;

    public void Dodge()
    {
        _animator.SetTrigger("Dodge");

        _animator.SetFloat("DodgeX", 0);
        _animator.SetFloat("DodgeY", _inputManager.move.magnitude == 0? 0 : 1);

        _dashController.DashTowardsInput(_dodgeDistance, _dodgeDuration, -transform.forward, 
            _inputManager.move.magnitude != 0);
    }

    public void OnDodgeAnimationEnd()
    {
        OnDodgeEnd?.Invoke();
    }
}
