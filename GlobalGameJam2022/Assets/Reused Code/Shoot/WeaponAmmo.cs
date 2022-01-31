using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Weapon))]
public class WeaponAmmo : MonoBehaviour
{
    private Weapon weapon = null;

    private int totalAmmo;
    private int clipAmmo;
    private Coroutine chargeRoutine = null;

    [SerializeField] private ParticleSystem[] particles = new ParticleSystem[0];

    [FoldoutGroup("Unity Events")] public UnityEvent OnReload;
    [FoldoutGroup("Unity Events")] public UnityEvent OnStartOverHeat;
    [FoldoutGroup("Unity Events")] public UnityEvent OnEndOverHeat;
    [FoldoutGroup("Unity Events")] public UnityEvent OnOutOfAmmo;

    private void Start() 
    {
        weapon = GetComponent<Weapon>();
        weapon.OnShoot.AddListener(OnShoot);

        clipAmmo = weapon.data.clipAmmo;
        totalAmmo = weapon.data.totalAmmo - clipAmmo;
    }

    public void OnShoot()
    {
        if (weapon.data.ammoType == WeaponData.AmmoType.Null)
            return;

        // Ammo
        clipAmmo = Mathf.Max(0, clipAmmo - 1);
        if (clipAmmo == 0)
        {
            weapon.canShoot = false;
            foreach (ParticleSystem p in particles)
            {
                p.Play();
            }

            // Audio
            weapon.data.outOfAmmoSound.Play(weapon.sourcePool.GetSource());

            OnStartOverHeat.Invoke();
        }

        if (weapon.data.ammoType == WeaponData.AmmoType.Limited && totalAmmo <= 0)
        {
            // If totally out of ammo
            if (clipAmmo <= 0)
            {
                // If out of all ammo
                OnOutOfAmmo.Invoke();
            }
        }
        else
        {
            // Recharge
            if (chargeRoutine != null)
                StopCoroutine(chargeRoutine);
            chargeRoutine = StartCoroutine(AmmoRoutine());
        }
    }

    private IEnumerator AmmoRoutine()
    {
        yield return new WaitForSeconds(Mathf.Max(0, weapon.data.reloadDelaySeconds - weapon.data.reloadIntervalSeconds));

        while (clipAmmo < weapon.data.clipAmmo && (totalAmmo > 0 || weapon.data.ammoType == WeaponData.AmmoType.Unlimited))
        {
            yield return new WaitForSeconds(weapon.data.reloadIntervalSeconds);

            // Clip Ammo
            clipAmmo++;

            // Total Ammo
            if (weapon.data.ammoType == WeaponData.AmmoType.Limited)
            {
                totalAmmo--;
            }

            // Audio
            weapon.data.reloadSound.Play(weapon.sourcePool.GetSource());

            OnReload.Invoke();
        }

        if (weapon.canShoot == false)
        {
            weapon.canShoot = true;
            foreach (ParticleSystem p in particles)
            {
                p.Stop();
            }

            // Audio
            weapon.data.onReloadedSound.Play(weapon.sourcePool.GetSource());

            // Events
            OnEndOverHeat.Invoke();
        }
    }

    public void ModifyAmmo(int pValue)
    {
        // Modify Ammo
        totalAmmo += pValue;

        // Check for recharge
        if (totalAmmo > 0 && clipAmmo < weapon.data.clipAmmo)
        {
            if (chargeRoutine != null)
                StopCoroutine(chargeRoutine);
            chargeRoutine = StartCoroutine(AmmoRoutine());
        }
    }
}