using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainFrontPeice : PlayerMountTrigger
{
    [Header("Front")]
    [SerializeField] private TrainMovement movement = null;
    [SerializeField] private BarValue forwardBar = null;
    [SerializeField] private BarValue reverseBar = null;
    private float lowerTime = 0.0f;
    private float input = 0.0f;
    private bool isMounted = false;

    public override void OnMove(Vector2 pInput)
    {
        input = pInput.y;
    }
       
    private float lastAccel = 0.0f;
    private void Update() 
    {
        movement.accel += input * Time.deltaTime;
        movement.accel = Mathf.Clamp(movement.accel, -1.0f, 1.0f);
        
        if (!isMounted && Mathf.Abs(movement.accel) > 0.65f && Time.time > lowerTime)
        {
            movement.accel -= Time.deltaTime * 0.05f * (movement.accel > 0.0f ? 1 : -1);
        }

        if (movement.accel != lastAccel)
        {
            if (movement.accel > 0)
            {
                forwardBar.SetValue(Mathf.Clamp01(movement.accel));
                reverseBar.SetValue(0.0f);
            }
            else
            {
                forwardBar.SetValue(0.0f);
                reverseBar.SetValue(Mathf.Clamp01(-movement.accel));
            }
        }
        lastAccel = movement.accel;
    }

    public override void OnEnter()
    {
        isMounted = true;

        base.OnEnter();
    }

    public override void OnExit()
    {
        isMounted = false;
        lowerTime = Time.time + 5.0f;

        base.OnExit();
    }
}
