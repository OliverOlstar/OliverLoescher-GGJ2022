using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovePiece : MonoBehaviour
{
    [Range(0.001f, 3.0f)] public float length = 1.0f;
    [Range(-1.0f, 1.0f)] public float offset = 0.0f;

    public float ForwardLength => (length * 0.5f) + offset;
    public float BackLength => (length * 0.5f) - offset;

    public void SetProgress(TrainTrack pTrack, float pProgress)
    {
        if (pTrack.TryGetPoint(pProgress, out Vector3 position, out Vector3 forward))
        {
            gameObject.SetActive(true);
            transform.position = position;
            transform.forward = forward;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.forward * offset, new Vector3(0.1f, 0.1f, length));
    }
}
