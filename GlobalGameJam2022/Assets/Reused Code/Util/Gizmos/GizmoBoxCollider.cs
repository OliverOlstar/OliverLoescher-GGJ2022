using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoBoxCollider : GizmoBase
{
    private new Collider collider;

    private void Start() 
    {
        collider = GetComponent<Collider>();
    }

    protected override void DrawGizmos() 
    {
        base.DrawGizmos();

        if (collider == null)
        {
            collider = GetComponent<Collider>();
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
