using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDodgeable
{   
    GameObject gameObject { get ; } 
    public bool IsInDodgeWindow { get; set; }
    public void StartDodgeWindow();
    public void EndDodgeWindow();
}
