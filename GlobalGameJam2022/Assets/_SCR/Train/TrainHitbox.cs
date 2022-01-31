using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainHitbox : MonoBehaviour
{
    [SerializeField] private TrainMovement movement = null;
    [SerializeField] private float triggerSpeed = 1.0f;
    [SerializeField] private Collider myCollider = null;
    [SerializeField] private float damage = 1.0f;

    private void OnTriggerEnter(Collider other) 
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        damageable.Damage(damage, transform.root.gameObject, transform.position, transform.forward);
    }

    private void FixedUpdate() 
    {
        if (movement.Speed >= triggerSpeed)
        {
            myCollider.enabled = !myCollider.enabled;
        }
        else
        {
            myCollider.enabled = false;
        }
    }
}
