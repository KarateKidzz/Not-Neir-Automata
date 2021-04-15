using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class CompanionSpawner : MonoBehaviour
{
    public GameObject[] companionsToSpawn;

    Pawn pawn;
    Companion spawnedCompanion;

    private void Awake()
    {
        pawn = GetComponent<Pawn>();
        Debug.Assert(pawn, "Must be attached to a pawn");
        pawn.OnPossess += OnPawnPossessed;
    }

    private void Start()
    {
        Controller controller = pawn.GetController();

        if (controller)
        {
            pawn.OnPossess -= OnPawnPossessed;
            OnPawnPossessed(controller);
        }
    }

    void OnPawnPossessed(Controller controller)
    {
        pawn.OnPossess -= OnPawnPossessed;

        if (spawnedCompanion)
        {
            Debug.LogWarning("[Companion Spawner] Companion already spawned. Ignoring");
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
                spawnedCompanion = spawned.GetComponent<Companion>();
                Debug.Assert(spawnedCompanion, "Spawned object must be a companion");

                pawn.AddCompanion(spawnedCompanion);
            }
        }
    }
}
