using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Carry : InteractableBase
{
    [Range(0, 1)] public float carryAccel = 0.8f;
    public PlayerCarry.ItemType type = PlayerCarry.ItemType.Log;
    public Transform root = null;

    [SerializeField] private List<GameObject> toggleOnDestroy = new List<GameObject>();

    public override bool CanInteract()
    {
        return true; 
    }

    public override void Interact()
    {

    }

    private Vector3 randomOffset()
    {
        return new Vector3(Random.value - 0.5f, 0.0f, Random.value - 0.5f).normalized * Random.value * 2.0f;
    }

    public void Finished() 
    {
        foreach (GameObject t in toggleOnDestroy)
        {
            t.SetActive(!t.activeSelf);
        }
    }
}
