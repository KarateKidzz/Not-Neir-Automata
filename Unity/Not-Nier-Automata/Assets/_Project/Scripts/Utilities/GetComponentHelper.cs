using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetComponentHelper
{
    /// <summary>
    /// Tries to get a component on the object, then its parents, then its children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetComponentInParentThenChildren<T>(this GameObject gameObject) where T : MonoBehaviour
    {
        if (!gameObject)
        {
            return null;
        }

        T component = gameObject.GetComponentInParent<T>();

        if (component)
        {
            return component;
        }

        component = gameObject.GetComponentInChildren<T>();

        return component;
    }
}
