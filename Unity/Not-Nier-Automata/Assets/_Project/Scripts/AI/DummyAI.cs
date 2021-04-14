using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

/// <summary>
/// Stands still and just fires some projectiles on bar
/// </summary>
public class DummyAI : Controller
{
    [Range(0f, 100f)]
    float attackChance = 25f;

    int previousBar;

    CombatManager combatManager;

    public override void ActivateController()
    {
        base.ActivateController();

        BeatCallbacks.OnBeatChange += OnBeat;

        combatManager = GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<CombatManager>();
    }

    public override void DisableController()
    {
        base.DisableController();

        BeatCallbacks.OnBeatChange -= OnBeat;
    }

    protected virtual void OnBeat(EventInstance instance, TIMELINE_BEAT_PROPERTIES beat)
    {
        if (!combatManager || !combatManager.IsInCombat)
        {
            return;
        }

        if (PossessedPawn && PossessedPawn.Faction)
        {
            if (previousBar == beat.bar)
            {
                return;
            }

            previousBar = beat.bar;

            WeaponUser weaponUser = PossessedPawn.WeaponUser;

            if (weaponUser)
            {
                float randomNumber = Random.Range(0f, 100f);

                if (attackChance > randomNumber)
                {
                    List<Pawn> allEnemies = new List<Pawn>();

                    foreach(Pawn pawn in GameManager.Instance.AllPawns)
                    {
                        if (!pawn.Faction)
                        {
                            continue;
                        }

                        if (PossessedPawn.Faction.AreEnemies(pawn.Faction))
                        {
                            allEnemies.Add(pawn);
                        }
                    }

                    if (allEnemies.Count > 0)
                    {
                        Pawn closestEnemy = null;
                        float closestDistance = 999999f;

                        foreach(Pawn pawn in allEnemies)
                        {
                            float distance = Vector3.Distance(PossessedPawn.transform.position, pawn.transform.position);

                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestEnemy = pawn;
                            }
                        }

                        Vector3 direction = PossessedPawn.transform.forward;

                        if (closestEnemy)
                        {
                            direction = closestEnemy.transform.position - PossessedPawn.transform.position;
                        }

                        weaponUser.Attack(direction);
                    }
                }
            }
        }
    }
}
