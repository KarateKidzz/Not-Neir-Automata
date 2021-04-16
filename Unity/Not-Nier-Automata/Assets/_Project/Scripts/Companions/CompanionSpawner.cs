using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class CompanionSpawner : Actor, IBeginPlay
{
    public GameObject[] companionsToSpawn;

    public void BeginPlay()
    {
        Pawn pawn = GetComponent<Pawn>();

        if (!pawn)
        {
            Debug.LogError("No pawn!");
        }

        for (int i = 0; i < companionsToSpawn.Length; i++)
        {
            if (!companionsToSpawn[i])
            {
                continue;
            }

            Debug.Log("[Companion Spawner] Spawning companion");

            GameObject spawned = Instantiate(companionsToSpawn[i]);

            if (spawned)
            {
                Companion spawnedCompanion = spawned.GetComponent<Companion>();
                Debug.Assert(spawnedCompanion, "Spawned object must be a companion");

                pawn.AddCompanion(spawnedCompanion);
            }
        }
    }
}
