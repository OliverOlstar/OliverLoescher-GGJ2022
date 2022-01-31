using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainAudioLoop : MonoBehaviour
{
    [SerializeField] private TrainMovement movement = null;
    [SerializeField] private AudioSource source = null;
    [SerializeField] private AudioSource hornSource = null;
    private float timer = 0.0f;
    private float seconds = 40.0f;

    void FixedUpdate()
    {
        float p01 = Mathf.Abs(movement.Speed) / movement.minMaxSpeed.y;
        source.pitch = p01 * 0.5f + 0.5f;
        source.volume = p01 * 0.5f;

        if (p01 > 0.65f)
        {
            timer += Time.fixedDeltaTime;
            if (timer > seconds)
            {
                timer = 0.0f;
                hornSource.Play();
                seconds = Random.Range(10.0f, 40.0f);
            }
        }
        else
        {
            timer = 0.0f;
            seconds = Random.Range(5.0f, 40.0f);
        }
    }
}
