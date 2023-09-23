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

    private bool _isCheckingForDodge = true;
    private bool _isNewDodgeBuffered;
    private bool _isPlayingDodgeAnimation;

    public void TriggerDodge()
    {
        if (!_isCheckingForDodge)
        {
            return;
        }

        if (_isPlayingDodgeAnimation)
        {
            _isNewDodgeBuffered = true;
            return;
        }

        _animator.SetTrigger("Dodge");

        _animator.SetFloat("DodgeX", 0);
        _animator.SetFloat("DodgeY", _inputManager.move.magnitude == 0? 0 : 1);

        _isCheckingForDodge = false;
        _isNewDodgeBuffered = false;
        _isPlayingDodgeAnimation = true;
    }

    public void ExecuteDodgeMovement()
    {
        _dashController.DashTowardsInput(_dodgeDistance, _dodgeDuration, -transform.forward, 
            _inputManager.move.magnitude != 0);
    }

    public void StartCheckingForDodge()
    {
        _isCheckingForDodge = true;
    }

    public void OnDodgeAnimationEnd()
    {
        _isPlayingDodgeAnimation = false;

        if (_isNewDodgeBuffered)
        {
            TriggerDodge();
            return;
        }

        OnDodgeEnd?.Invoke();
    }
}
