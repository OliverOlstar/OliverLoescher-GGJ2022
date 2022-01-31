using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private EnemyMovement movement = null;

    private void OnTriggerEnter(Collider other) 
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        if (other.tag == "Enemy")
            return;

        damageable.Damage(1.0f, transform.root.gameObject, transform.position, movement.velocity);
    }
}
