using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRiding : PlayerComponent
{
    public PlayerMountTrigger target = null;
    [SerializeField] private Vector3 offset = Vector3.up;

    private float canExitTime = 0.0f;

    private void OnEnable() 
    {
        input.onMove.AddListener(OnMove);
        input.onTrigger.AddListener(OnTrigger);
        input.onLook.AddListener(OnLook);
    }

    private void OnDisable() 
    {
        input.onMove.RemoveListener(OnMove);
        input.onTrigger.RemoveListener(OnTrigger);
        input.onLook.RemoveListener(OnLook);
    }

    public bool IsValid => target == null;

    public void SetTarget(PlayerMountTrigger pTarget)
    {
        // Exit
        if (target != null)
            target.OnExit();

        target = pTarget;

        // Enter
        if (target != null)
        {
            manager.SetState(PlayerManager.State.Riding);
            canExitTime = Time.time + 0.5f;
        }
        else
        {
            manager.SetState(PlayerManager.State.Movement);
        }
    }

    private void FixedUpdate() 
    {
        if (target == null)
            return;

        transform.position = target.follow.position + offset;
        transform.forward = target.follow.forward;
    }

    public void OnMove(Vector2 pInput)
    {
        if (!enabled)
            return;

        if (Mathf.Abs(pInput.x) > 0.5f && Time.time > canExitTime)
        {
            transform.position += transform.right * (pInput.x > 0.0f ? 1 : -1) * 0.85f;
            SetTarget(null);
        }

        if (target != null)
            target.OnMove(pInput);
    }

    public void OnTrigger(bool pBool)
    {
        if (target != null)
            target.OnTrigger(pBool);
    }

    public void OnLook(Vector2 pDirection)
    {
        if (target != null)
            target.OnLook(pDirection);
    }
}
