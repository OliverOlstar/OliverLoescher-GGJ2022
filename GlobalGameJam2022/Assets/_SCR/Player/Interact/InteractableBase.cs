using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        PlayerInteract player = other.GetComponent<PlayerInteract>();

        if (player == null)
            return;

        player.SetInteractable(this);
    }

    private void OnTriggerExit(Collider other) 
    {
        PlayerInteract player = other.GetComponent<PlayerInteract>();

        if (player == null)
            return;

        if (player.GetInteractable() == this)
            player.SetInteractable(null);
    }

    public abstract bool CanInteract();
    public abstract void Interact();
}
