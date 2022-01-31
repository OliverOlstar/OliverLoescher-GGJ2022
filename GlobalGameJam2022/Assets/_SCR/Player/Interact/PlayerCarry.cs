using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarry : PlayerComponent
{
    public enum ItemType
    {
        Log,
        TrainPiece
    }

    public bool IsCarrying => item != null;
    [SerializeField] private Transform carryContainer = null;
    private Interact_Carry item = null;
    public Interact_Carry GetItem() => item;
    [SerializeField] private AudioSource source = null;

    public override void Init()
    {
        input.onTriggerPerformed.AddListener(OnInteract);
    }

    public void StartCarrying(Interact_Carry pItem)
    {
        item = pItem;

        pItem.root.SetParent(carryContainer);
        pItem.root.localPosition = Vector3.zero;
        pItem.root.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                
        EZCameraShake.CameraShaker.Instance.ShakeOnce(4.0f, 1.0f, 0.05f, 0.1f);

        manager.movement.acceleration *= item.carryAccel;
        
        // Create building things
    }

    public void StopCarrying(bool pDrop)
    {
        if (!IsCarrying)
            return;
            
        manager.movement.acceleration /= item.carryAccel;

        if (pDrop)
            EZCameraShake.CameraShaker.Instance.ShakeOnce(2.0f, 1.0f, 0.05f, 0.1f);
        else
            EZCameraShake.CameraShaker.Instance.ShakeOnce(6.0f, 1.0f, 0.08f, 0.3f);

        if (item.type == ItemType.Log || pDrop)
        {
            source.pitch = Random.Range(0.5f, 0.8f);
            source.Play();
        }

        item.root.position = new Vector3(item.root.position.x, 0.0f, item.root.position.z) + transform.forward * 0.5f;
        item.root.SetParent(null);
        item = null;
    }

    public void OnInteract()
    {
        StopCarrying(true);
    }
}
