using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : PoolElement
{
    [Required] public WeaponData data = null;

    private new Rigidbody rigidbody = null;
    [SerializeField] private new Collider collider = null;
    public new ProjectileSFX audio = null;
    public GameObject sender = null;

    private bool canDamage = true;
    private bool activeSelf = true;
    private int currentFrame = 0;
    private int lastHitFrame = 0;
    private Collider lastHitCollider = null;

    [Header("Impact")]
    public ParticleSystem impactParticle = null;

    [Header("Floating Numbers")]
    [ColorPalette("UI")] [SerializeField] private Color hitColor = new Color(1, 0, 0, 1);
    [ColorPalette("UI")] [SerializeField] private Color critColor = new Color(1, 1, 0, 1);

    private Vector3 startPos = new Vector3();
    private Vector3 previousPosition = new Vector3();

    [SerializeField] private float spawnOffsetZ = 0;

    private void Awake() 
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void ReturnToPool()
    {
        if (lastHitFrame != currentFrame) // if death of natural cause (lifeTime) and not because of a hit
        {
            PlayParticle(impactParticle, transform.position);
        }

        activeSelf = false;

        CancelInvoke();
        base.ReturnToPool();
    }

    public override void OnExitPool()
    {
        currentFrame = 0;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        canDamage = true;
        lastHitCollider = null;
        collider.enabled = false;
        activeSelf = true;

        base.OnExitPool();
    }

    public void Init(Vector3 pForce)
    {
        rigidbody.velocity = pForce;
        transform.position += transform.forward * spawnOffsetZ;

        startPos = transform.position;
        previousPosition = transform.position;

        Invoke(nameof(ReturnToPool), Random.Range(data.lifeTime.x, data.lifeTime.y));
    }

    private void FixedUpdate() 
    {
        if (activeSelf == false)
            return;

        bool updateRot = false;
        if (data.bulletGravity > 0)
        {
            rigidbody.AddForce(Vector3.down * data.bulletGravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
            updateRot = true;
        }

        if (updateRot)
        {
            transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
        }
    }

    private void Update() 
    {
        if (activeSelf == false)
            return;

        currentFrame++; // Used to ignore collision on first two frames

        if (currentFrame >= 1 && data.bulletType == WeaponData.BulletType.RaycastProjectile) // Raycast Projectile
        {
            if (canDamage && Physics.Linecast(previousPosition, transform.position, out RaycastHit hit, data.layerMask, QueryTriggerInteraction.Ignore))
            {
                if (data.bulletCollision != WeaponData.BulletCollision.Penetrate)
                    transform.position = hit.point;
                DoHitOther(hit.collider, hit.point);
            }
        }

        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (activeSelf == false)
            return;

        Debug.Log("Hit " + other.name);

        DoHitOther(other, transform.position);
    }

#region Hit/Damage
    private void DoHitOther(Collider other, Vector3 point)
    {
        if (canDamage == false || currentFrame < 1 || other.isTrigger || other == lastHitCollider || other.gameObject == sender)
            return;

        DamageOther(other, point);

        lastHitFrame = currentFrame;
        lastHitCollider = other;

        PlayParticle(impactParticle, point);
        DoBulletCollision(other);
    }

    private void DamageOther(Collider other, Vector3 point)
    {
        // Debug.Log("[Projectile.cs] DamageOther(" + other.name + ")", other);
        Rigidbody otherRb = other.GetComponentInParent<Rigidbody>();
        if (otherRb != null)
            otherRb.AddForceAtPosition(rigidbody.velocity.normalized * data.hitForce, point);
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (Random.value > data.critChance01)
            {
                damageable.Damage(data.damage, sender, transform.position, rigidbody.velocity);
            }
            else
            {
                damageable.Damage(Mathf.RoundToInt(data.critDamageMultiplier * data.damage), sender, transform.position, rigidbody.velocity, critColor);
            }
        }

        // Audio
        if (audio != null)
            audio.OnCollision();
    }
#endregion

#region Collision
    protected void DoBulletCollision(Collider other)
    {
        switch (data.bulletCollision)
        {
            case WeaponData.BulletCollision.Stick:
                rigidbody.isKinematic = true;
                if (other.gameObject.isStatic == false)
                    transform.SetParent(other.transform);
                canDamage = false;
                activeSelf = false;
                break;

            case WeaponData.BulletCollision.Penetrate:
                if (other.gameObject.isStatic)
                {
                    rigidbody.isKinematic = true;
                    canDamage = false;
                    ReturnToPool();
                }
                break;
            
            case WeaponData.BulletCollision.Reflect:
                Debug.LogError("[Projectile.cs] BulletCollision.Reflect not implemented", gameObject);
                break;
            
            case WeaponData.BulletCollision.ReflectBack:
                rigidbody.velocity = -rigidbody.velocity;
                transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
                break;
            
            case WeaponData.BulletCollision.Physics:
                rigidbody.useGravity = true;
                collider.enabled = true;
                activeSelf = false;
                transform.position += rigidbody.velocity.normalized * -0.25f;
                break;
            
            default: // Destroy
                rigidbody.isKinematic = true;
                canDamage = false;
                ReturnToPool();
                break;
        }
    }
#endregion

#region Explosive
//     public virtual void DoExplosion(Vector3 pPoint) 
//     {
//         PlayParticle(explosiveParticle, pPoint); // Moves explosive & particle to point

//         explosive.ExplosionRadius = data.explosionRadius;
//         explosive.ExplosionDamage = data.explosionDamage;
//         explosive.ExplosionForce = data.explosionForce;
//         explosive.ExplosiveUpwardsModifier = data.explosiveUpwardsModifier;

//         explosive.DoExplosion();

//         // Audio
//         if (audio != null)
//             audio.OnExplode();
//     }
#endregion

    private void PlayParticle(ParticleSystem pParticle, Vector3 pPosition)
    {
        if (pParticle != null)
        {
            pParticle.gameObject.SetActive(true);
            pParticle.Play();
            pParticle.transform.position = pPosition;
            pParticle.transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Vector3 endPos = transform.position + (transform.forward * -spawnOffsetZ);
        Gizmos.DrawLine(transform.position, endPos);
        Gizmos.DrawWireSphere(endPos, 0.01f);
    }
}