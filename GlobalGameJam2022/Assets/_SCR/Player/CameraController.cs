using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 offset = new Vector3(0.0f, 4.0f, -1.0f);
    [SerializeField] private bool followX = false;
    [SerializeField] private Vector2 minMaxZ = new Vector2(-30, 60);
    

    void FixedUpdate()
    {
        if (target == null)
            return;
        
        Vector3 targetPos = target.position + offset;
        targetPos.y = offset.y;
        targetPos.z = Mathf.Clamp(targetPos.z, minMaxZ.x, minMaxZ.y);
        if (!followX)
            targetPos.x = 0.0f;

        transform.position = targetPos;
    }

    private void OnDrawGizmos() 
    {
        if (Application.isPlaying == false)
        {
            FixedUpdate();
        }
    }
}
