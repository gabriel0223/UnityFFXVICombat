using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct that stores attack data, specifically
/// animation name, damage multiplier and damage type.
/// </summary>
[Serializable]
public struct AttackData
{
    public string AnimationName;
    public float DamageMultiplier;
    public DamageType DamageType;

    public AttackData(string animationName, float damageMultiplier, DamageType damageType)
    {
        AnimationName = animationName;
        DamageMultiplier = damageMultiplier;
        DamageType = damageType;
    }
}
