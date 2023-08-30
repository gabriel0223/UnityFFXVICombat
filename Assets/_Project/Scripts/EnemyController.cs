using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int _initialHealth;

    private int _health;

    private void Awake()
    {
         _health = _initialHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
