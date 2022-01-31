using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        transform.rotation *= Quaternion.AngleAxis(Random.value * 360, Vector3.up);
        Destroy(this);
    }
}
