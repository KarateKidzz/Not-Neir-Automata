using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField, ReadOnly]
    protected Pawn possessedPawn;

    public Pawn PossessedPawn => possessedPawn;

    bool isQuitting;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    protected virtual void Start()
    {
        GameManager.Instance.AllControllers.Add(this);
        ActivateController();
    }

    protected virtual void OnDestroy()
    {
        if (!isQuitting)
        {
            GameManager.Instance.AllControllers.Remove(this);
            DisableController();
        }
    }

    public virtual void ActivateController()
    {

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
