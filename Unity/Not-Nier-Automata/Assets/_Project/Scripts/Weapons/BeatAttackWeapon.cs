using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public enum BeatFireRate
{
    StandardSpeed,
    DoubleSpeed,
    TripletSpeed
}

public class BeatAttackWeapon : AutoAttackWeapon
{
    bool readyForFire;
    public BeatFireRate fireRate;


    public override void Equip(WeaponUser user, bool useCamera = false)
    {
        base.Equip(user, useCamera);

        BeatCallbacks.OnBeatChange += OnBeat;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        BeatCallbacks.OnBeatChange -= OnBeat;
    }

    public override void StartAttack(Vector3 direction = default)
    {
        base.StartAttack();

        readyForFire = false;
    }

    public override void FinishAttack()
    {
        base.FinishAttack();

        readyForFire = false;
    }

    protected override bool ShouldDoNextAutoAttack()
    {
        return readyForFire;
    }

    protected override bool CanSingleFire()
    {
        if (!GameManager.IsValid() || !GameManager.Instance.GetCurrentGameMode() || !GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<CombatManager>() ||
            !GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<CombatManager>().IsInCombat)
        {
            return true;
        }

        return false;
    }

    void OnBeat(EventInstance eventInstance, TIMELINE_BEAT_PROPERTIES beats)
    {
        if (this)
        {
            readyForFire = true;
            StartCoroutine(FireRate(beats));
        }
    }

    IEnumerator FireRate(TIMELINE_BEAT_PROPERTIES beats)
    {
        float bpm = beats.tempo;
        float millisecondsBetweenBeats = 60 / bpm;

        if (fireRate == BeatFireRate.StandardSpeed)
        {
            yield return null;
        }

        if (fireRate == BeatFireRate.DoubleSpeed)
        {
            float millisecondsDoubleSpeed = millisecondsBetweenBeats / 2;
            yield return new WaitForSeconds(millisecondsDoubleSpeed);
            readyForFire = true;
        }

        if (fireRate == BeatFireRate.TripletSpeed)
        {
            float millisecondsTripleSpeed = millisecondsBetweenBeats / 3;
            yield return new WaitForSeconds(millisecondsTripleSpeed);
            readyForFire = true;
            yield return new WaitForSeconds(millisecondsTripleSpeed);
            readyForFire = true;
        }

        yield return null;
    }

    protected override Projectile SpawnProjectile(GameObject projectilePrefab)
    {
        Projectile spawnedProjectile =  base.SpawnProjectile(projectilePrefab);

        if (fireRate == BeatFireRate.DoubleSpeed)
        {
            spawnedProjectile.transform.localScale /= 2;
        }
        else if (fireRate == BeatFireRate.TripletSpeed)
        {
            spawnedProjectile.transform.localScale /= 3;
        }

        return spawnedProjectile;
    }
}
