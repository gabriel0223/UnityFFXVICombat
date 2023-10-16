using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonView : MonoBehaviour
{
    [SerializeField] private ButtonDirection _buttonDirection;
    [SerializeField] private CanvasGroup _skillSlotCanvasGroup;
    [SerializeField] private CanvasGroup _eikonicAbilitySlotCanvasGroup;
    [SerializeField] private Image _eikonicAbilityIcon;
    [SerializeField] private GameObject _eikonicAbilityDarkPanel;
    [SerializeField] private Image _eikonicAbilityCooldownBar;
    [SerializeField] private TMP_Text _skillNameText;
    [SerializeField] private float _switchAnimationDuration;
    [SerializeField] private float _switchSizeMultiplier;

    private AbilityData _abilityData;
    private string _originalSkillName;

    public ButtonDirection ButtonDirection => _buttonDirection;

    private void Awake()
    {
        _originalSkillName = _skillNameText.GetParsedText();
    }

    public void SwitchToEikonicAbility()
    {
        _skillSlotCanvasGroup.DOFade(0, _switchAnimationDuration);
        _eikonicAbilitySlotCanvasGroup.DOFade(1, _switchAnimationDuration);
        transform.DOScale(Vector3.one * _switchSizeMultiplier, _switchAnimationDuration);

        if (_abilityData != null)
        {
            _skillNameText.SetText(_abilityData.Name);
        }
    }

    public void SwitchToSkill()
    {
        _skillSlotCanvasGroup.DOFade(1, _switchAnimationDuration);
        _eikonicAbilitySlotCanvasGroup.DOFade(0, _switchAnimationDuration);
        transform.DOScale(Vector3.one, _switchAnimationDuration);
        _skillNameText.SetText(_originalSkillName);
    }

    public void SetAbilityData(AbilityData abilityData)
    {
        _abilityData = abilityData;
        _eikonicAbilityIcon.sprite = abilityData.Icon;
    }

    public void HandleAbilityExecuted()
    {
        _eikonicAbilityDarkPanel.SetActive(true);
        _eikonicAbilityCooldownBar.DOFillAmount(1, _abilityData.Cooldown)
            .OnComplete(HandleAbilityCooldownEnded);
    }

    private void HandleAbilityCooldownEnded()
    {
        _eikonicAbilityDarkPanel.SetActive(false);
        _eikonicAbilityCooldownBar.fillAmount = 0;
    }
}
