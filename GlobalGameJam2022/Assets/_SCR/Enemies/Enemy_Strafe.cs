using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class Enemy_Strafe : BaseState
{
    [SerializeField] private EnemyMovement movement = null;
    [SerializeField] private PlayerManager target = null;
    [SerializeField] private Health health = null;

    public override void OnEnter()
    {
        movement.doStrafe = true;
        movement.doZigzag = true;
        movement.targetDistance = 9.0f;

        movement.SetTarget(target.characterController.transform.position, true);
    }

    public override bool CanEnter()
    {
        return health.Get() >= 2.0f;
    }

    public override void OnFixedUpdate()
    {
        movement.SetTarget(target.characterController.transform.position);
    }
}
