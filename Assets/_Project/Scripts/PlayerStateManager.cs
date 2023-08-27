using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PlayerState PlayerState { get; private set; }

    public void SetPlayerState(PlayerState newPlayerState)
    {
        PlayerState = newPlayerState;
    }
}
