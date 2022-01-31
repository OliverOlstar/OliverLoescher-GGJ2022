using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimationController : MonoBehaviour
{
    public enum RotationType 
    { 
        None, 
        EnemyTarget,
        MoveTarget,
        MoveVelocity,
        MovePath
    }

    [Header("-----------Rotation-------------------------")]
    public RotationType rotType = RotationType.None;
    public bool doRotation = true;

    //PRIVATE
    private Enemy enemy;
    EnemyMovement movement;

    public float rotationSpeed;

    public void Initialize(Enemy pEnemy, EnemyMovement pMovement)
    {
        enemy = pEnemy;
        movement = pMovement;
    }

    private void FixedUpdate() 
    {
        DoRotation();
    }
    
    // public void DoDeath()
    // {
    //     // Animation
    //     // animator.SetInteger(HIT_WEAPON, HIT_WEAPON_INDEX);
    //     animator.SetTrigger(DEAD);
    //     if (deathAnimations > 1)
    //         animator.SetInteger(DEADINDEX, Random.Range(0, deathAnimations));

    //     // Dissolve
    //     if (dissolveRenderers.Length > 0)
    //         StartDissolve();
    // }

    private void DoRotation()
    {
        // Exit if mult is 0 to prevent wasted calls
        if (doRotation == false)
            return;

        // Get Target Direction
        Vector3 dir;
        switch (rotType)
        {
            case RotationType.EnemyTarget:
                // dir = enemy.target.position - transform.position;
                throw new System.Exception("Not Implemented");
                // break;

            case RotationType.MoveTarget:
                dir = movement.target - transform.position;
                break;

            case RotationType.MoveVelocity:
                dir = movement.velocity;
                break;

            case RotationType.MovePath:
                if (movement.path != null)
                    dir = movement.GetNextPathPoint() - transform.position;
                else
                    return;
                break;

            default: // RotationType.None
                return;
        }

        // Actual Rotation
        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            Quaternion targetQ = Quaternion.LookRotation(dir);
            if (targetQ != transform.rotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQ, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
