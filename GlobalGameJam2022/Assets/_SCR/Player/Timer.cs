using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text = null;

    private void Update() 
    {
        text.text = "Your time is: " + Time.time.ToString();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Train")
        {
            Destroy(gameObject);
        }
    }
}
