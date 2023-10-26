using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EikonicAbility : MonoBehaviour
{
    [SerializeField] protected AbilityData _abilityData;

    protected bool _isFinished;

    public AbilityData AbilityData => _abilityData;
    public float CurrentCooldown { get; protected set; }

    abstract public void Activate(AbilityManager ctx);
    abstract public void End();

    private void Update()
    {
        CurrentCooldown -= Time.deltaTime;
    }
}
