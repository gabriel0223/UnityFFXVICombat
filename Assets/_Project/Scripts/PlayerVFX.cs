using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    private const float SlashYOffset = 120f;

    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private GameObject _vfxSlash;
    [SerializeField] private Transform _playerWeapon;

    private void OnEnable()
    {
        _combatController.OnEnableWeaponCollider += SpawnSlash;
    }

    private void OnDisable()
    {
        _combatController.OnEnableWeaponCollider -= SpawnSlash;
    }

    private void SpawnSlash()
    {
        Quaternion weaponRotation = _playerWeapon.rotation;
        GameObject slash = Instantiate(_vfxSlash, _playerWeapon.position, weaponRotation);
        
        Quaternion offsetRotation = Quaternion.Euler(0, SlashYOffset, 0);
        Quaternion newSlashRotation = slash.transform.localRotation * offsetRotation;

        slash.transform.localRotation = newSlashRotation;
    }
}
