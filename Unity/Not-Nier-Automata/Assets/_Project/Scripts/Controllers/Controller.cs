using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : Actor, IInitialize, IEndPlay
{
    [SerializeField, ReadOnly]
    protected Pawn possessedPawn;

    public Pawn PossessedPawn => possessedPawn;

    public bool Activated { get; protected set; }

    public void Initialize()
    {
        GameManager.Instance.AllControllers.Add(this);
        ActivateController();
    }

    public void EndPlay(EndPlayModeReason Reason)
    {
        if (Reason != EndPlayModeReason.ApplicationQuit)
        {
            GameManager.Instance.AllControllers.Remove(this);
            DisableController();
        }
    }

    public virtual void ActivateController()
    {
        Activated = true;
    }

    public virtual void DisableController()
    {
        Unpossess();
    }

    public virtual void Possess(Pawn pawnToPossess)
    {
        Unpossess();

        possessedPawn = pawnToPossess;
        possessedPawn.OnPossessed(this);
        Debug.Log($"{gameObject.name} possessed {pawnToPossess.gameObject.name}");
    }

    public virtual void Unpossess()
    {
        if (possessedPawn)
        {
            possessedPawn.OnUnpossessed();
            Debug.Log($"{gameObject.name} unpossessed {possessedPawn.gameObject.name}");
            possessedPawn = null;
        }
    }
}
