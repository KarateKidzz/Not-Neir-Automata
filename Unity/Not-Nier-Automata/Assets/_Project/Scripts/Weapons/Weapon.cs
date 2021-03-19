using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Weapon : MonoBehaviour
{
    protected WeaponUser weaponUser;

    public WeaponUser WeaponUser => weaponUser;

    public string weaponName = "Weapon";

    public int damage = 10;

    protected bool autoFire;

    public bool useCameraForAim;

    [EventRef]
    public string attackSoundEvent;

    CameraManager cameraManager;

    protected virtual void Start()
    {
        if (useCameraForAim)
        {
            CacheCamera();
        }
    }

    void CacheCamera()
    {
        PlayerController playerController = GameManager.Instance.PlayerController;

        if (playerController)
        {
            cameraManager = playerController.CameraManager;
        }
    }

    public virtual void Equip(WeaponUser user, bool useCamera = false)
    {
        if (weaponUser)
        {
            Debug.LogWarning("There is already a weapon user for this weapon");
            return;
        }

        Debug.Log($"{user.gameObject.name} equipped {weaponName}");
        weaponUser = user;
        useCameraForAim = useCamera;
        if (useCamera)
        {
            CacheCamera();
        }
    }

    public virtual void UnEquip()
    {

    }

    public virtual void StartAttack()
    {

    }

    public virtual void FinishAttack()
    {
        autoFire = false;
    }

    public virtual void AutoAttack()
    {
        if (CanSingleFire())
        {
            StartAttack();
        }

        autoFire = true;
    }

    protected virtual bool CanSingleFire()
    {
        return true;
    }

    protected void SpawnProjectile(GameObject projectilePrefab)
    {
        if (!string.IsNullOrEmpty(attackSoundEvent))
        {
            RuntimeManager.PlayOneShot(attackSoundEvent, transform.position);
        }

        GameObject spawned = Instantiate(projectilePrefab);

        Projectile spawnedProjectile = spawned.GetComponent<Projectile>();
        Debug.Assert(spawnedProjectile);

        Vector3 direction;

        if (useCameraForAim && cameraManager)
        {
            direction = cameraManager.cameraBrain.transform.forward;
        }
        else
        {
            direction = transform.forward;
        }

        spawnedProjectile.Fire(this, direction);
    }
}
