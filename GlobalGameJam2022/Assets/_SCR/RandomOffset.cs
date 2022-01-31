using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomOffset : MonoBehaviour
{
    [SerializeField] private float maxMagnitude = 1.0f;

    void Start()
    {
        transform.position += new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * Random.value * maxMagnitude;
        Destroy(this);
    }
}
