using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class AssetIDs : ScriptableObject
{
    [System.Serializable]
    struct UniquePair
    {
        public string AssetID;
        public Object Object;
    }

    [System.Serializable]
    struct RuntimeAssets
    {
        public string AssetID;
        public List<UniqueAsset> Instances;

        public static RuntimeAssets Create()
        {
            RuntimeAssets runtimeAssets = new RuntimeAssets();
            runtimeAssets.AssetID = string.Empty;
            runtimeAssets.Instances = new List<UniqueAsset>();
            return runtimeAssets;
        }
    }

    /// <summary>
    /// Editor created dictionary of GUID strings to the prefabs
    /// </summary>
    [SerializeField, ReadOnly]
    List<UniquePair> assetsIDs = new List<UniquePair>();

    /// <summary>
    /// Runtime instances of GUID strings to the runtime objects
    /// </summary>
    List<RuntimeAssets> runtimeIDs = new List<RuntimeAssets>();

    const string AssetIDsAssetName = "AssetIDs";

    private static AssetIDs instance = null;
    private static bool isInitializing = false;

    private static Comparer<UniquePair> orderUniquePairsByAssetId = Comparer<UniquePair>.Create(
    (x, y) => x.AssetID.CompareTo(y.AssetID));

    private static Comparer<RuntimeAssets> orderRuntimeAssetsByAssetId = Comparer<RuntimeAssets>.Create(
        (x, y) => x.AssetID.CompareTo(y.AssetID));

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        AssetIDs tempInstance = Instance;
        Debug.Assert(tempInstance);
    }

    public static AssetIDs Instance
    {
        get
        {
            if (isInitializing)
            {
                return null;
            }

            if (instance == null)
            {
                isInitializing = true;
                instance = Resources.Load(AssetIDsAssetName) as AssetIDs;
                if (instance == null)
                {
                    Debug.Log("[Asset IDs] No Asset ID Database Exists. Creating AssetID database");
                    instance = CreateInstance<AssetIDs>();
                    instance.name = AssetIDsAssetName;

#if UNITY_EDITOR
                    if (!Directory.Exists("Assets/_Project/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets/_Project", "Resources");
                    }
                    AssetDatabase.CreateAsset(instance, "Assets/_Project/Resources" + AssetIDsAssetName + ".asset");
#endif
                }
                isInitializing = false;
            }
            return instance;
        }
    }

    public static void SaveAssetIDs()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(instance);
        AssetDatabase.SaveAssets();
#endif
    }

    public void AddRuntimeInstance(UniqueAsset uniqueAsset)
    {
        if (!uniqueAsset)
        {
            Debug.LogWarning("[Unique Asset] Can't add null asset");
            return;
        }

        string assetGUID = uniqueAsset.ID;

        int index = runtimeIDs.BinarySearch(new RuntimeAssets() { AssetID = assetGUID }, orderRuntimeAssetsByAssetId);

        if (index >= 0)
        {
            runtimeIDs[index].Instances.Add(uniqueAsset);
        }
        else
        {
            List<UniqueAsset> guidReferences = new List<UniqueAsset>();
            guidReferences.Add(uniqueAsset);
            runtimeIDs.Add(new RuntimeAssets() { AssetID = assetGUID, Instances = guidReferences });
        }
    }

    public void RemoveRuntimeInstance(UniqueAsset uniqueAsset)
    {
        if (!uniqueAsset || uniqueAsset == null)
        {
            Debug.LogWarning("[Unique Asset] Can't remove null asset");
            return;
        }

        string assetGUID = uniqueAsset.ID;

        int index = runtimeIDs.BinarySearch(new RuntimeAssets() { AssetID = assetGUID }, orderRuntimeAssetsByAssetId);

        if (index >= 0)
        {
            runtimeIDs[index].Instances.RemoveAll(gr => gr == null || gr.gameObject == null || gr.gameObject == uniqueAsset.gameObject);
        }
        // else, maybe throw error? we shouldn't get here if "AddRuntimeInstance" was called
    }

    public bool Set(string id, Object setObject)
    {
        int index = assetsIDs.BinarySearch(new UniquePair { AssetID = id }, orderUniquePairsByAssetId);
        if (index >= 0)
        {
            Object currentObject = assetsIDs[index].Object;
            if (setObject == currentObject)
            {
                return false;
            }

            Debug.LogError($"Old Object for ID and new object don't match. Old: {currentObject}. New {setObject}");

            assetsIDs[index] = new UniquePair() { Object = setObject, AssetID = id };
            return true;
        }
        else
        {
            Debug.Log($"[Asset IDs] Adding New ID:Object Pair --> {id}:{setObject}");
            assetsIDs.Add(new UniquePair() { AssetID = id, Object = setObject });
            return true;
        }
    }

    public Object GetPrefabObjectOfID(string id)
    {
        int index = assetsIDs.BinarySearch(new UniquePair { AssetID = id }, orderUniquePairsByAssetId);
        if (index >= 0)
        {
            return assetsIDs[index].Object;
        }
        return null;
    }

    public List<UniqueAsset> GetInstancesOfID(string id)
    {
        int index = runtimeIDs.BinarySearch(new RuntimeAssets { AssetID = id }, orderRuntimeAssetsByAssetId);
        if (index >= 0)
        {
            return runtimeIDs[index].Instances;
        }
        return null;
    }

    public UniqueAsset GetFirstInstanceOfID(string id)
    {
        List<UniqueAsset> allInstances = GetInstancesOfID(id);
        if (allInstances != null && allInstances.Count > 0)
        {
            return allInstances[0];
        }
        return null;
    }
}
