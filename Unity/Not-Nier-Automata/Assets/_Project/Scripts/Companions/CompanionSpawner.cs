using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class CompanionSpawner : Actor, IBeginPlay, IInitialize
{
    public GameObject[] companionsToSpawn;

    List<Companion> spawnedCompanions = new List<Companion>();

    Pawn pawn;

    public void Initialize()
    {
        pawn = GetComponent<Pawn>();

        if (!pawn)
        {
            Debug.LogError("No pawn!");
            return;
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
                spawnedCompanions.Add(spawnedCompanion); 
            }
        }
    }

    public void BeginPlay()
    {
        if (pawn)
        {
            foreach (Companion companion in spawnedCompanions)
            {
                companion.Initialize();
                pawn.AddCompanion(companion);
            }
        }    
    }
}
