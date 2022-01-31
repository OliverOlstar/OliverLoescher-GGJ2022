using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraSetter : MonoBehaviour
{
    private void Start() 
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        Destroy(this);
    }
}
