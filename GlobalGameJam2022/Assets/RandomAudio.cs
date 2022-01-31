using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudio : MonoBehaviour
{
    [SerializeField] private AudioSource source = null;
    [SerializeField] private Vector2 delayRange = new Vector2(5.0f, 10.0f);
    [SerializeField] private Vector2 repeatRange = new Vector2(5.0f, 10.0f);

    private void OnEnable() 
    {
        Invoke(nameof(Play), Random.Range(delayRange.x, delayRange.y));
    }

    private void OnDisable() 
    {
        CancelInvoke(nameof(Play));
    }

    private void Play()
    {
        source.Play();
        Invoke(nameof(Play), Random.Range(repeatRange.x, repeatRange.y));
    }
}
