using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using StarterAssets;
using UnityEngine;

/// <summary>
/// Manages the abilities the player can use and their execution.
/// </summary>
public class AbilityManager : MonoBehaviour
{
    public event Action<ButtonDirection, EikonicAbility> OnEikonicAbilityExecuted;

    [Tooltip("List of abilities available to the player and their respective inputs")]
    [SerializedDictionary("ButtonDirection", "Ability")] 
    [SerializeField] private SerializedDictionary<ButtonDirection, EikonicAbility> _abilities;

    private InputManager _inputManager;
    private bool _canUseAbilities; 

    public SerializedDictionary<ButtonDirection, EikonicAbility> Abilities => _abilities;
    public EikonicAbility AbilityInUse { get; private set; }

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
    }

    private void OnEnable()
    {
        _inputManager.OnEikonicAbilityPressed += HandleAbilityPressed;
    }

    private void OnDisable()
    {
        _inputManager.OnEikonicAbilityPressed -= HandleAbilityPressed;
    }

    public void SetCanUseAbilities(bool state)
    {
        _canUseAbilities = state;
    }

    private void HandleAbilityPressed(ButtonDirection buttonDirection)
    {
        if (!_canUseAbilities)
        {
            return;
        }

        EikonicAbility eikonicAbility = Abilities[buttonDirection];

        if (eikonicAbility.CurrentCooldown > 0)
        {
            return;
        }

        ExecuteAbility(buttonDirection, eikonicAbility);
    }

    private void ExecuteAbility(ButtonDirection buttonDirection, EikonicAbility eikonicAbility)
    {
        if (!eikonicAbility.gameObject.activeInHierarchy)
        {
            eikonicAbility = Instantiate(eikonicAbility, transform.position, transform.rotation);
            eikonicAbility.Initialize(this);
        }

        eikonicAbility.Activate();
        _abilities[buttonDirection] = eikonicAbility;
        AbilityInUse = eikonicAbility;

        OnEikonicAbilityExecuted?.Invoke(buttonDirection, eikonicAbility);
    }
}
