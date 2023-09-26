using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIndicatorSpawner : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private PlayerComboController _comboController;
    [SerializeField] private PhoenixShift _phoenixShift;
    [SerializeField] private ActionIndicatorView _comboIndicatorPrefab;
    [SerializeField] private ActionIndicatorView _shiftStrikeIndicatorPrefab;

    private void OnEnable()
    {
        _comboController.OnFullComboComplete += HandleFullComboComplete;
        _phoenixShift.OnShiftHitEnemy += HandleShiftHitEnemy;
    }

    private void OnDisable()
    {
        _comboController.OnFullComboComplete -= HandleFullComboComplete;
        _phoenixShift.OnShiftHitEnemy -= HandleShiftHitEnemy;
    }

    private void HandleFullComboComplete()
    {
        SpawnActionIndicator(ActionType.Combo);
    }

    private void HandleShiftHitEnemy()
    {
        SpawnActionIndicator(ActionType.ShiftStrike);
    }

    private void SpawnActionIndicator(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Combo:
                Instantiate(_comboIndicatorPrefab, _canvas);
                break;
            case ActionType.ShiftStrike:
                Instantiate(_shiftStrikeIndicatorPrefab, _canvas);
                break;
            case ActionType.PrecisionDodge:
                break;
            case ActionType.PrecisionCounter:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
    }
}
