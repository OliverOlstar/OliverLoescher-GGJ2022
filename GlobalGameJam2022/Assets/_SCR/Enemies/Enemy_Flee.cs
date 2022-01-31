using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class Enemy_Flee : BaseState
{
    [SerializeField] private EnemyMovement movement = null;
    [SerializeField] private PlayerManager target = null;

    public override void OnEnter()
    {
        movement.doStrafe = false;
        movement.doZigzag = true;
        movement.targetDistance = 1.0f;
    }

    public override void OnFixedUpdate()
    {
        movement.SetTarget(target.characterController.transform.position + FuncUtil.Horizontalize(transform.position - target.characterController.transform.position) * 5.0f);
    }
}
