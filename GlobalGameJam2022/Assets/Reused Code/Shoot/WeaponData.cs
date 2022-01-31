using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Custom/New Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    [Title("Display")]
    public string displayName = "Untitled";
    [TextArea] public string description = "";

    [Title("Type")]
    public FireType fireType = FireType.Single;
    public BulletType bulletType =  BulletType.Projectile;

    [ShowIf("@fireType == FireType.Burst")] [Min(2)] public int burstFireCount = 5;
    [Min(1)] public int bulletsPerShot = 1;
 
    [Header("Damage")]
    public int damage = 1;
    [Range(0, 1)] public float critChance01 = 0.1f;
    [HideIf("@critChance01 == 0")] public float critDamageMultiplier = 2;
    
    [ShowIfGroup("Proj", Condition = "@bulletType != BulletType.Raycast")]
    [TitleGroup("Proj/Projectile")] public string projecilePoolKey = "";
    [TitleGroup("Proj/Projectile")] [ShowIf("@projecilePoolKey == \"\"")] public GameObject projectilePrefab = null;
    [TitleGroup("Proj/Projectile")] public BulletCollision bulletCollision = BulletCollision.Destroy;
    [TitleGroup("Proj/Projectile")] public Vector2 lifeTime = new Vector2(4.0f, 4.5f);
    [TitleGroup("Proj/Projectile")] public Vector2 shootForce = new Vector2(5.0f, 5.0f);
    [TitleGroup("Proj/Projectile")] public float bulletGravity = 0.0f;

    [ShowIfGroup("Ray", Condition = "@bulletType == BulletType.Raycast")]
    [TitleGroup("Ray/Raycast")] public float range = 5.0f;
    [TitleGroup("Ray/Raycast")] [AssetsOnly] public GameObject hitFXPrefab = null;
    [ShowIf("@bulletType != BulletType.Projectile")] public LayerMask layerMask = new LayerMask();

    [Title("Stats")]
    public StartType startShootingType = StartType.Instant;
    [ShowIf("@startShootingType == StartType.InstantLimitedByFirerate || fireType == FireType.Auto")] public float secondsBetweenShots = 0.1f;
    [ShowIf("@fireType == FireType.Burst")] public float secondsBetweenBurstShots = 0.1f;
    [ShowIf("@startShootingType == StartType.Charge")] public float chargeSeconds = 0.5f;

    [Header("Spread")]
    public SpreadType spreadType = SpreadType.Circle;
    [ShowIf("@spreadType == SpreadType.Square || spreadType == SpreadType.Ellipse")] public Vector2 spreadVector = new Vector2(0.2f, 0.2f);
    [ShowIf("@spreadType == SpreadType.Square || spreadType == SpreadType.Ellipse")] public Vector2 spreadVectorMax = new Vector2(0.5f, 0.5f);
    [ShowIf("@spreadType == SpreadType.Circle")] public float spreadRadius = 0.2f;
    [ShowIf("@spreadType == SpreadType.Circle")] public float spreadRadiusMax = 0.5f;

    [Space]
    [HideIf("@spreadType == SpreadType.Null"), Range(0, 1)] public float spreadIncrease = 0.4f;
    [HideIf("@spreadType == SpreadType.Null"), Range(0.0001f, 3)] public float spreadDecrease = 0.6f;

    [Header("Force")]
    public Vector3 recoilForce = Vector3.zero;
    public void SetRecoilForce(Vector3 pForce)
    {
        recoilForce = pForce;
    }
    public float hitForce = 8;

    [Title("Ammo")]
    public AmmoType ammoType = AmmoType.Null;
    [ShowIf("@ammoType == AmmoType.Limited")] [MinValue("@clipAmmo")] public int totalAmmo = 12;
    [ShowIf("@ammoType != AmmoType.Null")] [Min(1)] public int clipAmmo = 4;

    [Space]
    [ShowIf("@ammoType != AmmoType.Null")] [Min(0)] public float reloadDelaySeconds = 0.6f;
    [ShowIf("@ammoType != AmmoType.Null")] [Min(0.01f)] public float reloadIntervalSeconds = 0.2f;

    [Title("Audio")]
    public AudioUtil.AudioPiece shotSound = new AudioUtil.AudioPiece();
    public AudioUtil.AudioPiece failedShotSound = new AudioUtil.AudioPiece();

    [Space]
    [ShowIf("@ammoType != AmmoType.Null")] public AudioUtil.AudioPiece reloadSound = new AudioUtil.AudioPiece();
    [ShowIf("@ammoType != AmmoType.Null")] public AudioUtil.AudioPiece outOfAmmoSound = new AudioUtil.AudioPiece();
    [ShowIf("@ammoType != AmmoType.Null")] public AudioUtil.AudioPiece onReloadedSound = new AudioUtil.AudioPiece();

    public enum FireType 
    {
        Single,
        Burst,
        Auto
    }
    
    public enum StartType 
    {
        Instant,
        Charge,
        InstantLimitedByFirerate
    }

    public enum SpreadType 
    {
        Square,
        Circle,
        Ellipse,
        Null
    }

    public enum BulletType 
    {
        Projectile,
        RaycastProjectile,
        Raycast
    }

    public enum AmmoType
    {
        Limited,
        Unlimited,
        Null
    }

    public enum BulletCollision
    {
        Destroy,
        Stick,
        Penetrate,
        Reflect,
        ReflectBack,
        Physics
    }

    public enum BulletExplosion
    {
        Null,
        ExplodeOnHit,
        ExplodeOnDeath
    }

    public enum BulletHoming
    {
        Null,
        HomingDamageables,
        HomingRigidbodies
    }

    public enum BulletHomingMovement
    {
        RotateVelocity,
        AddForce
    }

    public void CopyValues(WeaponData pOther)
    {
        displayName = pOther.displayName;
        description = pOther.description;

        fireType = pOther.fireType;
        bulletType = pOther.bulletType;

        burstFireCount = pOther.burstFireCount;
        bulletsPerShot = pOther.bulletsPerShot;

        damage = pOther.damage;
        critDamageMultiplier = pOther.critDamageMultiplier;
        critChance01 = pOther.critChance01;
        
        projecilePoolKey = pOther.projecilePoolKey;
        projectilePrefab = pOther.projectilePrefab;
        bulletCollision = pOther.bulletCollision;

        lifeTime = pOther.lifeTime;
        shootForce = pOther.shootForce;
        bulletGravity = pOther.bulletGravity;

        range = pOther.range;
        hitFXPrefab = pOther.hitFXPrefab;
        layerMask = pOther.layerMask;

        secondsBetweenShots = pOther.secondsBetweenShots;

        spreadType = pOther.spreadType;
        spreadVector = pOther.spreadVector;
        spreadVectorMax = pOther.spreadVectorMax;
        spreadRadius = pOther.spreadRadius;
        spreadRadiusMax = pOther.spreadRadiusMax;
        
        spreadIncrease = pOther.spreadIncrease;
        spreadDecrease = pOther.spreadDecrease;

        recoilForce = pOther.recoilForce;
        hitForce = pOther.hitForce;

        ammoType = pOther.ammoType;
        totalAmmo = pOther.totalAmmo;
        clipAmmo = pOther.clipAmmo;
        
        reloadDelaySeconds = pOther.reloadDelaySeconds;
        reloadIntervalSeconds = pOther.reloadIntervalSeconds;

        shotSound = pOther.shotSound;
        failedShotSound = pOther.failedShotSound;

        reloadSound = pOther.reloadSound;
        outOfAmmoSound = pOther.outOfAmmoSound;
        onReloadedSound = pOther.onReloadedSound;
    }
}
