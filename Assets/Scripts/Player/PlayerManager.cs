using System;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    private CanvasManager canvas;
    public Camera gameCamera;
    public Transform projectilePool;
    public Transform particlePool;

    public GameObject player;
    public WeaponSO defaultWeapon;
    public Transform spawnPoint;

    private PlayerHealth health;
    private PlayerMovement movement;
    private PlayerShoot shoot;
    private PlayerAim aim;
    private PlayerWeapon weapon;

    public void Setup(ref CanvasManager canvasManager)
    {   
        canvas = canvasManager;

        //Grab Player Scripts
        health = player.GetComponent<PlayerHealth>();
        movement = player.GetComponent<PlayerMovement>();
        shoot = player.GetComponentInChildren<PlayerShoot>();
        aim = player.GetComponentInChildren<PlayerAim>();
        weapon = player.GetComponentInChildren<PlayerWeapon>();

        //Pass Values
        health.canvasManager = canvas;
        shoot.canvasManager = canvas;
        aim.canvasManager = canvas;
        aim.gameCamera = gameCamera;

        ResetPlayer();
    }

    public void EquipWeapon(WeaponSO weaponType)
    {
        //Disable shooting while equip weapon occurs
        shoot.canShoot = false;

        //Bullet
        shoot.projectilePool = projectilePool;
        shoot.particlePool = particlePool;
        shoot.bulletPrefab = weaponType.bullet;
        shoot.bulletSpeed = weaponType.bulletSpeed;
        shoot.bulletDuration = weaponType.bulletDuration;
        shoot.bulletDamage = weaponType.bulletDamage;
        shoot.bulletColor = weaponType.bulletColor;
        shoot.reloadDelay = weaponType.reloadDelay;
        shoot.maxAmmo = weaponType.ammo;
        shoot.ammo = weaponType.ammo;

        //Weapon
        weapon.EquipWeapon(weaponType.gun);

        shoot.bulletSpawnLocation = weapon.bulletSpawn; //Updated after EquipWeapon()

        //Canvas Weapon UI
        canvas.SetWeaponUI(weaponType.weaponName, weaponType.ammo, weaponType.reloadDelay);
    }

    public void ResetPlayer()
    {
        //Default weapon kit
        EquipWeapon(defaultWeapon);
        weapon.DisableGun();

        movement.SetPosition(spawnPoint.position);

        health.Setup();

        //Other
    }

    public void StartGame()
    {
        shoot.canShoot = true;
        movement.canMove = true;
        weapon.EnableGun();
    }
    
    public void EndGame()
    {
        shoot.canShoot = false;
        movement.canMove = false;
    }
}
