using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class SkillButtonsManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private SkillButtonView[] _skillButtons;

    private void OnEnable()
    {
        _inputManager.OnShowAbilitiesPressed += HandleShowAbilitiesPressed;
        _inputManager.OnShowAbilitiesReleased += HandleShowAbilitiesReleased;
    }

    private void HandleShowAbilitiesPressed()
    {
        foreach (SkillButtonView skillButton in _skillButtons)
        {
            skillButton.SwitchToEikonicAbility();
        }
    }

    private void HandleShowAbilitiesReleased()
    {
        foreach (SkillButtonView skillButton in _skillButtons)
        {
            skillButton.SwitchToSkill();
        }
    }
}
