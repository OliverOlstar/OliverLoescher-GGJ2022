using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum State
    {
        Movement,
        Riding
    }

    [SerializeField] private State currState = State.Movement;
    
    [Header("References")]
    public InputBridge input = null;
    public CharacterMovement movement = null;
    public CharacterController characterController = null;
    public CharacterRiding riding = null;
    public Collider backupCollider = null;
    public PlayerInteract interact = null;

    private PlayerComponent[] components = new PlayerComponent[0];

    private void Awake() 
    {
        components = GetComponentsInChildren<PlayerComponent>();
        foreach (PlayerComponent component in components)
        {
            component.manager = this;
            component.input = input;
            component.Init();
        }
    }

    public void SetState(State pState)
    {
        if (currState == pState)
            return;
        
        // Exit
        switch (currState)
        {
            case State.Movement:
                movement.enabled = false;
                characterController.enabled = false;
                interact.enabled = false;
                break;

            case State.Riding:
                riding.enabled = false;
                backupCollider.enabled = false;
                break;
        }

        currState = pState;

        // Enter
        switch (currState)
        {
            case State.Movement:
                movement.enabled = true;
                characterController.enabled = true;
                interact.enabled = true;
                break;

            case State.Riding:
                riding.enabled = true;
                backupCollider.enabled = true;
                break;
        }
    }
}
