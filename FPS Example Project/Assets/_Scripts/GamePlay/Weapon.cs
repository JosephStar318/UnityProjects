using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public ParticleSystem fireVFX;
    public AudioClip fireSFX;

    public WeaponType type;
    public float accuracy;
    public float fireRate;
    public float dammage;
    public float recoil;
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
