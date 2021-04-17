using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuidComponent))]   // need GuidComponent for runtime instances
public class UniqueAsset : MonoBehaviour
{
    [SerializeField, UniqueID]
    protected string id;

    public string ID => id;

    [SerializeField, ReadOnly]
    protected GuidComponent guidComponent;

    public GuidComponent GuidComponent => guidComponent;

    private void Awake()
    {
        guidComponent = GetComponent<GuidComponent>();
    }

    private void OnEnable()
    {
        AssetIDs.Instance.AddRuntimeInstance(this);
    }

    private void OnDisable()
    {
        AssetIDs.Instance.RemoveRuntimeInstance(this);
    }
}
