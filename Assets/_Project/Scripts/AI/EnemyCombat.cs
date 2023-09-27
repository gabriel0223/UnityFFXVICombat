using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyWeapon _weapon;
    [SerializeField] private DashController _dashController;
    [SerializeField] private AttackData _attackData;
    [SerializeField] private float _attackDashSpeed;
    [SerializeField] private float _attackDashDuration;

    public void Attack()
    {
        _weapon.SetAttackData(_attackData);
        _animator.SetTrigger(_attackData.AnimationName);
    }

    public void EnableWeaponDamage()
    {
        _weapon.EndDodgeWindow();
        _weapon.SetCanDoDamage(true);
    }

    public void DisableWeaponDamage()
    {
        _weapon.SetCanDoDamage(false);
    }

    public void StartDodgeWindow()
    {
        _weapon.StartDodgeWindow();
    }

    public void DashForward()
    {
        _dashController.DashTowardsDirection(transform.forward * _attackDashSpeed, _attackDashDuration);
    }
}
