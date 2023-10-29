using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovingState : EnemyBaseState
{
    enum MovementDirection
    {
        Right,
        Left,
        Forward,
        Back,
    }

    /// <summary>
    /// The probability of the enemy attacking the player
    /// between each reevaluation.
    /// </summary>
    private const float AttackPercentage = 50;

    private const float RetreatDistance = 1f;
    private const float AttackingDistance = 3f;
    private const float DistanceToStartChase = 8f;

    private PlayerCombatController _player;
    private EnemyMovement _enemyMovement;
    private MovementDirection _movementDirection;
    private Vector3 _moveVector;

    public override void EnterState(BaseStateManager ctx)
    {
        _player = GameObject.FindObjectOfType<PlayerCombatController>();
        _enemyMovement = ctx.gameObject.GetComponent<EnemyMovement>();
        _movementDirection = GetRandomMovementDirection();
    }

    public override void UpdateState(BaseStateManager ctx)
    {
        Vector3 playerDirection = (_player.transform.position - ctx.transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(ctx.transform.position, _player.transform.position);

        if (distanceToPlayer <= RetreatDistance)
        {
            _movementDirection = MovementDirection.Back;
        }

        _moveVector = _movementDirection switch
        {
            MovementDirection.Right => Vector3.Cross(Vector3.up, playerDirection),
            MovementDirection.Left => -Vector3.Cross(Vector3.up, playerDirection),
            MovementDirection.Forward => playerDirection,
            MovementDirection.Back => -playerDirection,
        };

        _enemyMovement.RotateTowardsDirection(playerDirection);
        
        if (distanceToPlayer >= DistanceToStartChase)
        {
            _enemyMovement.SetMovementDirection(playerDirection);
            _enemyMovement.SetIsChashing(true);
        }
        else
        {
            _enemyMovement.SetMovementDirection(_moveVector);
            _enemyMovement.SetIsChashing(false); 
        }
    }

    public override void ExitState(BaseStateManager ctx)
    {
        
    }

    public override void ReevaluateState(EnemyStateManager ctx)
    {
        float distanceToPlayer = Vector3.Distance(ctx.transform.position, _player.transform.position);

        if (distanceToPlayer <= AttackingDistance)
        {
            int randomNum = Random.Range(0, 100);

            if (randomNum <= AttackPercentage)
            {
                ctx.SwitchState(ctx.AttackingState);
            }
        }
        else
        {
            _movementDirection = GetRandomMovementDirection();
        }
    }

    /// <summary>
    /// Returns a movement direction for the enemy to move.
    /// They can orbit the player going left or right, or
    /// move towards the player to get closer.
    /// </summary>
    /// <returns></returns>
    private MovementDirection GetRandomMovementDirection()
    {
        int randomNum = Random.Range(0, 3);

        return randomNum switch
        {
            0 => MovementDirection.Right,
            1 => MovementDirection.Left,
            _ => MovementDirection.Forward,
        };
    }
}
