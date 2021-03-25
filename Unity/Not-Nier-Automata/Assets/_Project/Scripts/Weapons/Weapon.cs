using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Weapon : MonoBehaviour
{
    protected WeaponUser weaponUser;

    public WeaponUser WeaponUser => weaponUser;

    public string weaponName = "Weapon";

    public int damage = 10;

    protected bool autoFire;

    public bool AutoFire => autoFire;

    public bool useCameraForAim;

    protected Vector3 rangedAttackDirection;

    [EventRef]
    public string attackSoundEvent;

    [EventRef]
    public string cooldownSoundEvent;

    protected CameraManager cameraManager;

    protected EventDescription attackSoundDescription;

    protected virtual void Start()
    {
        if (useCameraForAim)
        {
            CacheCamera();
        }

        if (!string.IsNullOrEmpty(attackSoundEvent))
        {
            attackSoundDescription = RuntimeManager.GetEventDescription(attackSoundEvent);

            if (attackSoundDescription.isValid())
            {
                attackSoundDescription.loadSampleData();
                RuntimeManager.StudioSystem.update();
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (attackSoundDescription.isValid())
        {
            attackSoundDescription.unloadSampleData();
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

    public virtual void StartAttack(Vector3 direction = new Vector3())
    {
        rangedAttackDirection = direction;
    }

    public virtual void FinishAttack()
    {
        if (!string.IsNullOrEmpty(cooldownSoundEvent))
        {
            RuntimeManager.PlayOneShotAttached(cooldownSoundEvent, gameObject);
        }
        autoFire = false;
    }

    public virtual void AutoAttack(Vector3 direction = new Vector3())
    {
        rangedAttackDirection = direction;

        if (CanSingleFire())
        {
            StartAttack();
        }

        autoFire = true;
    }

    protected virtual bool CanSingleFire()
    {
        rangedAttackDirection = Vector3.zero;

        return true;
    }

    protected virtual Projectile SpawnProjectile(GameObject projectilePrefab)
    {
        if (attackSoundDescription.isValid())
        {
            FMOD.RESULT createResult = attackSoundDescription.createInstance(out EventInstance instance);

            if (createResult == FMOD.RESULT.OK)
            {
                instance.set3DAttributes(transform.To3DAttributes());
                instance.start();
                instance.release();
            }            
        }

        GameObject spawned = Instantiate(projectilePrefab);

        Projectile spawnedProjectile = spawned.GetComponent<Projectile>();
        Debug.Assert(spawnedProjectile);

        Vector3 direction;

        if (useCameraForAim && cameraManager)
        {
            direction = cameraManager.cameraBrain.transform.forward;
        }
        else if (rangedAttackDirection != Vector3.zero)
        {
            direction = rangedAttackDirection;
        }
        else
        {
            direction = transform.forward;
        }

        spawnedProjectile.Fire(this, direction);

        return spawnedProjectile;
    }
}
