using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class PlayerVFX : MonoBehaviour
{
    private const float SlashYOffset = 90f;

    [SerializeField] private GameObject _beginShiftVfxPrefab;
    [SerializeField] private GameObject _endShiftVfxPrefab;
    [SerializeField] private GameObject _vfxSlash;
    [SerializeField] private GameObject _vfxFireSlash;
    [SerializeField] private Material _phoenixProjectionMaterial;
    [SerializeField] private Material _dodgeProjectionMaterial;
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private SkinnedMeshRenderer _playerSkinnedMesh;
    [SerializeField] private Transform _playerHipsTransform;
    [SerializeField] private Transform _playerWeapon;
    [SerializeField] private Light _outlineLight;
    [SerializeField] private float _outlineLightIntensity;

    private PlayerCombatController _combatController;
    private DodgeController _dodgeController;

    private GameObject _currentVfxSlash;
    private Renderer[] _meshes;
    private SkinnedMeshRenderer[] _skinnedMeshes;
    private Material _materialInstance;

    private void Awake()
    {
        _combatController = GetComponent<PlayerCombatController>();
        _dodgeController = GetComponent<DodgeController>();

        _meshes = GetComponentsInChildren<Renderer>();
        _skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        _materialInstance = new Material(_skinnedMeshes[0].material);

        foreach (SkinnedMeshRenderer skinnedMesh in _skinnedMeshes)
        {
            skinnedMesh.material = _materialInstance;
        }
    }

    private void Start()
    {
        _currentVfxSlash = _vfxSlash;
    }

    private void OnEnable()
    {
        _combatController.OnEnableWeaponDamage += SpawnSlash;
        _dodgeController.OnPrecisionDodge += HandlePrecisionDodgeVFX;
    }

    private void OnDisable()
    {
        _combatController.OnEnableWeaponDamage -= SpawnSlash;
        _dodgeController.OnPrecisionDodge -= HandlePrecisionDodgeVFX;
    }

    public void SpawnBeginPhoenixShiftVfx(Vector3 direction)
    {
        Quaternion vfxRotation = Quaternion.LookRotation(direction);
        Instantiate(_beginShiftVfxPrefab, transform.position, vfxRotation);
    }

    public void SpawnEndPhoenixShiftVfx()
    {
        VisualEffect visualEffect = Instantiate(_endShiftVfxPrefab, transform.position, Quaternion.identity)
            .GetComponent<VisualEffect>();

        visualEffect.SetSkinnedMeshRenderer("PlayerMesh", _playerSkinnedMesh);
        MyVFXTransformBinder transformBinder = visualEffect.gameObject.AddComponent<MyVFXTransformBinder>();
        transformBinder.Target = _playerHipsTransform;
    }

    public void SpawnPhoenixProjection()
    {
        GameObject projection = Instantiate(_playerMesh, _playerMesh.transform.position, _playerMesh.transform.rotation);
        projection.transform.parent = null;

        Renderer[] projectionMeshes = projection.GetComponentsInChildren<Renderer>();

        foreach (Renderer mesh in projectionMeshes)
        {
            mesh.material = _phoenixProjectionMaterial;
        }

        projection.AddComponent<ProjectionController>();
    }

    public void SetFireVfxSlashActive(bool state)
    {
        _currentVfxSlash = state? _vfxFireSlash : _vfxSlash;
    }

    public void SetCharacterMeshesEnabled(bool state)
    {
        foreach (Renderer mesh in _meshes)
        {
            mesh.enabled = state;
        }
    }

    public Tween AnimateCharacterOutlineIntensity(Color initialColor, float startValue, float endValue, float duration)
    {
        _materialInstance.SetColor("_OutlineColor", initialColor);
        _outlineLight.color = initialColor;

        return DOVirtual.Float(startValue, endValue, duration,
            value =>
            {
                _materialInstance.SetColor("_OutlineColor", initialColor * Mathf.Pow(2, value));
                _outlineLight.intensity = value * _outlineLightIntensity;

            });
    }

    private void HandlePrecisionDodgeVFX()
    {
        SpawnDodgeProjection();

        Sequence dodgeSlowMoSequence = DOTween.Sequence();

        dodgeSlowMoSequence.Append(DOVirtual.Float(1f, 0.5f, 0.2f,value => Time.timeScale = value));
        dodgeSlowMoSequence.Append(DOVirtual.Float(0.5f, 1f, 0.2f,value => Time.timeScale = value));
    }

    private void SpawnDodgeProjection()
    {
        GameObject projection = Instantiate(_playerMesh, _playerMesh.transform.position, _playerMesh.transform.rotation);
        projection.transform.parent = null;

        Renderer[] projectionMeshes = projection.GetComponentsInChildren<Renderer>();

        foreach (Renderer mesh in projectionMeshes)
        {
            mesh.material = _dodgeProjectionMaterial;
        }

        projection.AddComponent<ProjectionController>();
    }

    private void SpawnSlash()
    {
        Quaternion weaponRotation = _playerWeapon.rotation;

        GameObject slash = Instantiate(_currentVfxSlash, _playerWeapon.position, weaponRotation);
        
        Quaternion offsetRotation = Quaternion.Euler(0, SlashYOffset, 0);
        Quaternion newSlashRotation = slash.transform.localRotation * offsetRotation;

        slash.transform.rotation = newSlashRotation;
    }
}
