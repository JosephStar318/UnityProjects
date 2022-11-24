using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public Image crossHair;
    public Sprite crossNormal;
    public Sprite crossAim;


    private void OnEnable()
    {
        WeaponScript.OnShoot += OnShoot;
        WeaponScript.OnReload += OnReload;
        WeaponScript.OnWeaponSwtich += OnWeaponSwtich;
    }
    private void OnDisable()
    {
        WeaponScript.OnShoot -= OnShoot;
        WeaponScript.OnReload -= OnReload;
        WeaponScript.OnWeaponSwtich -= OnWeaponSwtich;
    }

    private void OnShoot(int ammoInClip, int totalAmmo)
    {
        ammoText.SetText($"{ammoInClip} / {totalAmmo}");
        //might add other effects
    }
    private void OnReload(int ammoInClip, int totalAmmo)
    {
        ammoText.SetText($"{ammoInClip} / {totalAmmo}");
        //might add other effects
    }
    private void OnWeaponSwtich(int ammoInClip, int totalAmmo)
    {
        ammoText.SetText($"{ammoInClip} / {totalAmmo}");
        //might add other effects
    }
    private void Update()
    {
        if (MyCharachterController.Instance.isAiming == true)
        {
            crossHair.sprite = crossAim;
        }
        else
        {
            crossHair.sprite = crossNormal;
        }
    }
}
