using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class Enemy_Attack : BaseState
{
    [SerializeField] private EnemyMovement movement = null;
    [SerializeField] private PlayerManager target = null;

    private float coolDownEndTime = 0.0f;

    public override void Init(StateMachine pMachine)
    {
        base.Init(pMachine);

        coolDownEndTime = Time.time + 10.0f;
    }

    public override bool CanEnter()
    {
        return Time.time > coolDownEndTime;
    }

    public override void OnEnter()
    {
        movement.doStrafe = false;
        movement.doZigzag = false;
        movement.targetDistance = 1.0f;
    
        movement.SetTarget(target.characterController.transform.position + FuncUtil.Horizontalize(target.characterController.transform.position - transform.position, true) * 0.25f, true, true);

        movement.onReachedTarget.AddListener(OnReachedTarget);
    }

    public override void OnExit()
    {
        coolDownEndTime = Time.time + 5.0f;

        movement.onReachedTarget.RemoveListener(OnReachedTarget);
    }

    public override void OnFixedUpdate()
    {
        if (target == null || target.characterController == null)
            return;

        movement.SetTarget(target.characterController.transform.position + FuncUtil.Horizontalize(target.characterController.transform.position - transform.position, true) * 0.25f);
    }

    private void OnReachedTarget()
    {
        machine.ReturnToDefault();
    }
}
