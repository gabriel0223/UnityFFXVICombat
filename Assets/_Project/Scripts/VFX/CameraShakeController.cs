using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private PlayerComboController _comboController;
    [SerializeField] private PhoenixShift _phoenixShift;
    [SerializeField] private DodgeController _dodgeController;

    private CinemachineImpulseSource _cinemachineImpulseSource;

    private void Awake()
    {
        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        _comboController.OnFullComboComplete += GenerateCameraShake;
        _phoenixShift.OnShiftStart += GenerateCameraShake;
        _phoenixShift.OnShiftAttack += GenerateCameraShake;
        _dodgeController.OnPrecisionCounter += GenerateCameraShake;
    }

    private void OnDisable()
    {
        _comboController.OnFullComboComplete -= GenerateCameraShake;
        _phoenixShift.OnShiftStart -= GenerateCameraShake;
        _phoenixShift.OnShiftAttack -= GenerateCameraShake;
        _dodgeController.OnPrecisionCounter -= GenerateCameraShake;
    }

    private void GenerateCameraShake()
    {
        _cinemachineImpulseSource.GenerateImpulse();
    }
}
