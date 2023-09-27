using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActionIndicatorSpawner : MonoBehaviour
{
    [SerializeField] private Transform _actionIndicatorsContainer;
    [SerializeField] private PlayerComboController _comboController;
    [SerializeField] private PhoenixShift _phoenixShift;
    [SerializeField] private DodgeController _dodgeController;
    [SerializeField] private ActionIndicatorView _comboIndicatorPrefab;
    [SerializeField] private ActionIndicatorView _shiftStrikeIndicatorPrefab;
    [SerializeField] private ActionIndicatorView _precisionDodgeIndicatorPrefab;

    private void OnEnable()
    {
        _comboController.OnFullComboComplete += HandleFullComboComplete;
        _phoenixShift.OnShiftHitEnemy += HandleShiftHitEnemy;
        _dodgeController.OnPrecisionDodge += HandlePrecisionDodge;
    }

    private void OnDisable()
    {
        _comboController.OnFullComboComplete -= HandleFullComboComplete;
        _phoenixShift.OnShiftHitEnemy -= HandleShiftHitEnemy;
        _dodgeController.OnPrecisionDodge -= HandlePrecisionDodge;
    }

    private void HandleFullComboComplete()
    {
        SpawnActionIndicator(ActionType.Combo);
    }

    private void HandleShiftHitEnemy()
    {
        SpawnActionIndicator(ActionType.ShiftStrike);
    }

    private void HandlePrecisionDodge()
    {
        SpawnActionIndicator(ActionType.PrecisionDodge);
    }

    private void SpawnActionIndicator(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Combo:
                Instantiate(_comboIndicatorPrefab, _actionIndicatorsContainer);
                break;
            case ActionType.ShiftStrike:
                Instantiate(_shiftStrikeIndicatorPrefab, _actionIndicatorsContainer);
                break;
            case ActionType.PrecisionDodge:
                Instantiate(_precisionDodgeIndicatorPrefab, _actionIndicatorsContainer);
                break;
            case ActionType.PrecisionCounter:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
    }
}
