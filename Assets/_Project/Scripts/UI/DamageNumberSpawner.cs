using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageNumberSpawner : MonoBehaviour
{
    private const float CharacterYOffset = 1f;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private DamageNumberView _enemyDamageNumberPrefab;
    [SerializeField] private DamageNumberView _playerDamageNumberPrefab;
    [SerializeField] private float _positionRandomness;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        WeaponController.OnWeaponHitHealth += SpawnDamageNumber;
    }

    private void OnDisable()
    {
        WeaponController.OnWeaponHitHealth -= SpawnDamageNumber;
    }

    private void SpawnDamageNumber(HealthBase health, int damage)
    {
        Vector3 healthPosition = new Vector3(health.transform.position.x,
            health.transform.position.y + CharacterYOffset, health.transform.position.z);
        Vector2 screenPosition = _mainCamera.WorldToScreenPoint(healthPosition);
        Vector2 randomScreenPosition = new Vector2(screenPosition.x + Random.Range(-_positionRandomness, _positionRandomness),
            screenPosition.y + Random.Range(-_positionRandomness, _positionRandomness));

        // Convert Screen Space position to Canvas Local Position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), 
            randomScreenPosition, _canvas.worldCamera, out Vector2 localPosition);

        DamageNumberView damageNumber;

        if (health.gameObject.GetComponent<EnemyHealth>() != null)
        {
            damageNumber = Instantiate(_enemyDamageNumberPrefab, _canvas.transform);
        }
        else
        {
            damageNumber = Instantiate(_playerDamageNumberPrefab, _canvas.transform);
        }

        damageNumber.Initialize(damage, localPosition);
    }
}
