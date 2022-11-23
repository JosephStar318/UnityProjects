using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponScript : MonoBehaviour
{
    
    public MyCharachterController myCharachterCtrl;
    public bool isAiming;
    public bool isReloading;

    private Dictionary<WeaponType, Ammunition> ammoPouch = new Dictionary<WeaponType, Ammunition>();
    private Animator animator;
    private Weapon activeWeapon;
    private float shootCooldown;

    public static event Action<int,int> OnShoot;

    private void Start()
    {
        // debug only
        Ammunition temp = ScriptableObject.CreateInstance<Ammunition>();
        temp.ammo = 250;
        temp.maxAmmo = 250;
        temp.label = "rifle bullets";
        temp.type = WeaponType.Rifle;
        // debug only
        ammoPouch.Add(temp.type, temp);
        animator = GetComponent<Animator>();
        activeWeapon = GetComponentInChildren<Weapon>();
        shootCooldown = 1 / activeWeapon.fireRate;
    }
    private void FixedUpdate()
    {
        shootCooldown -= Time.fixedDeltaTime;
        if(Input.GetKey(KeyCode.Mouse0) && shootCooldown < 0)
        {
            Shoot();
            shootCooldown = 1 / activeWeapon.fireRate;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if(activeWeapon.ammoInClip > 0)
        {
            animator.SetFloat("FireRate", activeWeapon.fireRate);
            animator.SetTrigger("Shooting");
            //activeWeapon.fireVFX?.Play();
            AudioUtils.CreateSFX(activeWeapon.fireSFX, activeWeapon.transform.parent.position);

            activeWeapon.ammoInClip--;

            OnShoot?.Invoke(activeWeapon.ammoInClip, ammoPouch[activeWeapon.type].ammo);
        }
        else
        {
            Reload();
        }
    }
    private void Reload()
    {
        int ammoToAdd = activeWeapon.maxAmmoInClip - activeWeapon.ammoInClip;
        if (ammoToAdd > 0 && ammoPouch[activeWeapon.type].ammo >= ammoToAdd)
        {
            activeWeapon.ammoInClip += ammoToAdd;
            ammoPouch[activeWeapon.type].ammo -= ammoToAdd;
        }
        else if(ammoToAdd > 0)
        {
            activeWeapon.ammoInClip += ammoPouch[activeWeapon.type].ammo;
            ammoPouch[activeWeapon.type].ammo = 0;
        }
    }
}
