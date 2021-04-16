using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuidComponent))]   // need GuidComponent for runtime instances
public class UniqueAsset : Actor, IInitialize, IEndPlay
{
    [SerializeField, UniqueID]
    protected string id;

    public string ID => id;

    [SerializeField, ReadOnly]
    protected GuidComponent guidComponent;

    public GuidComponent GuidComponent => guidComponent;

    public void Initialize()
    {
        guidComponent = GetComponent<GuidComponent>();
        AssetIDs.Instance.AddRuntimeInstance(this);
    }

    public void EndPlay(EndPlayModeReason Reason)
    {
        AssetIDs.Instance.RemoveRuntimeInstance(this);
    }
}
