using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private WeaponController _weapon;
    [SerializeField] private DashController _dashController;
    [SerializeField] private AttackData _attackData;
    [SerializeField] private float _attackDashSpeed;
    [SerializeField] private float _attackDashDuration;

    public void Attack()
    {
        _weapon.SetAttackData(_attackData);
        _animator.SetTrigger(_attackData.AnimationName);
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
        _dashController.DashTowardsDirection(transform.forward * _attackDashSpeed, _attackDashDuration);
    }
}
