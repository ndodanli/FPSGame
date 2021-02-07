using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon;
    private PlayerWeapon currentWeapon;
    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;
    private WeaponGraphics currentGraphics;

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }
    public PlayerWeapon GetCurrentWeapon()
    {
        if (currentWeapon == null) EquipWeapon(primaryWeapon);
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        if (currentWeapon == null) EquipWeapon(primaryWeapon);
        return currentGraphics;
    }
    void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;
        GameObject weaponIns = (GameObject)Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);
        currentGraphics = weaponIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null) Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponIns.name);
        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

}
