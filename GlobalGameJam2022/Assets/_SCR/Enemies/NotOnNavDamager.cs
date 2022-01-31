using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NotOnNavDamager : MonoBehaviour
{
    [SerializeField] private OliverLoescher.Health health = null;

    private void Start() 
    {
        InvokeRepeating(nameof(Tick), 3.0f, 1.5f);
    }

    private void Tick()
    {
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            health.Set(0.0f);
    }
}
