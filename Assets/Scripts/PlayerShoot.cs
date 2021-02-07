using UnityEngine.Networking;
using UnityEngine;
using System;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{


    [SerializeField]
    private Camera cam;
    private const string PLAYER_TAG = "Player";

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;


    [SerializeField]
    private LayerMask mask;
    private void Start()
    {
        if (cam == null)
        {
            Debug.Log("No camera referenced");
            this.enabled = false;
        }
        currentWeapon = new PlayerWeapon();
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }
    //call on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();

    }
    //is called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }
    //its called on the server when we hit something, takes in the hit point and the normal of the surface
    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }
    //its called on all clients here we can spawn in cool effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }
    [Client]    //only called on client
    private void Shoot()
    {
        if (!isLocalPlayer) return;
        //we are shootingi call the OnShoot method on the server
        CmdOnShoot();
        Debug.Log("shoot");
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if (hit.collider.tag.Equals(PLAYER_TAG))
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);
            }
            //if we hit something, call the OnHit method on the server
            CmdOnHit(hit.point, hit.normal);
        }
    }

    [Command]   //only called on server
    void CmdPlayerShot(string playerId, int damage)
    {
        Debug.Log(playerId + "has been shot.");
        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);

    }

}
