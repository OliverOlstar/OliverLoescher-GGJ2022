using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoBase : MonoBehaviour
{
    [SerializeField] private Color color = new Color(0, 0.5f, 1, 1);
    [SerializeField] private bool alwaysShow = false;

    protected virtual void OnDrawGizmos() 
    {
        if (alwaysShow == true) 
        {
            DrawGizmos();
        }
    }

    protected virtual void OnDrawGizmosSelected() 
    {
        if (alwaysShow == false) 
        {
            DrawGizmos();
        }
    }

    protected virtual void DrawGizmos() 
    { 
        Gizmos.color = color;
    }
}
