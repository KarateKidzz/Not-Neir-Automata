using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class BeatAttackWeapon : AutoAttackWeapon
{
    bool readyForFire;

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

    public override void StartAttack()
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
        return false;
    }

    void OnBeat(EventInstance eventInstance, TIMELINE_BEAT_PROPERTIES beats)
    {
        readyForFire = true;
    }
}
