using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : PlayerComponent
{
    [Header("References")]
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private OliverLoescher.OnGround ground = null;

    [Header("Movement")]
    [Min(0)] public float acceleration = 5.0f;
    [SerializeField, Min(0)] private float drag = 5.0f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField, Min(0)] private float gravity = 9.81f;

    void FixedUpdate()
    {
        if (characterController == null)
            return;

        velocity += input.moveInputVector3 * acceleration * Time.fixedDeltaTime;
        velocity += Horizontalize(velocity) * -drag * Time.fixedDeltaTime;

        velocity.y += -gravity * Time.fixedDeltaTime;
        if (ground.isGrounded)
            velocity.y = 0.0f;

        characterController.Move(velocity * Time.fixedDeltaTime);

        if (OliverLoescher.FuncUtil.Horizontalize(velocity) != Vector3.zero)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(OliverLoescher.FuncUtil.Horizontalize(velocity)), Time.fixedDeltaTime * 25.0f);
    }

    public void AddForce(Vector3 pForce)
    {
        velocity += pForce;
    }

    private Vector3 Horizontalize(Vector3 v) => new Vector3(v.x, 0.0f, v.z); 
}
