using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    [Required] public WeaponData data = null;
    [ShowIf("@muzzlePoints.Length > 1")] [SerializeField] protected MultiMuzzleType multiMuzzleType = MultiMuzzleType.RandomNotOneAfterItself;
    public bool canShoot = true;

    private float nextCanShootTime = 0;
    private int burstFireActiveCount = 0;

    [Header("References")]
    [SerializeField] protected Transform[] muzzlePoints = new Transform[1];
    [SerializeField] private ParticleSystem muzzleFlash = null;
    [ShowIf("@muzzleFlash != null")] [SerializeField] private Vector3 muzzleFlashRelOffset = new Vector3();

    [Space]
    public GameObject sender = null;
    [SerializeField] private Rigidbody recoilBody = null;
    public AudioSourcePool sourcePool = null;

    [FoldoutGroup("Unity Events")] public UnityEvent OnShoot;
    [FoldoutGroup("Unity Events")] public UnityEvent OnFailedShoot;

    private void Start() 
    {
        if (sender == null)
        {
            sender = gameObject;
        }

        Init();
    }

    protected virtual void Init() {}

    [HideInInspector] public bool isShooting {get; private set;} = false;
    public void ShootStart()
    {        
        // If shoot on click
        if (data.startShootingType == WeaponData.StartType.Instant)
        {
            ShootStartDelayed();
        }
        else if (data.startShootingType == WeaponData.StartType.InstantLimitedByFirerate)
        {
            if (nextCanShootTime <= Time.time)
                ShootStartDelayed();
        }
        else if (data.startShootingType == WeaponData.StartType.Charge)
        {
            CancelInvoke(nameof(ShootStartDelayed));
            Invoke(nameof(ShootStartDelayed), data.chargeSeconds);
        }
    }

    private void ShootStartDelayed()
    {
        if (data.fireType == WeaponData.FireType.Single)    // Single
        {
            
        }
        else if (data.fireType == WeaponData.FireType.Burst) // Burst
        {
            // If already shooting don't shoot again
            if (isShooting == true)
                return;
            isShooting = true;
            
            burstFireActiveCount = data.burstFireCount - 1;
        }
        else if (data.fireType == WeaponData.FireType.Auto) // Auto
        {
            isShooting = true;
        }

        Shoot();
    }

    public void ShootEnd()
    {
        if (data.fireType == WeaponData.FireType.Auto)
        {
            isShooting = false;
        }

        if (data.startShootingType == WeaponData.StartType.Charge)
        {
            CancelInvoke(nameof(ShootStartDelayed));
        }
    }

    private void Update()
    {
        if (isShooting == true && nextCanShootTime <= Time.time)
        {
            // Shoot
            Shoot();
        }
        else
        {
            // Spread
            spread01 = Mathf.Max(0, spread01 - (Time.deltaTime * data.spreadDecrease));
        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            // Firerate
            nextCanShootTime = Time.time + data.secondsBetweenShots;

            // If Burst
            if (data.fireType == WeaponData.FireType.Burst)
            {
                burstFireActiveCount--;
                if (burstFireActiveCount < 1)
                {
                    isShooting = false;
                    return;
                }
                else
                {
                    // Overrride Firerate
                    nextCanShootTime = Time.time + data.secondsBetweenBurstShots;
                }
            }

            // Bullet
            Transform muzzle = GetMuzzle();
            SpawnBullet(muzzle);

            // Recoil
            if (recoilBody != null && data.recoilForce != Vector3.zero)
            {
                recoilBody.AddForceAtPosition(muzzle.TransformDirection(data.recoilForce), muzzle.position, ForceMode.VelocityChange);
            }

            // Spread
            spread01 = Mathf.Min(1, spread01 + data.spreadIncrease);

            // Particles
            if (muzzleFlash != null)
            {
                if (muzzleFlash.transform.parent != muzzle)
                {
                    muzzleFlash.transform.SetParent(muzzle);
                    muzzleFlash.transform.localPosition = muzzleFlashRelOffset;
                    muzzleFlash.transform.localRotation = Quaternion.identity;
                }
                muzzleFlash.Play();
            }

            // Audio
            if (sourcePool != null)
                data.shotSound.Play(sourcePool.GetSource());

            // Event
            OnShoot?.Invoke();
        }
        else
        {
            OnShootFailed();
        }
    }

    protected virtual void SpawnBullet(Transform pMuzzle)
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(2.5f, 6.0f, 0.0f, 0.1f);
        for (int i = 0; i < data.bulletsPerShot; i++)
        {
            if (data.bulletType == WeaponData.BulletType.Raycast)
            {
                SpawnRaycast(pMuzzle.position, pMuzzle.forward);
            }
            else
            {
                Vector3 force = Random.Range(data.shootForce.x, data.shootForce.y) * (GetSpreadQuaternion() * pMuzzle.forward);
                SpawnProjectile(pMuzzle.position, force);
            }
        }
    }

    protected virtual void OnShootFailed()
    {
        // Audio
        data.failedShotSound.Play(sourcePool.GetSource());

        // Event
        OnFailedShoot?.Invoke();
    }

    protected virtual void SpawnProjectile(Vector3 pPoint, Vector3 pForce)
    {
        // Spawn projectile
        GameObject projectile;
        if (data.projecilePoolKey != "")
        {
            projectile = ObjectPoolDictionary.dictionary[data.projecilePoolKey].CheckOutObject(true);
            if (projectile == null)
                return;
        }
        else
        {
            projectile = Instantiate(data.projectilePrefab);
        }
        
        projectile.transform.position = pPoint;
        projectile.transform.rotation = Quaternion.LookRotation(pForce);

        Projectile projectileScript = projectile.GetComponentInChildren<Projectile>();
        projectileScript.data = data;

        projectileScript.sender = sender;

        projectileScript.Init(pForce);

        // Audio
        if (sourcePool != null)
            data.shotSound.Play(sourcePool.GetSource());

        // Event
        OnShoot?.Invoke();
    }

    protected virtual void SpawnRaycast(Vector3 pPoint, Vector3 pForward)
    {
        Vector3 dir = GetSpreadQuaternion() * pForward;
        if (Physics.Raycast(pPoint, dir, out RaycastHit hit, data.range, data.layerMask, QueryTriggerInteraction.Ignore)) 
        {
            ApplyParticleFX(hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.collider);

            // push object if rigidbody
            Rigidbody hitRb = hit.collider.attachedRigidbody;
            if (hitRb != null)
                hitRb.AddForceAtPosition(data.hitForce * dir, hit.point);

            // Damage my script if possible
            IDamageable a = hit.collider.GetComponent<IDamageable>();
            if (a != null)
                a.Damage(data.damage, sender, hit.point, hit.normal);
        }
    }

    public virtual void ApplyParticleFX(Vector3 position, Quaternion rotation, Collider attachTo) 
    {
        if (data.hitFXPrefab) 
        {
            GameObject impact = Instantiate(data.hitFXPrefab, position, rotation) as GameObject;
        }
    }

    private int lastMuzzleIndex = 0;
    private bool muzzlePingPongDirection = true;
    private List<int> muzzleIndexList = new List<int>();
    protected Transform GetMuzzle()
    {
        switch (multiMuzzleType)
        {
            case MultiMuzzleType.Loop: // Loop ////////////////////////////////////////
                lastMuzzleIndex++;
                if (lastMuzzleIndex == muzzlePoints.Length)
                    lastMuzzleIndex = 0;
                return muzzlePoints[lastMuzzleIndex];
                
            case MultiMuzzleType.PingPong: // PingPong ////////////////////////////////
                if (muzzlePingPongDirection)
                {
                    lastMuzzleIndex++; // Forward
                    if (lastMuzzleIndex == muzzlePoints.Length - 1)
                        muzzlePingPongDirection = false;
                }
                else
                {
                    lastMuzzleIndex--; // Back
                    if (lastMuzzleIndex == 0)
                        muzzlePingPongDirection = true;
                }
                return muzzlePoints[lastMuzzleIndex];

            case MultiMuzzleType.Random: // Random ////////////////////////////////////
                return muzzlePoints[Random.Range(0, muzzlePoints.Length)];

            case MultiMuzzleType.RandomNotOneAfterItself: // RandomNotOneAfterItself //
                int i = Random.Range(0, muzzlePoints.Length);
                if (i == lastMuzzleIndex)
                {
                    // If is previous offset to new index
                    i += Random.Range(1, muzzlePoints.Length);
                    // If past max, loop back around
                    if (i >= muzzlePoints.Length)
                        i -= muzzlePoints.Length;
                }
                lastMuzzleIndex = i;
                return muzzlePoints[i];

            case MultiMuzzleType.RandomAllOnce: // RandomAllOnce //////////////////////
                if (muzzleIndexList.Count == 0)
                {
                    // If out of indexes, refill
                    for (int z = 0; z < muzzlePoints.Length; z++)
                        muzzleIndexList.Add(z);
                }
                
                // Get random index from list of unused indexes
                int a = Random.Range(0, muzzleIndexList.Count);
                int b = muzzleIndexList[a];
                muzzleIndexList.RemoveAt(a);
                return muzzlePoints[b];

            default: // First Only ////////////////////////////////////////////////////
                return muzzlePoints[0];
        }
    }

    private float spread01 = 0.0f;
    protected Quaternion GetSpreadQuaternion()
    {
        if (data.spreadType == WeaponData.SpreadType.Ellipse) // Ellipse
        {
            Vector2 spread = Vector2.Lerp(data.spreadVector, data.spreadVectorMax, spread01);
            return Quaternion.Euler(GetRandomPointInEllipse(spread.y * 2.0f, spread.x * 2.0f));
        }
        else if (data.spreadType == WeaponData.SpreadType.Circle) // Circle
        {
            float spread = Mathf.Lerp(data.spreadRadius, data.spreadRadiusMax, spread01);
            return Quaternion.Euler(GetRandomPointOnCircle(spread));
        }
        else if (data.spreadType == WeaponData.SpreadType.Square) // Square
        {
            return Quaternion.Euler(Random.Range(-data.spreadVector.y, data.spreadVector.y), Random.Range(-data.spreadVector.x, data.spreadVector.x), 0);
        }

        return Quaternion.identity; // Null
    }

    private Vector2 GetRandomPointOnCircle(float pRadius) 
    {
        return GetRandomPointInEllipse(pRadius, pRadius);
    }

    private Vector2 GetRandomPointInEllipse(float ellipse_width, float ellipse_height)
    {
        float t = 2 * Mathf.PI * Random.value;
        float u = Random.value + Random.value;
        float r;
        if (u > 1)
        {
            r = 2 - u;
        }
        else 
        {
            r = u;
        }
        return new Vector2(ellipse_width * r * Mathf.Cos(t) / 2, 
                           ellipse_height * r * Mathf.Sin(t) / 2);
    }

    private void OnDrawGizmos() 
    {
#if UNITY_EDITOR
        if (data == null)
            return;
            
        foreach (Transform m in muzzlePoints)
        {
            if (m == null)
                continue;

            Handles.matrix = transform.localToWorldMatrix;
            Vector3 localForward = transform.InverseTransformVector(m.forward);

            Handles.color = Color.cyan;
            if (data.spreadType == WeaponData.SpreadType.Circle) // Circle
            {
                Handles.DrawWireDisc(localForward * 1.0f, localForward, data.spreadRadius * 0.01f);

                Handles.color = Color.blue;
                Handles.DrawWireDisc(localForward * 1.0f, localForward, data.spreadRadiusMax * 0.01f);

                if (Application.isPlaying)
                {
                    Handles.color = Color.green;
                    float spread = Mathf.Lerp(data.spreadRadius, data.spreadRadiusMax, spread01);
                    Handles.DrawWireDisc(localForward * 1.0f, localForward, spread * 0.01f);
                }
            }
            else if (data.spreadType != WeaponData.SpreadType.Null) // Square || Ellipse
            {
                Handles.DrawWireCube(localForward * 1.0f, new Vector3(data.spreadVector.x * 0.04f, data.spreadVector.y * 0.04f, 0.0f));

                Handles.color = Color.blue;
                Handles.DrawWireCube(localForward * 1.0f, new Vector3(data.spreadVectorMax.x * 0.04f, data.spreadVectorMax.y * 0.04f, 0.0f));

                if (Application.isPlaying)
                {
                    Handles.color = Color.green;
                    Vector2 spread = Vector2.Lerp(data.spreadVector, data.spreadVectorMax, spread01);
                    Handles.DrawWireCube(localForward * 1.0f, new Vector3(spread.x * 0.04f, spread.y * 0.04f, 0.0f));
                }
            }
        }
#endif
    }

    public enum MultiMuzzleType
    {
        FirstOnly,
        Loop,
        PingPong,
        Random,
        RandomNotOneAfterItself,
        RandomAllOnce
    }
}
