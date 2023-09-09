using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private WeaponController _weapon;
    [SerializeField] private DashController _dashController;
    [SerializeField] private float _attackDashSpeed;
    [SerializeField] private float _attackDashDuration;

    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    public void EnableWeaponCollider()
    {
        _weapon.SetColliderActive(true);
    }

    public void DisableWeaponCollider()
    {
        _weapon.SetColliderActive(false);
    }

    public void DashForward()
    {
        _dashController.ExecuteDash(transform.forward * _attackDashSpeed, _attackDashDuration);
    }
}
