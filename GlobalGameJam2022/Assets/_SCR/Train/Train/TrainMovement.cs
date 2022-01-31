using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private TrainTrack track = null;
    public List<TrainMovePiece> pieces = new List<TrainMovePiece>();
    public Vector2 minMaxSpeed = new Vector2(0.8f, 1.5f);
    [SerializeField] private Vector2 minMaxAccel = new Vector2(1.0f, 1.0f);
    [SerializeField] private float drag = 0.5f;
    private float speed = 0.0f;
    public float accel = 0.0f;
    [SerializeField] private float progress = 1.0f;

    public float Speed => speed;

    private void FixedUpdate() 
    {
        // Progress
        speed += Time.fixedDeltaTime * accel;
        speed -= Time.fixedDeltaTime * speed * drag;
        speed = Mathf.Clamp(speed, minMaxSpeed.x, minMaxSpeed.y);
        progress += Time.fixedDeltaTime * speed;

        float clampedProgress = track.GetProgressClamped(progress, out bool pClamped);
        if (pClamped)
            speed = 0;
        progress = clampedProgress;
        float subProgress = progress;

        // Move peieces
        for (int i = 0; i < pieces.Count; i++)
        {
            if (i > 0)
            {
                subProgress -= pieces[i - 1].BackLength;
                subProgress -= pieces[i].ForwardLength;
            }
            pieces[i].SetProgress(track, subProgress);
        }

        // Accel
        
    }
}
