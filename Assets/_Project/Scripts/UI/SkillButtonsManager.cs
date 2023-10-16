using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillButtonsManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private AbilityManager _abilityManager;
    [SerializeField] private SkillButtonView[] _skillButtons;
    [SerializeField] private RectTransform _showAbilitiesButtomPrompt;
    [SerializeField] private CanvasGroup _circles;
    [SerializeField] private CanvasGroup _glowyLineImage;

    private void OnEnable()
    {
        _inputManager.OnShowAbilitiesPressed += HandleShowAbilitiesPressed;
        _inputManager.OnShowAbilitiesReleased += HandleShowAbilitiesReleased;
        _abilityManager.OnEikonicAbilityExecuted += HandleEikonicAbilityExecuted;
    }

    private void OnDisable()
    {
        _inputManager.OnShowAbilitiesPressed += HandleShowAbilitiesPressed;
        _inputManager.OnShowAbilitiesReleased += HandleShowAbilitiesReleased;
        _abilityManager.OnEikonicAbilityExecuted -= HandleEikonicAbilityExecuted;
    }

    private void Start()
    {
        foreach (SkillButtonView skillButton in _skillButtons)
        {
            ButtonDirection buttonDirection = skillButton.ButtonDirection;
            EikonicAbility matchingAbility = _abilityManager.GetCorrespondentAbility(buttonDirection);

            if (matchingAbility != null)
            {
                skillButton.SetAbilityData(matchingAbility.AbilityData);
            }
        }
    }

    private void HandleShowAbilitiesPressed()
    {
        foreach (SkillButtonView skillButton in _skillButtons)
        {
            skillButton.SwitchToEikonicAbility();
        }

        _showAbilitiesButtomPrompt.transform.localScale = Vector3.one;
        _circles.alpha = 0;
        _circles.transform.localScale = Vector3.one;
        _glowyLineImage.alpha = 0;
        _glowyLineImage.transform.localScale = new Vector3(1f, 0.3f, 1f);

        Sequence showAnimation = DOTween.Sequence();
        showAnimation.Append(_showAbilitiesButtomPrompt.DOScale(Vector3.zero, 0.1f));
        showAnimation.Join(_circles.DOFade(1, 0.1f));
        showAnimation.Join(_circles.transform.DOScale(Vector3.one * 1.3f, 0.25f));
        showAnimation.Join(_glowyLineImage.DOFade(1, 0.1f));
        showAnimation.Join(_glowyLineImage.transform.DOScale(Vector3.one, 0.25f));
        showAnimation.Append(_circles.DOFade(0, 0.25f));
        showAnimation.Join(_circles.transform.DOScale(Vector3.one * 1.4f, 0.25f));
        showAnimation.Join(_glowyLineImage.DOFade(0, 0.25f));
    }

    private void HandleShowAbilitiesReleased()
    {
        foreach (SkillButtonView skillButton in _skillButtons)
        {
            skillButton.SwitchToSkill();
        }

        Sequence showAnimation = DOTween.Sequence();
        showAnimation.Append(_showAbilitiesButtomPrompt.DOScale(Vector3.one, 0.1f));
    }

    private void HandleEikonicAbilityExecuted(ButtonDirection buttonDirection, EikonicAbility eikonicAbility)
    {
        SkillButtonView skillButton = _skillButtons.FirstOrDefault(b => b.ButtonDirection == buttonDirection);

        if (skillButton != null)
        {
            skillButton.HandleAbilityExecuted();
        }
    }
}
