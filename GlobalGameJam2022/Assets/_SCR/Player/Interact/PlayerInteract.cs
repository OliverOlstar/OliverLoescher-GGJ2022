using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : PlayerComponent
{
    private InteractableBase target = null;
    [SerializeField] private GameObject toggleUI = null;
    [SerializeField] private PlayerCarry carry = null;

    [Space]
    [SerializeField] private AudioSource source = null;
    [SerializeField] private AudioSource bushSource = null;

    private void Start() 
    {
        input.onTriggerPerformed.AddListener(OnTrigger);
    }

    public void SetInteractable(InteractableBase pInteractable)
    {
        if (enabled == false)
            return;

        if (target == pInteractable)
            return;

        if (pInteractable != null)
        {
            if (carry.IsCarrying)
                return;

            if (!pInteractable.CanInteract())
                return;
        }

        target = pInteractable;
        toggleUI.SetActive(target != null);
    }

    public InteractableBase GetInteractable() => target;

    public void OnTrigger()
    {
        if (target == null)
            return;

        target.Interact();

        if (target is Interact_Carry)
            carry.StartCarrying((Interact_Carry)target);

        if (target is Interact_Bush)
        {
            bushSource.pitch = Random.Range(0.9f, 1.2f);
            bushSource.Play();
        }
        else
        {
            source.pitch = Random.Range(0.7f, 1.2f);
            source.Play();
        }
            
        SetInteractable(null);
    }
}
