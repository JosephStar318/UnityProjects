using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject fireVFX;
    public GameObject bulletImpactVFX;
    public AudioClip fireSFX;
    public AudioClip emptyClipSFX;
    public AudioClip reloadSFX;

    public Transform muzzleFlashPos;
    public WeaponType type;
    public float accuracy;
    public float fireRate;
    public float dammage;
    public float recoil;
    public float range;
    public int ammoInClip;
    public int maxAmmoInClip;
    //burst mode...

}
public enum WeaponType
{
    Pistol = 0,
    Rifle,
    Sniper,
    Shotgun
}
