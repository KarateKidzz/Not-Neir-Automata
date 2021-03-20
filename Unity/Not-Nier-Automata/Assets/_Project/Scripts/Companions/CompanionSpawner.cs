using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class CompanionSpawner : MonoBehaviour
{
    public GameObject[] companionsToSpawn;

    private void Start()
    {
        Pawn pawn = GetComponent<Pawn>();
        Debug.Assert(pawn, "Must be attached to a pawn");

        for (int i = 0; i < companionsToSpawn.Length; i++)
        {
            if (!companionsToSpawn[i])
            {
                continue;
            }

            GameObject spawned = Instantiate(companionsToSpawn[i]);

            if (spawned)
            {
                Companion companion = spawned.GetComponent<Companion>();
                Debug.Assert(companion, "Spawned object must be a companion");

                pawn.AddCompanion(companion);
            }
        }
    }
}
