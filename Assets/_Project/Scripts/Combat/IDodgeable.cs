using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes something possible to dodge and trigger a "Precision Dodge"
/// </summary>
public interface IDodgeable
{   
    GameObject gameObject { get ; } 
    public bool IsInDodgeWindow { get; set; }
    public void StartDodgeWindow();
    public void EndDodgeWindow();
}
