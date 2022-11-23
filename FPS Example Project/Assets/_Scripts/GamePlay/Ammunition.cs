using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Ammo")]
public class Ammunition : ScriptableObject
{
    public WeaponType type;
    public string label;
    public int ammo;
    public int maxAmmo;
}


