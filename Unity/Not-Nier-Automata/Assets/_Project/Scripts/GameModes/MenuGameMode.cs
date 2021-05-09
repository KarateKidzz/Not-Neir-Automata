using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameMode : GameMode, IBeginPlay
{
    public string podID;

    public void BeginPlay()
    {
        UniqueAsset pod = AssetIDs.Instance.GetFirstInstanceOfID(podID);

        if (pod)
        {
            pod.gameObject.transform.rotation = Quaternion.Euler(0, -136.9f, 0);
        }
    }
}
