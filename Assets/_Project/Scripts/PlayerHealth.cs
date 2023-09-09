using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerStateManager _playerStateManager;
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
            StartCoroutine(TakeDamageCoroutine());
            //OnTakeHit?.Invoke(this);
        }
    }

    private IEnumerator TakeDamageCoroutine()
    {
        if (_playerStateManager.PlayerState == PlayerState.TakingDamage)
        {
            yield break;
        }

        _animator.SetTrigger("TakeDamage");
        _playerStateManager.SetPlayerState(PlayerState.TakingDamage);

        yield return new WaitForSeconds(1f);

        _playerStateManager.SetPlayerState(PlayerState.Idle);
    }
}
