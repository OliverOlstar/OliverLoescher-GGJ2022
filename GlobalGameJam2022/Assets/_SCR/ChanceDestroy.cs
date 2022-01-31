using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceDestroy : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float chance01 = 0.5f;

    private void Start() {
        if (Random.value < chance01)
            Destroy(gameObject);
        else
            Destroy(this);
    }
}
