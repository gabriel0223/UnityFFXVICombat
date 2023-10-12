using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "AbilityData", fileName = "NewAbility")]
public class AbilityData : ScriptableObject
{
    public string Name;
    public ButtonDirection ButtonDirection;
    public Sprite Icon;
    public float Duration;
    public float Cooldown;
}
