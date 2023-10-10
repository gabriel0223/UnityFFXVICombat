using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillButtonView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _skillSlotCanvasGroup;
    [SerializeField] private CanvasGroup _eikonicAbilitySlotCanvasGroup;
    [SerializeField] private float _switchAnimationDuration;
    [SerializeField] private float _switchSizeMultiplier;

    public void SwitchToEikonicAbility()
    {
        _skillSlotCanvasGroup.DOFade(0, _switchAnimationDuration);
        _eikonicAbilitySlotCanvasGroup.DOFade(1, _switchAnimationDuration);
        transform.DOScale(Vector3.one * _switchSizeMultiplier, _switchAnimationDuration);
    }

    public void SwitchToSkill()
    {
        _skillSlotCanvasGroup.DOFade(1, _switchAnimationDuration);
        _eikonicAbilitySlotCanvasGroup.DOFade(0, _switchAnimationDuration);
        transform.DOScale(Vector3.one, _switchAnimationDuration);
    }
}
