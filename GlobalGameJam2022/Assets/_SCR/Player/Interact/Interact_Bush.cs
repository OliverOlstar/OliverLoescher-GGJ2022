using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interact_Bush : InteractableBase
{
    [SerializeField] private List<GameObject> destroyObjects = new List<GameObject>();
    [SerializeField] private OliverLoescher.Health player = null;
    [SerializeField] private float heal = 1.0f;

    public override bool CanInteract()
    {
        return player.Get() < player.GetMax(); 
    }

    public override void Interact()
    {
        while (destroyObjects.Count > 0)
        {
            GameObject o = destroyObjects[0];
            destroyObjects.Remove(o);
            Destroy(o);
        }
        player.Modify(heal);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(2.0f, 1.0f, 0.05f, 0.1f);

        Destroy(this);
        Destroy(GetComponent<Collider>());
    }
}
