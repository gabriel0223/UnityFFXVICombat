using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EikonicAbility : MonoBehaviour
{
    [SerializeField] protected AbilityData _abilityData;

    public AbilityData AbilityData => _abilityData;
    public float CurrentCooldown { get; protected set; }

    abstract public void Activate();

    private void Awake()
    {
        CurrentCooldown = AbilityData.Cooldown;
    }

    private void Update()
    {
        CurrentCooldown -= Time.deltaTime;
    }
}
