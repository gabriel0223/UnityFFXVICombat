using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboController : MonoBehaviour
{
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private Animator _animator;

    private bool _isInAttackAnimation;
    private bool _isCheckingForAttack;
    private int _currentCombo;

    private readonly string[] _attackList = new[] { "Slash1", "Slash2", "Slash3", "Slash4" };
    private Queue<string> _attackQueue;
    private bool _isNextAttackBuffered;

    public void OnAttack()
    {
        if (!_isInAttackAnimation)
        {
            StartCombo();
        }
        else
        {
            if (!_isCheckingForAttack)
            {
                return;
            }

            ExecuteNextAttack();
        }
    }

    private void StartCombo()
    {
        _attackQueue = new Queue<string>(_attackList);

        _animator.SetTrigger(_attackQueue.Dequeue());
    }

    private void ExecuteNextAttack()
    {
        _isCheckingForAttack = false;
        _isNextAttackBuffered = true;

        if (_attackQueue.Count != 0)
        {
            _animator.SetTrigger(_attackQueue.Dequeue());
        }
    }

    public void OnAttackAnimationStart()
    {
        _isInAttackAnimation = true;
        _playerStateManager.SetPlayerState(PlayerState.Attacking);
    }

    public void OnAttackAnimationEnd()
    {
        //if it's in the middle of the combo, return
        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slash"))
        {
            return;
        }

        _isInAttackAnimation = false;
        _playerStateManager.SetPlayerState(PlayerState.Idle);
    }

    public void StartCheckingForAttack()
    {
        _isCheckingForAttack = true;
    }

    public void EndCheckingForAttack()
    {
        _isCheckingForAttack = false;
    }
}
