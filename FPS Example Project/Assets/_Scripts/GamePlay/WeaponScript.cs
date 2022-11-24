using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponScript : MonoBehaviour
{
    private Dictionary<WeaponType, Ammunition> ammoPouch = new Dictionary<WeaponType, Ammunition>();
    private Animator animator;
    private Weapon activeWeapon;
    private float shootCooldown;
    public LayerMask bulletMask;
    public static event Action<int,int> OnShoot;
    public static event Action<int,int> OnReload;
    public static event Action<int,int> OnWeaponSwtich;
    public static event Action<float> OnHit;

    private void Start()
    {
        // debug only
        Ammunition temp = ScriptableObject.CreateInstance<Ammunition>();
        temp.ammo = 100;
        temp.maxAmmo = 250;
        temp.label = "rifle bullets";
        temp.type = WeaponType.Rifle;
        // debug only
        ammoPouch.Add(temp.type, temp);
        animator = GetComponent<Animator>();
        activeWeapon = GetComponentInChildren<Weapon>();
        shootCooldown = 1 / activeWeapon.fireRate;
        OnWeaponSwtich?.Invoke(activeWeapon.ammoInClip, ammoPouch[activeWeapon.type].ammo);
    }
    private void FixedUpdate()
    {
        shootCooldown -= Time.fixedDeltaTime;
        if(Input.GetKey(KeyCode.Mouse0) && shootCooldown < 0 && animator.Whitelist("Aiming", "Holding"))
        {
            Shoot();
            shootCooldown = 1 / activeWeapon.fireRate;
        }
        if(Input.GetKeyDown(KeyCode.R) && animator.Whitelist("Holding"))
        {
            Reload();
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            animator.SetBool("Aiming", true);
            
            MyCharachterController.Instance.isAiming = true;
        }
        else
        {
            animator.SetBool("Aiming", false);
            MyCharachterController.Instance.isAiming = false;
        }
        if(MyCharachterController.Instance.isSprinting == true)
        {
            animator.SetBool("Sprinting", true);
        }
        else
        {
            animator.SetBool("Sprinting", false);
        }
    }

    private void Shoot()
    {
        if(activeWeapon.ammoInClip > 0)
        {
            animator.SetFloat("FireRate", activeWeapon.fireRate);
            animator.SetTrigger("Shooting");
            AudioUtils.CreateSFX(activeWeapon.fireSFX, activeWeapon.transform.parent.position);
            VisualUtils.CreateVFX(activeWeapon.fireVFX, activeWeapon.muzzleFlashPos, 0.1f);
            if(Physics.Raycast(activeWeapon.muzzleFlashPos.position,activeWeapon.muzzleFlashPos.forward, out RaycastHit hit, activeWeapon.range, bulletMask))
            {
                VisualUtils.CreateImpactVFX(activeWeapon.bulletImpactVFX, hit, 5f);
                OnHit?.Invoke(activeWeapon.dammage);
            }

            activeWeapon.ammoInClip--;

            OnShoot?.Invoke(activeWeapon.ammoInClip, ammoPouch[activeWeapon.type].ammo);
        }
        else
        {
            AudioUtils.CreateSFX(activeWeapon.emptyClipSFX, activeWeapon.transform.parent.position);
        }
    }
    private void Reload()
    {
        int ammoToAdd = activeWeapon.maxAmmoInClip - activeWeapon.ammoInClip;
        if (ammoToAdd > 0 && ammoPouch[activeWeapon.type].ammo >= ammoToAdd)
        {
            animator.SetTrigger("Reloading");
            AudioUtils.CreateSFX(activeWeapon.reloadSFX, activeWeapon.transform.parent.position);
            activeWeapon.ammoInClip += ammoToAdd;
            ammoPouch[activeWeapon.type].ammo -= ammoToAdd;
            OnReload?.Invoke(activeWeapon.ammoInClip, ammoPouch[activeWeapon.type].ammo);
        }
        else if(ammoPouch[activeWeapon.type].ammo < ammoToAdd && ammoPouch[activeWeapon.type].ammo > 0)
        {
            animator.SetTrigger("Reloading");
            AudioUtils.CreateSFX(activeWeapon.reloadSFX, activeWeapon.transform.parent.position);
            activeWeapon.ammoInClip += ammoPouch[activeWeapon.type].ammo;
            ammoPouch[activeWeapon.type].ammo = 0;
            OnReload?.Invoke(activeWeapon.ammoInClip, ammoPouch[activeWeapon.type].ammo);
        }
    }
}

