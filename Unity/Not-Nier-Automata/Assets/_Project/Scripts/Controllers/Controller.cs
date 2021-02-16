using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected Pawn possessedPawn;

    public Pawn PossessedPawn => possessedPawn;

    public virtual void ActivateController()
    {

    }

    public virtual void DisableController()
    {

    }

    public virtual void Possess(Pawn pawnToPossess)
    {
        Unpossess();
        possessedPawn = pawnToPossess;
        possessedPawn.OnPossessed(this);
    }

    public virtual void Unpossess()
    {
        if (possessedPawn)
        {
            possessedPawn.OnUnpossessed();
            possessedPawn = null;
        }
    }
}
