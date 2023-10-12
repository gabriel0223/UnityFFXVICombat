using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializedDictionary("ButtonDirection", "Ability")] 
    [SerializeField] private SerializedDictionary<ButtonDirection, EikonicAbility> _abilities;

    [Tooltip("Ability to be executed by the attack input")]
    [SerializeField] private EikonicAbility _attackEikonicAbility;
    [Tooltip("Ability to be executed by the fire input")]
    [SerializeField] private EikonicAbility _fireEikonicAbility;

    public SerializedDictionary<ButtonDirection, EikonicAbility> Abilities => _abilities;

    private void Start()
    {
        Debug.Log(_attackEikonicAbility.AbilityData.Name);
    }

    public EikonicAbility GetCorrespondentAbility(ButtonDirection buttonDirection)
    {
        return Abilities.FirstOrDefault(a => a.Key == buttonDirection).Value;
    }
}
