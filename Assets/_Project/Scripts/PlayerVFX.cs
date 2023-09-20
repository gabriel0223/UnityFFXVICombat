using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class PlayerVFX : MonoBehaviour
{
    private const float SlashYOffset = 120f;

    [SerializeField] private GameObject _beginShiftVfxPrefab;
    [SerializeField] private GameObject _endShiftVfxPrefab;
    [SerializeField] private GameObject _vfxSlash;
    [SerializeField] private GameObject _vfxFireSlash;
    [SerializeField] private PlayerCombatController _combatController;
    [SerializeField] private SkinnedMeshRenderer _playerSkinnedMesh;
    [SerializeField] private Transform _playerHipsTransform;
    [SerializeField] private Transform _playerWeapon;

    private GameObject _currentVfxSlash;

    private void Start()
    {
        _currentVfxSlash = _vfxSlash;
    }

    private void OnEnable()
    {
        _combatController.OnEnableWeaponCollider += SpawnSlash;
    }

    private void OnDisable()
    {
        _combatController.OnEnableWeaponCollider -= SpawnSlash;
    }

    public void SpawnBeginPhoenixShiftVfx()
    {
        Instantiate(_beginShiftVfxPrefab, transform.position, Quaternion.identity);
    }

    public void SpawnEndPhoenixShiftVfx()
    {
        VisualEffect visualEffect = Instantiate(_endShiftVfxPrefab, transform.position, Quaternion.identity)
            .GetComponent<VisualEffect>();

        visualEffect.SetSkinnedMeshRenderer("PlayerMesh", _playerSkinnedMesh);
        MyVFXTransformBinder transformBinder = visualEffect.gameObject.AddComponent<MyVFXTransformBinder>();
        transformBinder.Target = _playerHipsTransform;
    }

    public void SetFireVfxSlashActive(bool state)
    {
        _currentVfxSlash = state? _vfxFireSlash : _vfxSlash;
    }

    private void SpawnSlash()
    {
        Quaternion weaponRotation = _playerWeapon.rotation;

        GameObject slash = Instantiate(_currentVfxSlash, _playerWeapon.position, weaponRotation);
        
        Quaternion offsetRotation = Quaternion.Euler(0, SlashYOffset, 0);
        Quaternion newSlashRotation = slash.transform.localRotation * offsetRotation;

        slash.transform.localRotation = newSlashRotation;
    }
}
