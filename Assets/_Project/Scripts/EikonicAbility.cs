using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EikonicAbility : MonoBehaviour
{
    [SerializeField] protected AbilityData _abilityData;

    public AbilityData AbilityData => _abilityData;

    abstract public void Activate();
}
