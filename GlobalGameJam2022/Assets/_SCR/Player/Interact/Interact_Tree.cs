using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Tree : InteractableBase
{
    [SerializeField] private GameObject prefab = null;

    public override bool CanInteract()
    {
        return true; 
    }

    public override void Interact()
    {
        Destroy(gameObject);
        Instantiate(prefab, transform.position + randomOffset(), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f)).SetActive(true);
        Instantiate(prefab, transform.position + randomOffset(), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f)).SetActive(true);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(5.0f, 2.0f, 0.05f, 0.1f);
    }

    private Vector3 randomOffset()
    {
        return new Vector3(Random.value - 0.5f, 0.0f, Random.value - 0.5f).normalized * Random.value * 2.0f;
    }
}
