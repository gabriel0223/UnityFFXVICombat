using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes something (object or character) "hittable" by a weapon.
/// </summary>
public interface IDamageable
{
    public void TakeDamage(int damage, DamageType damageType);
}
