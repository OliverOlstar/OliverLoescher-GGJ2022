using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTrigger : MonoBehaviour
{
    [SerializeField] private OliverLoescher.Health health = null;
    [SerializeField] private float heal = 5.0f;
    private int doHeal = 0;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            if (health.Get() < health.GetMax())
                health.Modify(heal);
        }
    }
}
