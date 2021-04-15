using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class CompanionSpawner : MonoBehaviour
{
    public GameObject[] companionsToSpawn;

    Pawn pawn;

    private void Awake()
    {
        pawn = GetComponent<Pawn>();
        Debug.Assert(pawn, "Must be attached to a pawn");
        pawn.OnPossess += OnPawnPossessed;
    }

    void OnPawnPossessed(Controller controller)
    {
        pawn.OnPossess -= OnPawnPossessed;

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
                Companion companion = spawned.GetComponent<Companion>();
                Debug.Assert(companion, "Spawned object must be a companion");

                pawn.AddCompanion(companion);
            }
        }
    }
}
