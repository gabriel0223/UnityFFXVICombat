using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class PlayerVFX : MonoBehaviour
{
    private const float SlashYOffset = 90f;

    [SerializeField] private VisualEffect _beginShiftVfxPrefab;
    [SerializeField] private VisualEffect _endShiftVfxPrefab;
    [SerializeField] private VisualEffect _vfxSlash;
    [SerializeField] private VisualEffect _vfxFireSlash;
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

    private Renderer[] _meshes;
    private SkinnedMeshRenderer[] _skinnedMeshes;
    private Material _materialInstance;

    private ObjectPool<VisualEffect> _slashPool;
    private ObjectPool<VisualEffect> _fireSlashPool;
    private ObjectPool<VisualEffect> _beginShiftPool;
    private ObjectPool<VisualEffect> _endShiftPool;

    private bool _isFireSlashActive;

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
        _slashPool = CreateVisualEffectPool(_vfxSlash,2, 3);
        _fireSlashPool = CreateVisualEffectPool(_vfxFireSlash,1, 2);
        _beginShiftPool = CreateVisualEffectPool(_beginShiftVfxPrefab,1, 2);
        _endShiftPool = CreateVisualEffectPool(_endShiftVfxPrefab,1, 2);
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
        VisualEffect visualEffect = _beginShiftPool.Get();

        visualEffect.transform.position = transform.position;
        visualEffect.transform.rotation = vfxRotation;

        DOVirtual.DelayedCall(2f, () => _beginShiftPool.Release(visualEffect));
    }

    public void SpawnEndPhoenixShiftVfx()
    {
        VisualEffect visualEffect = _endShiftPool.Get();

        visualEffect.SetSkinnedMeshRenderer("PlayerMesh", _playerSkinnedMesh);
        MyVFXTransformBinder transformBinder = visualEffect.gameObject.AddComponent<MyVFXTransformBinder>();
        transformBinder.Target = _playerHipsTransform;

        visualEffect.transform.position = transform.position;

        DOVirtual.DelayedCall(2f, () => _endShiftPool.Release(visualEffect));
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
        _isFireSlashActive = state;
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

    private ObjectPool<VisualEffect> CreateVisualEffectPool(VisualEffect vfxPrefab, int defaultCapacity, int maxSize)
    {
        return new ObjectPool<VisualEffect>(
            () =>
            {
                return Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            }, vfx =>
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }, vfx =>
            {
                vfx.gameObject.SetActive(false);
            },
            vfx =>
            {
                Destroy(vfx.gameObject);
            }, false, defaultCapacity, maxSize);
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

        VisualEffect slashVfx = _isFireSlashActive? _fireSlashPool.Get() : _slashPool.Get();
        GameObject slash = slashVfx.gameObject;

        Quaternion offsetRotation = Quaternion.Euler(0, SlashYOffset, 0);
        Quaternion newSlashRotation = weaponRotation * offsetRotation;

        slash.transform.position = _playerWeapon.position;
        slash.transform.rotation = newSlashRotation;

        if (_isFireSlashActive)
        {
            DOVirtual.DelayedCall(1f, () => _fireSlashPool.Release(slashVfx));
        }
        else
        {
            DOVirtual.DelayedCall(1f, () => _slashPool.Release(slashVfx));
        }
    }
}
