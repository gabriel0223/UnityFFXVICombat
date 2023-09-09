using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _initialHealth;

    private int _health;

    private void Awake()
    {
        _health = _initialHealth;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            //Die();
        }
        else
        {
            _animator.SetTrigger("TakeDamage");
            //OnTakeHit?.Invoke(this);
        }
    }

}
