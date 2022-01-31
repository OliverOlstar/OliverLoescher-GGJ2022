using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainGunPiece : PlayerMountTrigger
{
    [SerializeField] private Transform model = null;
    [SerializeField] private Weapon weapon = null;

    [Space]
    [SerializeField] private float angleLimit = 90.0f;
    [SerializeField] private float dampening = 10.0f;
    private Vector3 shootDir = Vector3.forward;

    public override void OnLook(Vector2 pDirection)
    {
        shootDir = new Vector3(pDirection.x, 0.0f, pDirection.y);
    }

    public override void OnTrigger(bool pBool)
    {
        if (pBool)
            weapon.ShootStart();
        else
            weapon.ShootEnd();
    }

    private void FixedUpdate() 
    {
        Quaternion initialQ = model.localRotation;

        model.rotation = Quaternion.LookRotation(shootDir);

        if (angleLimit < 360)
        {
            float angle = SafeAngle(model.localEulerAngles.y);
            angle = Mathf.Clamp(angle, -angleLimit * 0.5f, angleLimit * 0.5f);
            angle = Mathf.Lerp(SafeAngle(initialQ.eulerAngles.y), angle, Time.fixedDeltaTime * dampening);
            model.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }
        else
        {
            model.localRotation = Quaternion.Lerp(initialQ, model.localRotation, Time.fixedDeltaTime * dampening);
        }
    }

    private float SafeAngle(float pAngle)
    {
        if (pAngle > 180)
        {
            return pAngle - 360;
        }
        else
        {
            return pAngle;
        }
    }
}
