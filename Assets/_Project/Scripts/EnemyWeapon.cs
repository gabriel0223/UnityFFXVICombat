using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : WeaponController, IDodgeable
{
    public bool IsInDodgeWindow { get; set; }

    public void StartDodgeWindow()
    {
        IsInDodgeWindow = true;
    }

    public void EndDodgeWindow()
    {
        IsInDodgeWindow = false;
    }
}
