using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using StarterAssets;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public event Action<ButtonDirection, EikonicAbility> OnEikonicAbilityExecuted;

    [Tooltip("List of abilities available to the player and their respective inputs")]
    [SerializedDictionary("ButtonDirection", "Ability")] 
    [SerializeField] private SerializedDictionary<ButtonDirection, EikonicAbility> _abilities;

    private InputManager _inputManager;

    public SerializedDictionary<ButtonDirection, EikonicAbility> Abilities => _abilities;

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

    private void HandleAbilityPressed(ButtonDirection buttonDirection)
    {
        EikonicAbility eikonicAbility = Abilities[buttonDirection];

        if (eikonicAbility.CurrentCooldown > 0)
        {
            return;
        }

        if (!eikonicAbility.gameObject.activeInHierarchy)
        {
            eikonicAbility = Instantiate(eikonicAbility);
        }

        eikonicAbility.Activate(transform.position);
        _abilities[buttonDirection] = eikonicAbility;

        OnEikonicAbilityExecuted?.Invoke(buttonDirection, eikonicAbility);
    }
}
