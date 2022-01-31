using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using OliverLoescher;
using UnityEngine.Events;

public class InputBridge : MonoBehaviour
{
    [SerializeField] private Transform playerTransform = null;
    [FoldoutGroup("Look")] public Vector2 lookInput { get; private set; } = Vector2.zero;
    [FoldoutGroup("Look")] public UnityEventsUtil.Vector2Event onLook;

    // Move
    [FoldoutGroup("Move")] public Vector2 moveInput { get; private set; } = new Vector2();
    [FoldoutGroup("Move")] public Vector3 moveInputVector3 => new Vector3(moveInput.x, 0.0f, moveInput.y);
    [FoldoutGroup("Move")] public UnityEventsUtil.Vector2Event onMove;
    
    // Primary
    [FoldoutGroup("Primary")] public bool isPressingTrigger { get; private set; } = false;
    [FoldoutGroup("Primary")] public UnityEventsUtil.BoolEvent onTrigger;
    [FoldoutGroup("Primary")] public UnityEvent onTriggerPerformed;
    [FoldoutGroup("Primary")] public UnityEvent onTriggerCanceled;

#region Initialize
    private void Start() 
    {
        OliverLoescher.InputSystem.Input.Default.LookPosition.performed += OnLookPosition;
        // OliverLoescher.InputSystem.Input.Default.LookPosition.canceled += OnLookPosition;
        OliverLoescher.InputSystem.Input.Default.LookDirection.performed += OnLook;
        OliverLoescher.InputSystem.Input.Default.LookDirection.canceled += OnLook;
        OliverLoescher.InputSystem.Input.Default.Move.performed += OnMove;
        OliverLoescher.InputSystem.Input.Default.Move.canceled += OnMove;
        OliverLoescher.InputSystem.Input.Default.Trigger.performed += OnTriggerPerformed;
        OliverLoescher.InputSystem.Input.Default.Trigger.canceled += OnTriggerCanceled;
    }

    private void OnDestroy() 
    {
        OliverLoescher.InputSystem.Input.Default.LookPosition.performed -= OnLookPosition;
        // OliverLoescher.InputSystem.Input.Default.LookPosition.canceled -= OnLookPosition;
        OliverLoescher.InputSystem.Input.Default.LookDirection.performed -= OnLook;
        OliverLoescher.InputSystem.Input.Default.LookDirection.canceled -= OnLook;
        OliverLoescher.InputSystem.Input.Default.Move.performed -= OnMove;
        OliverLoescher.InputSystem.Input.Default.Move.canceled -= OnMove;
        OliverLoescher.InputSystem.Input.Default.Trigger.performed -= OnTriggerPerformed;
        OliverLoescher.InputSystem.Input.Default.Trigger.canceled -= OnTriggerCanceled;
    }

    private void OnEnable()
    {
        OliverLoescher.InputSystem.Input.Default.Enable();
    }

    private void OnDisable() 
    {
        OliverLoescher.InputSystem.Input.Default.Disable();
    }
#endregion

    public bool IsValid() => true;

    public void ClearInputs()
    {
        lookInput = Vector2.zero;
        onLook?.Invoke(lookInput);

        moveInput = Vector2.zero;
        onMove?.Invoke(moveInput);

        isPressingTrigger = false;
        onTriggerCanceled?.Invoke();
    }

    public void OnLookPosition(InputAction.CallbackContext ctx) 
    {
        if (IsValid() == false)
            return;

        Vector2 lookPosition = ctx.ReadValue<Vector2>();
        lookInput = lookPosition - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.position);
        lookInput = lookInput.normalized;
        onLook.Invoke(lookInput);
    } 

    public void OnLook(InputAction.CallbackContext ctx) 
    {
        if (IsValid() == false)
            return;

        lookInput = ctx.ReadValue<Vector2>();
        onLook.Invoke(lookInput);
    } 

    private void OnMove(InputAction.CallbackContext ctx) 
    {
        if (IsValid() == false)
            return;

        moveInput = ctx.ReadValue<Vector2>();
        onMove?.Invoke(moveInput);
    }

    private void OnTriggerPerformed(InputAction.CallbackContext ctx)
    {
        if (IsValid() == false)
            return;

        isPressingTrigger = true;
        onTrigger?.Invoke(true);
        onTriggerPerformed?.Invoke();
    }
    private void OnTriggerCanceled(InputAction.CallbackContext ctx)
    {
        if (IsValid() == false)
            return;

        isPressingTrigger = false;
        onTrigger?.Invoke(false);
        onTriggerCanceled?.Invoke();
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawLine(playerTransform.position, playerTransform.position + new Vector3(lookInput.x, 0.0f, lookInput.y));
    }
}
